using System;
using System.Drawing;
using System.IO;
using FFMpegSharp.FFMPEG.Enums;

namespace FFMpegSharp.FFMPEG.Atomic
{
    internal static class Arguments
    {
        internal static string Speed(Speed speed)
        {
            return $"-preset {speed.ToString().ToLower()} ";
        }

        internal static string Speed(int cpu)
        {
            return $"-quality good -cpu-used {cpu} -deadline realtime ";
        }

        internal static string Audio(AudioCodec codec, AudioQuality bitrate)
        {
            return $"-codec:a {codec.ToString().ToLower()} -b:a {(int) bitrate}k -strict experimental ";
        }

        internal static string Video(VideoCodec codec, int bitrate = 0)
        {
            var video = $"-codec:v {codec.ToString().ToLower()} ";

            if (bitrate > 0)
                video += $"-b:v {bitrate}k ";

            return video;
        }

        internal static string Threads(bool multiThread)
        {
            var threadCount = multiThread
                ? Environment.ProcessorCount.ToString()
                : "1";

            return $"-threads {threadCount} ";
        }

        internal static string Input(Uri uri)
        {
            return $"-i \"{uri.AbsoluteUri}\" ";
        }

        internal static string Disable(Channel type)
        {
            switch (type)
            {
                case Channel.Video:
                    return "-vn ";
                case Channel.Audio:
                    return "-an ";
                default:
                    return string.Empty;
            }
        }

        internal static string Input(VideoInfo input)
        {
            return $"-i \"{input.FullName}\" ";
        }

        internal static string Input(FileInfo input)
        {
            return $"-i \"{input.FullName}\" ";
        }

        internal static string Output(FileInfo output)
        {
            return $"\"{output}\"";
        }

        internal static string Scale(VideoSize size)
        {
            return size == VideoSize.Original ? string.Empty : $"-vf scale={(int) size} ";
        }

        internal static string Size(Size? size)
        {
            if (!size.HasValue) return string.Empty;

            var formatedSize = $"{size.Value.Width}x{size.Value.Height}";

            return $"-s {formatedSize} ";
        }

        internal static string ForceFormat(VideoCodec codec)
        {
            return $"-f {codec.ToString().ToLower()} ";
        }

        internal static string BitStreamFilter(Channel type, Filter filter)
        {
            switch (type)
            {
                case Channel.Audio:
                    return $"-bsf:a {filter.ToString().ToLower()} ";
                case Channel.Video:
                    return $"-bsf:v {filter.ToString().ToLower()} ";
                default:
                    return string.Empty;
            }
        }

        internal static string Copy(Channel type = Channel.Both)
        {
            switch (type)
            {
                case Channel.Audio:
                    return "-c:a copy ";
                case Channel.Video:
                    return "-c:v copy ";
                default:
                    return "-c copy ";
            }
        }

        internal static string Seek(TimeSpan? seek)
        {
            return !seek.HasValue ? string.Empty : $"-ss {seek} ";
        }

        internal static string FrameOutputCount(int number)
        {
            return $"-vframes {number} ";
        }

        public static string Loop(int count)
        {
            return $"-loop {count} ";
        }

        public static string FinalizeAtShortestInput()
        {
            return "-shortest ";
        }

        public static string InputConcat(string[] paths)
        {
            return $"-i \"concat:{string.Join(@"|", paths)}\" ";
        }
    }
}