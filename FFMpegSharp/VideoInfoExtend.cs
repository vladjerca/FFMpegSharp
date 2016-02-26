using FFMpegSharp.Enums;
using FFMpegSharp.FFMPEG.Enums;
using System.IO;

namespace FFMpegSharp
{
    public partial class VideoInfo
    {
        /// <summary>
        /// Convert video file with implicit output. Output will be in the parent directory of the original video file.
        /// </summary>
        /// <param name="type">Output format.</param>
        /// <param name="speed">MP4 encoding speed (applies only to mp4 format). Faster results in lower quality.</param>
        /// <param name="size">Aspect ratio of the output video file.</param>
        /// <param name="audio">Audio quality of the output video file.</param>
        /// <param name="multithread">Tell FFMpeg to use multithread in the conversion process.</param>
        /// <param name="tryToPurge">Flag original file purging after conversion is done (Will not result in exception if file is readonly or missing.).</param>
        /// <returns>Video information object with the new video file.</returns>
        public VideoInfo ConvertTo(VideoType type, Speed speed = Speed.SuperFast, VideoSize size = VideoSize.Original, AudioQuality audio = AudioQuality.Normal, bool multithread = false, bool deleteOriginal = false)
        {
            string outputPath = this.FullName.Replace(this.Extension, string.Format(".{0}", type.ToString().ToLower()));
            FileInfo output = new FileInfo(outputPath);
            return this.ConvertTo(type, output, speed, size, audio, multithread, deleteOriginal);
        }
    }
}
