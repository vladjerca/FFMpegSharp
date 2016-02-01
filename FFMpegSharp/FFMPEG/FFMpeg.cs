using FFMpegSharp.FFMPEG.Enums;
using FFMpegSharp.Helpers;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace FFMpegSharp.FFMPEG
{
    public delegate void ConversionHandler(int percentage);

    public class FFMpeg : FFBase, IDisposable
    {
        #region Private Members & Methods
        private string ffmpegPath;
        private TimeSpan totalTime;

        private volatile string errorData = string.Empty;

        private bool RunProcess(string args, FileInfo output)
        {
            bool SuccessState = true;

            RunProcess(args, ffmpegPath, rStandardInput: true, rStandardError: true);

            try
            {
                process.Start();
                process.ErrorDataReceived += OutputData;
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception)
            {
                SuccessState = false;
            }
            finally
            {
                process.Close();

                if (File.Exists(output.FullName))
                    using (var file = File.Open(output.FullName, FileMode.Open))
                    {
                        if (file.Length == 0)
                        {
                            throw new Exception(errorData);
                        }
                    }
                else
                {
                    throw new Exception(errorData);
                }
            }
            return SuccessState;
        }

        private void OutputData(object sender, DataReceivedEventArgs e)
        {
            if(e.Data != null)
                errorData = e.Data;

        #if DEBUG
            Trace.WriteLine(e.Data);
        #endif

            if (IsWorking && e.Data != null && OnProgress != null)
            {
                Regex r = new Regex(@"\w\w:\w\w:\w\w");
                Match m = r.Match(e.Data);
                if (e.Data.Contains("frame"))
                {
                    if (m.Success)
                    {
                        TimeSpan t = TimeSpan.Parse(m.Value);
                        int percentage = (int)(t.TotalSeconds / totalTime.TotalSeconds * 100);
                        OnProgress(percentage);
                    }
                }
            }
        }
        #endregion
   
        /// <summary>
        /// Passes the conversion percentage when encoding.
        /// </summary>
        public event ConversionHandler OnProgress;

        /// <summary>
        /// Intializes the FFMPEG encoder.
        /// </summary>
        /// <param name="rootPath">Directory root where ffmpeg.exe can be found. If not specified, root will be loaded from config.</param>
        public FFMpeg()
        {
            FFMpegHelper.RootExceptionCheck(configuredRoot);
            FFProbeHelper.RootExceptionCheck(configuredRoot);

            ffmpegPath = configuredRoot + "\\ffmpeg.exe";
        }

        /// <summary>
        /// Saves a 'png' thumbnail from the input video.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file</param>
        /// <param name="captureTime">Seek position where the thumbnail should be taken.</param>
        /// <param name="thumbWidth">Thumbnail width.</param>
        /// <param name="thumbHeight">Thumbnail height.</param>
        /// <returns>Success state.</returns>
        public bool SaveThumbnail(VideoInfo source, FileInfo output, Size? size = null, TimeSpan? captureTime = null)
        {
            if (captureTime == null)
                captureTime = TimeSpan.FromSeconds(source.Duration.TotalSeconds / 3);

            if (size == null)
                size = new Size(source.Width, source.Height);

            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, ".png");

            string thumbArgs,
                   thumbSize = string.Format("{0}x{1}", source.Width, source.Height);

            thumbArgs = String.Format("-i \"{0}\" -vcodec png -vframes 1 -ss {1} -s {2} \"{3}\"", source.Path,
                                                                                                  captureTime,
                                                                                                  thumbSize,
                                                                                                  output);

            return RunProcess(thumbArgs, output);
        }

        /// <summary>
        /// Converts a source video to MP4 format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <param name="speed">Conversion speed preset.</param>
        /// <param name="size">Output video size.</param>
        /// <param name="multithread">Use multithreading for conversion.</param>
        /// <returns>Success state.</returns>
        public bool ToMP4(VideoInfo source, FileInfo output, Speed speed = Speed.SuperFast, VideoSize size = VideoSize.Original, AudioQuality aQuality = AudioQuality.Normal, bool multithread = false)
        {
            totalTime = source.Duration;

            string threadCount = multithread ? 
                                Environment.ProcessorCount.ToString() : "1",
                                scale = FFMpegHelper.GetScale(size);

            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, ".mp4");

            string conversionArgs = string.Format("-i \"{0}\" -threads {1} {2} -b:v 2000k -vcodec libx264 -preset {3} -g 30 -codec:a aac -b:a {4}k -strict experimental \"{5}\"", source.Path,
                                                                                                                                                                                  threadCount,
                                                                                                                                                                                  scale,
                                                                                                                                                                                  speed.ToString().ToLower(),
                                                                                                                                                                                  (int)aQuality,
                                                                                                                                                                                  output);

            return RunProcess(conversionArgs, output);
        }

        /// <summary>
        /// Converts a source video to WebM format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <param name="size">Output video size.</param>
        /// <param name="multithread">Use multithreading for conversion.</param>
        /// <returns>Success state.</returns>
        public bool ToWebM(VideoInfo source, FileInfo output, VideoSize size = VideoSize.Original, AudioQuality aQuality = AudioQuality.Normal, bool multithread = false)
        {
            totalTime = source.Duration;

            string threadCount = multithread ? 
                                 Environment.ProcessorCount.ToString() : "1",
                   scale = FFMpegHelper.GetScale(size);

            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, ".webm");

            string conversionArgs = string.Format("-i \"{0}\" -threads {1} {2} -vcodec libvpx -quality good -cpu-used 16 -deadline realtime -b:v 2000k -qmin 10 -qmax 42 -maxrate 500k -bufsize 1000k -codec:a libvorbis -b:a {3}k \"{4}\"", source.Path,
                                                                                                                                                                                                                         threadCount,
                                                                                                                                                                                                                         scale,
                                                                                                                                                                                                                         (int)aQuality,
                                                                                                                                                                                                                         output);

            return RunProcess(conversionArgs, output);
        }

        /// <summary>
        /// Converts a source video to OGV format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <param name="size">Output video size.</param>
        /// <param name="multithread">Use multithreading for conversion.</param>
        /// <returns>Success state.</returns>
        public bool ToOGV(VideoInfo source, FileInfo output, VideoSize size = VideoSize.Original, AudioQuality aQuality = AudioQuality.Normal, bool multithread = false)
        {
            totalTime = source.Duration;

            string threadCount = multithread ? 
                                 Environment.ProcessorCount.ToString() : "1",
                   scale = FFMpegHelper.GetScale(size);

            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, ".ogv");

            string conversionArgs = string.Format("-i \"{0}\" -threads {1} {2} -codec:v libtheora -qscale:v 7 -cpu-used 16 -deadline realtime -codec:a libvorbis -codec:a libvorbis -b:a {3}k -qscale:a 5 \"{4}\"", source.Path,
                                                                                                                                                       threadCount,
                                                                                                                                                       scale,
                                                                                                                                                       (int)aQuality,
                                                                                                                                                       output);
            return RunProcess(conversionArgs, output);
        }

        /// <summary>
        /// Converts a source video to TS format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <returns>Success state.</returns>
        public bool ToTS(VideoInfo source, FileInfo output)
        {
            totalTime = source.Duration;

            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, ".ts");

            string conversionArgs = string.Format("-i \"{0}\" -c copy -bsf:v h264_mp4toannexb -f mpegts \"{1}\"", source.Path, output);
            return RunProcess(conversionArgs, output);
        }

        /// <summary>
        /// Adds a poster image to an audio file.
        /// </summary>
        /// <param name="image">Source image file.</param>
        /// <param name="audio">Source audio file.</param>
        /// <param name="output">Output video file.</param>
        /// <returns></returns>
        public bool AddPosterToAudio(FileInfo image, FileInfo audio, FileInfo output)
        {
            FFMpegHelper.InputFilesExistExceptionCheck(image, audio);
            FFMpegHelper.ExtensionExceptionCheck(output, ".mp4");

            string args = string.Format("-loop 1 -i \"{0}\" -i \"{1}\" -b:v 2000k -vcodec libx264 -c:a aac -strict experimental -shortest \"{2}\"", image, audio, output);

            return RunProcess(args, output);
        }

        /// <summary>
        /// Joins a list of video files.
        /// </summary>
        /// <param name="output">Output video file.</param>
        /// <param name="videos">List of vides that need to be joined together.</param>
        /// <returns>Success state.</returns>
        public bool Join(FileInfo output, params VideoInfo[] videos)
        {
            string[] pathList = new string[videos.Length];
            for (int i = 0; i < pathList.Length; i++)
            {
                pathList[i] = videos[i].Path.Replace(videos[i].Extension, ".ts");
                ToTS(videos[i], new FileInfo(videos[i].Path.Replace("mp4", "ts")));
            }

            string conversionArgs = string.Format("-i \"concat:{0}\" -c copy -bsf:a aac_adtstoasc \"{1}\"", string.Join(@"|", (object[])pathList), output);
            bool result = RunProcess(conversionArgs, output);

            if (result)
                foreach (string path in pathList)
                    if (File.Exists(path))
                        File.Delete(path);

            return result;
        }

        /// <summary>
        /// Records M3U8 streams to the specified output.
        /// </summary>
        /// <param name="uri">URI to pointing towards stream.</param>
        /// <param name="output">Output file</param>
        /// <returns>Success state.</returns>
        public bool SaveM3U8Stream(Uri uri, FileInfo output)
        {
            FFMpegHelper.ExtensionExceptionCheck(output, ".mp4");

            if (uri.Scheme == "http" || uri.Scheme == "https")
            {
                string conversionArgs = string.Format("-i \"{0}\" \"{1}\"", uri.AbsoluteUri, output);
                return RunProcess(conversionArgs, output);
            }
            else throw new ArgumentException("Uri: {0}, does not point to a valid http(s) stream.", uri.AbsoluteUri);
        }

        /// <summary>
        /// Strips a video file of audio.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <returns></returns>
        public bool Mute(VideoInfo source, FileInfo output)
        {
            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, source.Extension);

            string args = string.Format("-i \"{0}\" -c copy -an \"{1}\"", source.Path, output);

            return RunProcess(args, output);
        }

        /// <summary>
        /// Saves audio from a specific video file to disk.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output audio file.</param>
        /// <returns>Success state.</returns>
        public bool SaveAudio(VideoInfo source, FileInfo output)
        {
            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, ".mp3");

            string args = string.Format("-i \"{0}\" -vn -ab 256 \"{1}\"", source.Path, output);

            return RunProcess(args, output);
        }

        /// <summary>
        /// Adds audio to a video file.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="audio">Source audio file.</param>
        /// <param name="output">Output video file.</param>
        /// <param name="stopAtShortest">Indicates if the encoding should stop at the shortest input file.</param>
        /// <returns>Success state</returns>
        public bool AddAudio(VideoInfo source, FileInfo audio, FileInfo output, bool stopAtShortest = false)
        {
            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.InputFilesExistExceptionCheck(audio);
            FFMpegHelper.ExtensionExceptionCheck(output, source.Extension);

            string args = string.Format("-i \"{0}\" -i \"{1}\" -c:v copy -c:a aac -strict experimental {3} \"{2}\"", source.Path, audio, output, stopAtShortest ? "-shortest" : "");

            return RunProcess(args, output);
        }

        /// <summary>
        /// Stops any current job that FFMpeg is running.
        /// </summary>
        public void Stop()
        {
            if(IsWorking)
            {
                process.StandardInput.Write('q');
            }
        }
    }
}
