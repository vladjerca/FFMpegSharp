using FFMpegSharp.FFMPEG.Atomic;
using FFMpegSharp.FFMPEG.Enums;
using FFMpegSharp.FFMPEG.Exceptions;
using FFMpegSharp.Helpers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace FFMpegSharp.FFMPEG
{
    public delegate void ConversionHandler(double percentage);

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
                            throw new FFMpegException(FFMpegExceptionType.Conversion, errorData);
                        }
                    }
                else
                {
                    throw new FFMpegException(FFMpegExceptionType.Conversion, errorData);
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

            if (OnProgress != null && e.Data != null && IsWorking)
            {
                Regex r = new Regex(@"\w\w:\w\w:\w\w");
                Match m = r.Match(e.Data);
                if (e.Data.Contains("frame"))
                {
                    if (m.Success)
                    {
                        TimeSpan t = TimeSpan.Parse(m.Value);
                        double percentage = Math.Round((t.TotalSeconds / totalTime.TotalSeconds * 100), 2);
                        OnProgress(percentage);
                    }
                }
            }
        }
        #endregion

        //// <summary>
        /// Returns the percentage of the current conversion progress.
        /// </summary>
        public event ConversionHandler OnProgress;

        /// <summary>
        /// Intializes the FFMPEG encoder.
        /// </summary>
        public FFMpeg()
        {
            FFMpegHelper.RootExceptionCheck(configuredRoot);
            FFProbeHelper.RootExceptionCheck(configuredRoot);

            var target = Environment.Is64BitProcess ? "x64" : "x86";

            ffmpegPath = configuredRoot + string.Format("\\{0}\\ffmpeg.exe", target);
        }

        /// <summary>
        /// Saves a 'png' thumbnail from the input video.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file</param>
        /// <param name="captureTime">Seek position where the thumbnail should be taken.</param>
        /// <param name="size">Thumbnail size. If width or height equal 0, the other will be computed automatically.</param>
        /// <returns>Success state.</returns>
        public bool Snapshot(VideoInfo source, FileInfo output, Size? size = null, TimeSpan? captureTime = null)
        {
            if (captureTime == null)
                captureTime = TimeSpan.FromSeconds(source.Duration.TotalSeconds / 3);

            if (output.Extension.ToLower() != FFMpegHelper.Extensions.PNG)
                output = new FileInfo(output.FullName.Replace(output.Extension, FFMpegHelper.Extensions.PNG));

            if (size == null || (size.Value.Height == 0 && size.Value.Width == 0))
            {
                size = new Size(source.Width, source.Height);
            }
            
            if( size.Value.Width != size.Value.Height )
            {
                if (size.Value.Width == 0)
                {
                    double ratio = source.Width / (double)size.Value.Width;

                    size = new Size((int)(source.Width * ratio), (int)(source.Height * ratio));
                }

                if(size.Value.Height == 0)
                {
                    double ratio = source.Height / (double)size.Value.Height;

                    size = new Size((int)(source.Width * ratio), (int)(source.Height * ratio));
                }
            }    

            FFMpegHelper.ConversionExceptionCheck(source, output);

            string thumbArgs = Arguments.Input(source) +
                               Arguments.Video(VideoCodec.PNG) + 
                               Arguments.FrameOutputCount(1) +
                               Arguments.Seek(captureTime) +
                               Arguments.Size(size) +
                               Arguments.Output(output);

            return RunProcess(thumbArgs, output);
        }

        /// <summary>
        /// Converts a source video to MP4 format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <param name="speed">Conversion speed preset.</param>
        /// <param name="size">Output video size.</param>
        /// <param name="aQuality">Output audio quality.</param>
        /// <param name="multithread">Use multithreading for conversion.</param>
        /// <returns>Success state.</returns>
        public bool ToMP4(VideoInfo source, FileInfo output, Speed speed = Speed.SuperFast, VideoSize size = VideoSize.Original, AudioQuality aQuality = AudioQuality.Normal, bool multithread = false)
        {
            totalTime = source.Duration;

            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, FFMpegHelper.Extensions.MP4);

            string conversionArgs = Arguments.Input(source) +
                                    Arguments.Threads(multithread) +
                                    Arguments.Scale(size) +
                                    Arguments.Video(VideoCodec.LibX264, 2400) +
                                    Arguments.Speed(speed) +
                                    Arguments.Audio(AudioCodec.AAC, aQuality) +
                                    Arguments.Output(output);

            return RunProcess(conversionArgs, output);
        }

        /// <summary>
        /// Converts a source video to WebM format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <param name="size">Output video size.</param>
        /// <param name="aQuality">Output audio quality.</param>
        /// <param name="multithread">Use multithreading for conversion.</param>
        /// <returns>Success state.</returns>
        public bool ToWebM(VideoInfo source, FileInfo output, VideoSize size = VideoSize.Original, AudioQuality aQuality = AudioQuality.Normal)
        {
            totalTime = source.Duration;

            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, FFMpegHelper.Extensions.WebM);

            string conversionArgs = Arguments.Input(source) +
                                    Arguments.Scale(size) +
                                    Arguments.Video(VideoCodec.LibVPX, 2400) +
                                    Arguments.Speed(16) +
                                    Arguments.Audio(AudioCodec.LibVorbis, aQuality) +
                                    Arguments.Output(output);

            return RunProcess(conversionArgs, output);
        }

        /// <summary>
        /// Converts a source video to OGV format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <param name="size">Output video size.</param>
        /// <param name="aQuality">Output audio quality.</param>
        /// <param name="multithread">Use multithreading for conversion.</param>
        /// <returns>Success state.</returns>
        public bool ToOGV(VideoInfo source, FileInfo output, VideoSize size = VideoSize.Original, AudioQuality aQuality = AudioQuality.Normal, bool multithread = false)
        {
            totalTime = source.Duration;

            string threadCount = multithread ?
                                 Environment.ProcessorCount.ToString() : "1";

            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, FFMpegHelper.Extensions.OGV);

            string conversionArgs = Arguments.Input(source) +
                                    Arguments.Threads(multithread) +
                                    Arguments.Scale(size) +
                                    Arguments.Video(VideoCodec.LibTheora, 2400) +
                                    Arguments.Speed(16) +
                                    Arguments.Audio(AudioCodec.LibVorbis, aQuality) +
                                    Arguments.Output(output);

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
            FFMpegHelper.ExtensionExceptionCheck(output, FFMpegHelper.Extensions.TS);

            string conversionArgs = Arguments.Input(source) +
                                    Arguments.Copy() +
                                    Arguments.BitStreamFilter(Channel.Video, Filter.H264_MP4ToAnnexB) +
                                    Arguments.ForceFormat(VideoCodec.MpegTS) +
                                    Arguments.Output(output);

            return RunProcess(conversionArgs, output);
        }

        /// <summary>
        /// Adds a poster image to an audio file.
        /// </summary>
        /// <param name="image">Source image file.</param>
        /// <param name="audio">Source audio file.</param>
        /// <param name="output">Output video file.</param>
        /// <returns></returns>
        public bool PosterWithAudio(FileInfo image, FileInfo audio, FileInfo output)
        {
            FFMpegHelper.InputFilesExistExceptionCheck(image, audio);
            FFMpegHelper.ExtensionExceptionCheck(output, FFMpegHelper.Extensions.MP4);

            string args = Arguments.Loop(1) +
                          Arguments.Input(image) +
                          Arguments.Input(audio) +
                          Arguments.Video(VideoCodec.LibX264, 2400) +
                          Arguments.Audio(AudioCodec.AAC, AudioQuality.Normal) +
                          Arguments.FinalizeAtShortestInput() +
                          Arguments.Output(output);

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
                pathList[i] = videos[i].FullName.Replace(videos[i].Extension, FFMpegHelper.Extensions.TS);
                ToTS(videos[i], new FileInfo(videos[i].FullName.Replace(FFMpegHelper.Extensions.MP4, FFMpegHelper.Extensions.TS)));
            }

            string conversionArgs = Arguments.InputConcat(pathList) +
                                    Arguments.Copy() +
                                    Arguments.BitStreamFilter(Channel.Audio, Filter.AAC_ADTSToASC) +
                                    Arguments.Output(output);

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
            FFMpegHelper.ExtensionExceptionCheck(output, FFMpegHelper.Extensions.MP4);

            if (uri.Scheme == "http" || uri.Scheme == "https")
            {
                string conversionArgs = Arguments.Input(uri) +
                                        Arguments.Output(output);

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

            string args = Arguments.Input(source) +
                          Arguments.Copy() +
                          Arguments.Disable(Channel.Audio) +
                          Arguments.Output(output);

            return RunProcess(args, output);
        }

        /// <summary>
        /// Saves audio from a specific video file to disk.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output audio file.</param>
        /// <returns>Success state.</returns>
        public bool ExtractAudio(VideoInfo source, FileInfo output)
        {
            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, FFMpegHelper.Extensions.MP3);

            string args = Arguments.Input(source) +
                          Arguments.Disable(Channel.Video) +
                          Arguments.Output(output);

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
        public bool ReplaceAudio(VideoInfo source, FileInfo audio, FileInfo output, bool stopAtShortest = false)
        {
            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.InputFilesExistExceptionCheck(audio);
            FFMpegHelper.ExtensionExceptionCheck(output, source.Extension);

            string args = Arguments.Input(source) +
                          Arguments.Input(audio) +
                          Arguments.Copy(Channel.Video) +
                          Arguments.Audio(AudioCodec.AAC, AudioQuality.HD) +
                          Arguments.FinalizeAtShortestInput() +
                          Arguments.Output(output);

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
