﻿using FFMpegSharp.Enums;
using FFMpegSharp.FFMPEG.Arguments;
using FFMpegSharp.FFMPEG.Enums;
using FFMpegSharp.FFMPEG.Exceptions;
using FFMpegSharp.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FFMpegSharp.FFMPEG
{
    public delegate void ConversionHandler(double percentage);

    public class FFMpeg : FFBase
    {
        IArgumentBuilder argumentBuilder { get; set; }

        /// <summary>
        ///     Intializes the FFMPEG encoder.
        /// </summary>
        public FFMpeg()
        {
            _Init();
            argumentBuilder = new FFArgumentBuilder();
        }

        public FFMpeg(IArgumentBuilder builder)
        {
            _Init();
            argumentBuilder = builder;
        }

        private void _Init()
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
        /// <returns>Bitmap with the requested snapshot.</returns>
        public Bitmap Snapshot(VideoInfo source, FileInfo output, Size? size = null, TimeSpan? captureTime = null)
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

            FFMpegHelper.ConversionExceptionCheck(source.ToFileInfo(), output);

            var thumbArgs = ArgumentsStringifier.Input(source) +
                            ArgumentsStringifier.Video(VideoCodec.Png) +
                            ArgumentsStringifier.FrameOutputCount(1) +
                            ArgumentsStringifier.Seek(captureTime) +
                            ArgumentsStringifier.Size(size) +
                            ArgumentsStringifier.Output(output);

            if (!RunProcess(thumbArgs, output))
            {
                throw new OperationCanceledException("Could not take snapshot!");
            }

            output.Refresh();

            Bitmap result;
            using (var bmp = (Bitmap)Image.FromFile(output.FullName))
            {
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    result = new Bitmap(ms);
                }
            }

            if (output.Exists)
            {
                output.Delete();
            }

            return result;
        }

        /// <summary>
        /// Convert a video do a different format.
        /// </summary>
        /// <param name="source">Input video source.</param>
        /// <param name="output">Output information.</param>
        /// <param name="type">Target conversion video type.</param>
        /// <param name="speed">Conversion target speed/quality (faster speed = lower quality).</param>
        /// <param name="size">Video size.</param>
        /// <param name="audioQuality">Conversion target audio quality.</param>
        /// <param name="multithreaded">Is encoding multithreaded.</param>
        /// <returns>Output video information.</returns>
        public VideoInfo Convert(
            VideoInfo source,
            FileInfo output,
            VideoType type = VideoType.Mp4,
            Speed speed =
            Speed.SuperFast,
            VideoSize size =
            VideoSize.Original,
            AudioQuality audioQuality = AudioQuality.Normal,
            bool multithreaded = false)
        {
            FFMpegHelper.ConversionExceptionCheck(source.ToFileInfo(), output);
            FFMpegHelper.ExtensionExceptionCheck(output, FileExtension.ForType(type));
            FFMpegHelper.ConversionSizeExceptionCheck(source);

            string args = "";

            var height = source.Height * (source.Width / (int)size);

            switch (type)
            {
                case VideoType.Mp4:
                    args = ArgumentsStringifier.Input(source) +
                                 ArgumentsStringifier.Threads(multithreaded) +
                                 ArgumentsStringifier.Scale(size, height) +
                                 ArgumentsStringifier.Video(VideoCodec.LibX264, 2400) +
                                 ArgumentsStringifier.Speed(speed) +
                                 ArgumentsStringifier.Audio(AudioCodec.Aac, audioQuality) +
                                 ArgumentsStringifier.Output(output);
                    break;
                case VideoType.Ogv:
                    args = ArgumentsStringifier.Input(source) +
                                 ArgumentsStringifier.Threads(multithreaded) +
                                 ArgumentsStringifier.Scale(size, height) +
                                 ArgumentsStringifier.Video(VideoCodec.LibTheora, 2400) +
                                 ArgumentsStringifier.Speed(16) +
                                 ArgumentsStringifier.Audio(AudioCodec.LibVorbis, audioQuality) +
                                 ArgumentsStringifier.Output(output);
                    break;
                case VideoType.Ts:
                    args = ArgumentsStringifier.Input(source) +
                                 ArgumentsStringifier.Copy() +
                                 ArgumentsStringifier.BitStreamFilter(Channel.Video, Filter.H264_Mp4ToAnnexB) +
                                 ArgumentsStringifier.ForceFormat(VideoCodec.MpegTs) +
                                 ArgumentsStringifier.Output(output);
                    break;
            }

            if (!RunProcess(args, output))
            {
                throw new FFMpegException(FFMpegExceptionType.Conversion, $"The video could not be converted to {Enum.GetName(typeof(VideoType), type)}");
            }

            return new VideoInfo(output);
        }

        /// <summary>
        ///     Adds a poster image to an audio file.
        /// </summary>
        /// <param name="image">Source image file.</param>
        /// <param name="audio">Source audio file.</param>
        /// <param name="output">Output video file.</param>
        /// <returns></returns>
        public VideoInfo PosterWithAudio(FileInfo image, FileInfo audio, FileInfo output)
        {
            FFMpegHelper.InputsExistExceptionCheck(image, audio);
            FFMpegHelper.ExtensionExceptionCheck(output, FileExtension.Mp4);
            FFMpegHelper.ConversionSizeExceptionCheck(Image.FromFile(image.FullName));

            var args = ArgumentsStringifier.Loop(1) +
                       ArgumentsStringifier.Input(image) +
                       ArgumentsStringifier.Input(audio) +
                       ArgumentsStringifier.Video(VideoCodec.LibX264, 2400) +
                       ArgumentsStringifier.Audio(AudioCodec.Aac, AudioQuality.Normal) +
                       ArgumentsStringifier.FinalizeAtShortestInput(true) +
                       ArgumentsStringifier.Output(output);

            if (!RunProcess(args, output))
            {
                throw new FFMpegException(FFMpegExceptionType.Operation, "An error occured while adding the audio file to the image.");
            }

            return new VideoInfo(output);
        }

        /// <summary>
        ///     Joins a list of video files.
        /// </summary>
        /// <param name="output">Output video file.</param>
        /// <param name="videos">List of vides that need to be joined together.</param>
        /// <returns>Output video information.</returns>
        public VideoInfo Join(FileInfo output, params VideoInfo[] videos)
        {
            FFMpegHelper.OutputExistsExceptionCheck(output);
            FFMpegHelper.InputsExistExceptionCheck(videos.Select(video => video.ToFileInfo()).ToArray());

            var temporaryVideoParts = videos.Select(video =>
            {
                FFMpegHelper.ConversionSizeExceptionCheck(video);
                var destinationPath = video.FullName.Replace(video.Extension, FileExtension.Ts);
                Convert(
                   video,
                   new FileInfo(destinationPath),
                   VideoType.Ts
               );
                return destinationPath;
            }).ToList();

            var args = ArgumentsStringifier.InputConcat(temporaryVideoParts) +
                ArgumentsStringifier.Copy() +
                ArgumentsStringifier.BitStreamFilter(Channel.Audio, Filter.Aac_AdtstoAsc) +
                ArgumentsStringifier.Output(output);

            try
            {
                if (!RunProcess(args, output))
                {
                    throw new FFMpegException(FFMpegExceptionType.Operation, "Could not join the provided video files.");
                }
                return new VideoInfo(output);

            }
            finally
            {
                Cleanup(temporaryVideoParts);
            }
        }

        /// <summary>
        /// Converts an image sequence to a video.
        /// </summary>
        /// <param name="output">Output video file.</param>
        /// <param name="frameRate">FPS</param>
        /// <param name="images">Image sequence collection</param>
        /// <returns>Output video information.</returns>
        public VideoInfo JoinImageSequence(FileInfo output, double frameRate = 30, params ImageInfo[] images)
        {            
            var temporaryImageFiles = images.Select((image, index) =>
            {
                FFMpegHelper.ConversionSizeExceptionCheck(Image.FromFile(image.FullName));
                var destinationPath = image.FullName.Replace(image.Name, $"{index.ToString().PadLeft(9, '0')}{image.Extension}");
                File.Copy(image.FullName, destinationPath);

                return destinationPath;
            }).ToList();

            var firstImage = images.First();

            var args = ArgumentsStringifier.FrameRate(frameRate) +
                ArgumentsStringifier.Size(new Size(firstImage.Width, firstImage.Height)) +
                ArgumentsStringifier.StartNumber(0) +
                ArgumentsStringifier.Input($"{firstImage.Directory}\\%09d.png") +
                ArgumentsStringifier.FrameOutputCount(images.Length) +
                ArgumentsStringifier.Video(VideoCodec.LibX264) +
                ArgumentsStringifier.Output(output);

            try
            {
                if (!RunProcess(args, output))
                {
                    throw new FFMpegException(FFMpegExceptionType.Operation, "Could not join the provided image sequence.");
                }

                return new VideoInfo(output);
            }
            finally
            {
                Cleanup(temporaryImageFiles);
            }
        }

        /// <summary>
        ///     Records M3U8 streams to the specified output.
        /// </summary>
        /// <param name="uri">URI to pointing towards stream.</param>
        /// <param name="output">Output file</param>
        /// <returns>Success state.</returns>
        public VideoInfo SaveM3U8Stream(Uri uri, FileInfo output)
        {
            FFMpegHelper.ExtensionExceptionCheck(output, FileExtension.Mp4);

            if (uri.Scheme == "http" || uri.Scheme == "https")
            {
                var args = ArgumentsStringifier.Input(uri) +
                    ArgumentsStringifier.Output(output);

                if (!RunProcess(args, output))
                {
                    throw new FFMpegException(FFMpegExceptionType.Operation, $"Saving the ${uri.AbsoluteUri} stream failed.");
                }

                return new VideoInfo(output);
            }
            throw new ArgumentException($"Uri: {uri.AbsoluteUri}, does not point to a valid http(s) stream.");
        }

        /// <summary>
        ///     Strips a video file of audio.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <returns></returns>
        public VideoInfo Mute(VideoInfo source, FileInfo output)
        {
            FFMpegHelper.ConversionExceptionCheck(source.ToFileInfo(), output);
            FFMpegHelper.ConversionSizeExceptionCheck(source);
            FFMpegHelper.ExtensionExceptionCheck(output, source.Extension);

            var args = ArgumentsStringifier.Input(source) +
                       ArgumentsStringifier.Copy() +
                       ArgumentsStringifier.Disable(Channel.Audio) +
                       ArgumentsStringifier.Output(output);

            if (!RunProcess(args, output))
            {
                throw new FFMpegException(FFMpegExceptionType.Operation, "Could not mute the requested video.");
            }

            return new VideoInfo(output);
        }

        /// <summary>
        ///     Saves audio from a specific video file to disk.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output audio file.</param>
        /// <returns>Success state.</returns>
        public FileInfo ExtractAudio(VideoInfo source, FileInfo output)
        {
            FFMpegHelper.ConversionExceptionCheck(source.ToFileInfo(), output);
            FFMpegHelper.ExtensionExceptionCheck(output, FileExtension.Mp3);

            var args = ArgumentsStringifier.Input(source) +
                       ArgumentsStringifier.Disable(Channel.Video) +
                       ArgumentsStringifier.Output(output);

            if (!RunProcess(args, output))
            {
                throw new FFMpegException(FFMpegExceptionType.Operation, "Could not extract the audio from the requested video.");
            }

            output.Refresh();

            return output;
        }

        /// <summary>
        ///     Adds audio to a video file.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="audio">Source audio file.</param>
        /// <param name="output">Output video file.</param>
        /// <param name="stopAtShortest">Indicates if the encoding should stop at the shortest input file.</param>
        /// <returns>Success state</returns>
        public VideoInfo ReplaceAudio(VideoInfo source, FileInfo audio, FileInfo output, bool stopAtShortest = false)
        {
            FFMpegHelper.ConversionExceptionCheck(source.ToFileInfo(), output);
            FFMpegHelper.InputsExistExceptionCheck(audio);
            FFMpegHelper.ConversionSizeExceptionCheck(source);
            FFMpegHelper.ExtensionExceptionCheck(output, source.Extension);

            var args = ArgumentsStringifier.Input(source) +
                       ArgumentsStringifier.Input(audio) +
                       ArgumentsStringifier.Copy(Channel.Video) +
                       ArgumentsStringifier.Audio(AudioCodec.Aac, AudioQuality.Hd) +
                       ArgumentsStringifier.FinalizeAtShortestInput(stopAtShortest) +
                       ArgumentsStringifier.Output(output);

            if (!RunProcess(args, output))
            {
                throw new FFMpegException(FFMpegExceptionType.Operation, "Could not replace the video audio.");
            }

            return new VideoInfo(output);
        }

        public VideoInfo Convert(ArgumentsContainer arguments)
        {
            var args = argumentBuilder.BuildArguments(arguments);
            var output = ((OutputArgument)arguments[ArgumentsFlag.Output]).GetAsFileInfo();

            if (!RunProcess(args, output))
            {
                throw new FFMpegException(FFMpegExceptionType.Operation, "Could not replace the video audio.");
            }

            return new VideoInfo(output);
        }

        public VideoInfo Convert(ArgumentsContainer arguments, FileInfo input, FileInfo output)
        {
            var args = argumentBuilder.BuildArguments(arguments, input, output);

            if (!RunProcess(args, output))
            {
                throw new FFMpegException(FFMpegExceptionType.Operation, "Could not replace the video audio.");
            }

            return new VideoInfo(output);
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

        private string _ffmpegPath;
        private TimeSpan _totalTime;

        private volatile StringBuilder _errorOutput = new StringBuilder();

        private bool RunProcess(string args, FileInfo output)
        {
            var successState = true;

            CreateProcess(args, _ffmpegPath, true, rStandardError: true);

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
                            throw new FFMpegException(FFMpegExceptionType.Process, _errorOutput);
                        }
                    }
                else
                {
                    throw new FFMpegException(FFMpegExceptionType.Process, _errorOutput);
                }
            }
            return successState;
        }

        private void Cleanup(IEnumerable<string> pathList)
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

            _errorOutput.AppendLine(e.Data);
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