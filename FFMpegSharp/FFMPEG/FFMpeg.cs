using FFMpegSharp.Enums;
using FFMpegSharp.FFMPEG.Atomic;
using FFMpegSharp.FFMPEG.Enums;
using FFMpegSharp.FFMPEG.Exceptions;
using FFMpegSharp.Helpers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace FFMpegSharp.FFMPEG
{
    public delegate void ConversionHandler(double percentage);

    public class FFMpeg : FFBase
    {
        /// <summary>
        ///     Intializes the FFMPEG encoder.
        /// </summary>
        public FFMpeg()
        {
            FFMpegHelper.RootExceptionCheck(ConfiguredRoot);
            FFProbeHelper.RootExceptionCheck(ConfiguredRoot);

            var target = Environment.Is64BitProcess ? "x64" : "x86";

            _ffmpegPath = ConfiguredRoot + $"\\{target}\\ffmpeg.exe";
        }

        /// <summary>
        /// Returns the percentage of the current conversion progress.
        /// </summary>
        public event ConversionHandler OnProgress;

        /// <summary>
        ///     Saves a 'png' thumbnail from the input video.
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

            if (output.Extension.ToLower() != FileExtension.Png)
                output = new FileInfo(output.FullName.Replace(output.Extension, FileExtension.Png));

            if (size == null || (size.Value.Height == 0 && size.Value.Width == 0))
            {
                size = new Size(source.Width, source.Height);
            }

            if (size.Value.Width != size.Value.Height)
            {
                if (size.Value.Width == 0)
                {
                    var ratio = source.Width / (double)size.Value.Width;

                    size = new Size((int)(source.Width * ratio), (int)(source.Height * ratio));
                }

                if (size.Value.Height == 0)
                {
                    var ratio = source.Height / (double)size.Value.Height;

                    size = new Size((int)(source.Width * ratio), (int)(source.Height * ratio));
                }
            }

            FFMpegHelper.ConversionExceptionCheck(source, output);

            var thumbArgs = Arguments.Input(source) +
                            Arguments.Video(VideoCodec.Png) +
                            Arguments.FrameOutputCount(1) +
                            Arguments.Seek(captureTime) +
                            Arguments.Size(size) +
                            Arguments.Output(output);

            return RunProcess(thumbArgs, output);
        }

        /// <summary>
        ///     Converts a source video to MP4 format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <param name="speed">Conversion speed preset.</param>
        /// <param name="size">Output video size.</param>
        /// <param name="aQuality">Output audio quality.</param>
        /// <param name="multithread">Use multithreading for conversion.</param>
        /// <returns>Success state.</returns>
        public bool ToMp4(VideoInfo source, FileInfo output, Speed speed = Speed.SuperFast,
            VideoSize size = VideoSize.Original, AudioQuality aQuality = AudioQuality.Normal, bool multithread = false)
        {
            _totalTime = source.Duration;

            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, FileExtension.Mp4);

            var newHeight = source.Height * (source.Width / (int)size);

            var conversionArgs = Arguments.Input(source) +
                                 Arguments.Threads(multithread) +
                                 Arguments.Scale(size, newHeight) +
                                 Arguments.Video(VideoCodec.LibX264, 2400) +
                                 Arguments.Speed(speed) +
                                 Arguments.Audio(AudioCodec.Aac, aQuality) +
                                 Arguments.Output(output);

            return RunProcess(conversionArgs, output);
        }

        /// <summary>
        ///     Converts a source video to WebM format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <param name="size">Output video size.</param>
        /// <param name="aQuality">Output audio quality.</param>
        /// <returns>Success state.</returns>
        public bool ToWebM(VideoInfo source, FileInfo output, VideoSize size = VideoSize.Original,
            AudioQuality aQuality = AudioQuality.Normal)
        {
            _totalTime = source.Duration;

            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, FileExtension.WebM);

            var newHeight = source.Height * (source.Width / (int)size);

            var conversionArgs = Arguments.Input(source) +
                                 Arguments.Scale(size, newHeight) +
                                 Arguments.Video(VideoCodec.LibVpx, 2400) +
                                 Arguments.Speed(16) +
                                 Arguments.Audio(AudioCodec.LibVorbis, aQuality) +
                                 Arguments.Output(output);

            return RunProcess(conversionArgs, output);
        }

        /// <summary>
        ///     Converts a source video to OGV format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <param name="size">Output video size.</param>
        /// <param name="aQuality">Output audio quality.</param>
        /// <param name="multithread">Use multithreading for conversion.</param>
        /// <returns>Success state.</returns>
        public bool ToOgv(VideoInfo source, FileInfo output, VideoSize size = VideoSize.Original,
            AudioQuality aQuality = AudioQuality.Normal, bool multithread = false)
        {
            _totalTime = source.Duration;

            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, FileExtension.Ogv);

            var newHeight = source.Height * (source.Width / (int)size);

            var conversionArgs = Arguments.Input(source) +
                                 Arguments.Threads(multithread) +
                                 Arguments.Scale(size, newHeight) +
                                 Arguments.Video(VideoCodec.LibTheora, 2400) +
                                 Arguments.Speed(16) +
                                 Arguments.Audio(AudioCodec.LibVorbis, aQuality) +
                                 Arguments.Output(output);

            return RunProcess(conversionArgs, output);
        }

        /// <summary>
        ///     Converts a source video to TS format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <returns>Success state.</returns>
        public bool ToTs(VideoInfo source, FileInfo output)
        {
            _totalTime = source.Duration;

            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, FileExtension.Ts);

            var conversionArgs = Arguments.Input(source) +
                                 Arguments.Copy() +
                                 Arguments.BitStreamFilter(Channel.Video, Filter.H264_Mp4ToAnnexB) +
                                 Arguments.ForceFormat(VideoCodec.MpegTs) +
                                 Arguments.Output(output);

            return RunProcess(conversionArgs, output);
        }

        /// <summary>
        ///     Adds a poster image to an audio file.
        /// </summary>
        /// <param name="image">Source image file.</param>
        /// <param name="audio">Source audio file.</param>
        /// <param name="output">Output video file.</param>
        /// <returns></returns>
        public bool PosterWithAudio(FileInfo image, FileInfo audio, FileInfo output)
        {
            FFMpegHelper.InputFilesExistExceptionCheck(image, audio);
            FFMpegHelper.ExtensionExceptionCheck(output, FileExtension.Mp4);

            var args = Arguments.Loop(1) +
                       Arguments.Input(image) +
                       Arguments.Input(audio) +
                       Arguments.Video(VideoCodec.LibX264, 2400) +
                       Arguments.Audio(AudioCodec.Aac, AudioQuality.Normal) +
                       Arguments.FinalizeAtShortestInput() +
                       Arguments.Output(output);

            return RunProcess(args, output);
        }

        /// <summary>
        ///     Joins a list of video files.
        /// </summary>
        /// <param name="output">Output video file.</param>
        /// <param name="videos">List of vides that need to be joined together.</param>
        /// <returns>Success state.</returns>
        public bool Join(FileInfo output, params VideoInfo[] videos)
        {
            var pathList = new string[videos.Length];

            for (var i = 0; i < pathList.Length; i++)
            {
                pathList[i] = videos[i].FullName.Replace(videos[i].Extension, FileExtension.Ts);
                ToTs(videos[i],
                    new FileInfo(videos[i].FullName.Replace(FileExtension.Mp4, FileExtension.Ts)));
            }

            var conversionArgs = Arguments.InputConcat(pathList) +
                                 Arguments.Copy() +
                                 Arguments.BitStreamFilter(Channel.Audio, Filter.Aac_AdtstoAsc) +
                                 Arguments.Output(output);

            try
            {
                return RunProcess(conversionArgs, output);
            }
            finally
            {
                Cleanup(pathList);
            }
        }

        public VideoInfo JoinImageSequence(FileInfo output, double frameRate = 30, params ImageInfo[] images)
        {
            var pathList = new string[images.Length];

            for (var i = 0; i < pathList.Length; i++)
            {
                pathList[i] = images[i].FullName.Replace(images[i].Name, $"{i.ToString().PadLeft(9, '0')}{images[i].Extension}");

                File.Copy(images[i].FullName, pathList[i]);
            }

            var firstImage = images[0];

            var conversionArgs = Arguments.FrameRate(frameRate) +
                Arguments.Size(new Size(firstImage.Width, firstImage.Height)) +
                Arguments.StartNumber(0) +
                Arguments.Input($"{firstImage.Directory}\\%09d.png") +
                Arguments.FrameOutputCount(images.Length) +
                Arguments.Video(VideoCodec.LibX264) +
                Arguments.YuvFormat() +
                Arguments.Output(output);
            try
            {
                var result = RunProcess(conversionArgs, output);

                if (result)
                {
                    return new VideoInfo(output);
                }
                else
                {
                    throw new OperationCanceledException("Could not join the provided image sequence.");
                }
            }
            finally
            {
                Cleanup(pathList);
            }
        }

        /// <summary>
        ///     Records M3U8 streams to the specified output.
        /// </summary>
        /// <param name="uri">URI to pointing towards stream.</param>
        /// <param name="output">Output file</param>
        /// <returns>Success state.</returns>
        public bool SaveM3U8Stream(Uri uri, FileInfo output)
        {
            FFMpegHelper.ExtensionExceptionCheck(output, FileExtension.Mp4);

            if (uri.Scheme == "http" || uri.Scheme == "https")
            {
                var conversionArgs = Arguments.Input(uri) +
                                     Arguments.Output(output);

                return RunProcess(conversionArgs, output);
            }
            throw new ArgumentException("Uri: {0}, does not point to a valid http(s) stream.", uri.AbsoluteUri);
        }

        /// <summary>
        ///     Strips a video file of audio.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <returns></returns>
        public bool Mute(VideoInfo source, FileInfo output)
        {
            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, source.Extension);

            var args = Arguments.Input(source) +
                       Arguments.Copy() +
                       Arguments.Disable(Channel.Audio) +
                       Arguments.Output(output);

            return RunProcess(args, output);
        }

        /// <summary>
        ///     Saves audio from a specific video file to disk.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output audio file.</param>
        /// <returns>Success state.</returns>
        public bool ExtractAudio(VideoInfo source, FileInfo output)
        {
            FFMpegHelper.ConversionExceptionCheck(source, output);
            FFMpegHelper.ExtensionExceptionCheck(output, FileExtension.Mp3);

            var args = Arguments.Input(source) +
                       Arguments.Disable(Channel.Video) +
                       Arguments.Output(output);

            return RunProcess(args, output);
        }

        /// <summary>
        ///     Adds audio to a video file.
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

            var args = Arguments.Input(source) +
                       Arguments.Input(audio) +
                       Arguments.Copy(Channel.Video) +
                       Arguments.Audio(AudioCodec.Aac, AudioQuality.Hd) +
                       Arguments.FinalizeAtShortestInput() +
                       Arguments.Output(output);

            return RunProcess(args, output);
        }

        /// <summary>
        ///     Stops any current job that FFMpeg is running.
        /// </summary>
        public void Stop()
        {
            if (IsWorking)
            {
                Process.StandardInput.Write('q');
            }
        }

        #region Private Members & Methods

        private readonly string _ffmpegPath;
        private TimeSpan _totalTime;

        private volatile string _errorData = string.Empty;

        private bool RunProcess(string args, FileInfo output)
        {
            var successState = true;

            RunProcess(args, _ffmpegPath, true, rStandardError: true);

            try
            {
                Process.Start();
                Process.ErrorDataReceived += OutputData;
                Process.BeginErrorReadLine();
                Process.WaitForExit();
            }
            catch (Exception)
            {
                successState = false;
            }
            finally
            {
                Process.Close();

                if (File.Exists(output.FullName))
                    using (var file = File.Open(output.FullName, FileMode.Open))
                    {
                        if (file.Length == 0)
                        {
                            throw new FFMpegException(FFMpegExceptionType.Conversion, _errorData);
                        }
                    }
                else
                {
                    throw new FFMpegException(FFMpegExceptionType.Conversion, _errorData);
                }
            }
            return successState;
        }

        private void Cleanup(string[] pathList)
        {
            foreach (var path in pathList)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private void OutputData(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            _errorData = e.Data;
#if DEBUG
            Trace.WriteLine(e.Data);
#endif

            if (OnProgress == null || !IsWorking) return;

            var r = new Regex(@"\w\w:\w\w:\w\w");
            var m = r.Match(e.Data);

            if (!e.Data.Contains("frame")) return;
            if (!m.Success) return;

            var t = TimeSpan.Parse(m.Value, CultureInfo.InvariantCulture);
            var percentage = Math.Round(t.TotalSeconds / _totalTime.TotalSeconds * 100, 2);
            OnProgress(percentage);
        }

        #endregion
    }
}