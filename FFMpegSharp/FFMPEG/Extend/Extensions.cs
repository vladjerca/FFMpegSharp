using System.Collections.Generic;
using System.IO;
using System.Linq;
using FFMpegSharp.FFMPEG.Enums;

namespace FFMpegSharp.FFMPEG.Extend
{
    public static class Extensions
    {
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
        public static bool ToMp4(this FFMpeg encoder, string source, string output, Speed speed = Speed.SuperFast,
            VideoSize size = VideoSize.Original, AudioQuality aQuality = AudioQuality.Normal, bool multithread = false)
        {
            return encoder.ToMp4(VideoInfo.FromPath(source), new FileInfo(output), speed, size, aQuality, multithread);
        }

        /// <summary>
        ///     Converts a source video to WebM format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <param name="size">Output video size.</param>
        /// <param name="aQuality">Output audio quality.</param>
        /// <returns>Success state.</returns>
        public static bool ToWebM(this FFMpeg encoder, string source, string output, VideoSize size = VideoSize.Original,
            AudioQuality aQuality = AudioQuality.Normal)
        {
            return encoder.ToWebM(VideoInfo.FromPath(source), new FileInfo(output), size, aQuality);
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
        public static bool ToOgv(this FFMpeg encoder, string source, string output, VideoSize size = VideoSize.Original,
            AudioQuality aQuality = AudioQuality.Normal, bool multithread = false)
        {
            return encoder.ToOgv(VideoInfo.FromPath(source), new FileInfo(output), size, aQuality, multithread);
        }

        /// <summary>
        ///     Converts a source video to TS format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <returns>Success state.</returns>
        public static bool ToTs(this FFMpeg encoder, string source, string output)
        {
            return encoder.ToTs(VideoInfo.FromPath(source), new FileInfo(output));
        }

        /// <summary>
        ///     Strips a video file of audio.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <returns></returns>
        public static bool Mute(this FFMpeg encoder, string source, string output)
        {
            return encoder.Mute(VideoInfo.FromPath(source), new FileInfo(output));
        }

        /// <summary>
        ///     Saves audio from a specific video file to disk.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output audio file.</param>
        /// <returns>Success state.</returns>
        public static bool ExtractAudio(this FFMpeg encoder, string source, string output)
        {
            return encoder.ExtractAudio(VideoInfo.FromPath(source), new FileInfo(output));
        }

        /// <summary>
        ///     Adds audio to a video file.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="audio">Source audio file.</param>
        /// <param name="output">Output video file.</param>
        /// <param name="stopAtShortest">Indicates if the encoding should stop at the shortest input file.</param>
        /// <returns>Success state</returns>
        public static bool ReplaceAudio(this FFMpeg encoder, string source, string audio, string output,
            bool stopAtShortest = false)
        {
            return encoder.ReplaceAudio(VideoInfo.FromPath(source), new FileInfo(audio), new FileInfo(output));
        }

        /// <summary>
        ///     Joins a list of video files.
        /// </summary>
        /// <param name="output">Output video file.</param>
        /// <param name="videos">List of vides that need to be joined together.</param>
        /// <returns>Success state.</returns>
        public static bool Join(this FFMpeg encoder, string output, params string[] videos)
        {
            if (videos.Length > 0)
            {
                var infoList = new VideoInfo[videos.Length];
                for (var i = 0; i < videos.Length; i++)
                {
                    var vidInfo = new VideoInfo(videos[i]);
                    infoList[i] = vidInfo;
                }
                return encoder.Join(new FileInfo(output), infoList);
            }
            return false;
        }

        /// <summary>
        ///     Joins a list of video files.
        /// </summary>
        /// <param name="output">Output video file.</param>
        /// <param name="videos">List of vides that need to be joined together.</param>
        /// <returns>Success state.</returns>
        public static bool Join(this FFMpeg encoder, string output, IEnumerable<string> videos)
        {
            return encoder.Join(output, videos.ToArray());
        }
    }
}