using FFMpegSharp.FFMPEG.Enums;
using FFMpegSharp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Extend
{
    public static class Extensions
    {
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
        public static bool ToMP4(this FFMpeg encoder, string source, string output, Speed speed = Speed.SuperFast, VideoSize size = VideoSize.Original, AudioQuality aQuality = AudioQuality.Normal, bool multithread = false)
        {
            return encoder.ToMP4(VideoInfo.FromPath(source), new FileInfo(output), speed, size, aQuality, multithread);
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
        public static bool ToWebM(this FFMpeg encoder, string source, string output, VideoSize size = VideoSize.Original, AudioQuality aQuality = AudioQuality.Normal, bool multithread = false)
        {
            return encoder.ToWebM(VideoInfo.FromPath(source), new FileInfo(output), size, aQuality, multithread);
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
        public static bool ToOGV(this FFMpeg encoder, string source, string output, VideoSize size = VideoSize.Original, AudioQuality aQuality = AudioQuality.Normal, bool multithread = false)
        {
            return encoder.ToOGV(VideoInfo.FromPath(source), new FileInfo(output), size, aQuality, multithread);
        }

        /// <summary>
        /// Converts a source video to TS format.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <returns>Success state.</returns>
        public static bool ToTS(this FFMpeg encoder, string source, string output)
        {
            return encoder.ToTS(VideoInfo.FromPath(source), new FileInfo(output));
        }


        /// <summary>
        /// Strips a video file of audio.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output video file.</param>
        /// <returns></returns>
        public static bool Mute(this FFMpeg encoder, string source, string output)
        {
            return encoder.Mute(VideoInfo.FromPath(source), new FileInfo(output));
        }

        /// <summary>
        /// Saves audio from a specific video file to disk.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="output">Output audio file.</param>
        /// <returns>Success state.</returns>
        public static bool SaveAudio(this FFMpeg encoder, string source, string output)
        {
            return encoder.SaveAudio(VideoInfo.FromPath(source), new FileInfo(output));
        }

        /// <summary>
        /// Adds audio to a video file.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <param name="audio">Source audio file.</param>
        /// <param name="output">Output video file.</param>
        /// <param name="stopAtShortest">Indicates if the encoding should stop at the shortest input file.</param>
        /// <returns>Success state</returns>
        public static bool AddAudio(this FFMpeg encoder, string source, string audio, string output, bool stopAtShortest = false)
        {
            return encoder.AddAudio(VideoInfo.FromPath(source), new FileInfo(audio), new FileInfo(output));
        }

        /// <summary>
        /// Joins a list of video files.
        /// </summary>
        /// <param name="output">Output video file.</param>
        /// <param name="videos">List of vides that need to be joined together.</param>
        /// <returns>Success state.</returns>
        public static bool Join(string output, IEnumerable<string> videos)
        {
            return Join(output, videos.ToArray());
        }

        /// <summary>
        /// Joins a list of video files.
        /// </summary>
        /// <param name="output">Output video file.</param>
        /// <param name="videos">List of vides that need to be joined together.</param>
        /// <returns>Success state.</returns>
        public static bool Join(this FFMpeg encoder, string output, params string[] videos)
        {
            if (videos.Length > 0)
            {
                VideoInfo[] infoList = new VideoInfo[videos.Length];
                for (int i = 0; i < videos.Length; i++)
                {
                    var vidInfo = new VideoInfo(videos[i]);
                    infoList[i] = vidInfo;
                }
                return encoder.Join(output, infoList);
            }
            return false;
        }

        /// <summary>
        /// Joins a list of video files.
        /// </summary>
        /// <param name="output">Output video file.</param>
        /// <param name="videos">List of vides that need to be joined together.</param>
        /// <returns>Success state.</returns>
        public static bool Join(this FFMpeg encoder, string output, IEnumerable<VideoInfo> videos)
        {
            return encoder.Join(output, videos.ToArray());
        }
    }
}
