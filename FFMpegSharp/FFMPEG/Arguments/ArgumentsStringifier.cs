using FFMpegSharp.FFMPEG.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public static class ArgumentsStringifier
    {
        public static string Speed(Speed speed)
        {
            return $"-preset {speed.ToString().ToLower()} ";
        }

        public static string Speed(int cpu)
        {
            return $"-quality good -cpu-used {cpu} -deadline realtime ";
        }

        public static string Audio(AudioCodec codec, AudioQuality bitrate)
        {
            return Audio(codec) + Audio(bitrate);
        }

        public static string Audio(AudioCodec codec, int bitrate)
        {
            return Audio(codec) + Audio(bitrate);
        }

        public static string Audio(AudioCodec codec)
        {
            return $"-codec:a {codec.ToString().ToLower()} ";
        }

        public static string Audio(AudioQuality bitrate)
        {
            return Audio((int)bitrate);
        }

        public static string Audio(int bitrate)
        {
            return $"-b:a {bitrate}k -strict experimental ";
        }

        public static string Video(VideoCodec codec, int bitrate = 0)
        {
            var video = $"-codec:v {codec.ToString().ToLower()} -pix_fmt yuv420p ";

            if (bitrate > 0)
            {
                video += $"-b:v {bitrate}k ";
            }

            return video;
        }

        public static string Threads(bool multiThread)
        {
            var threadCount = multiThread
                ? Environment.ProcessorCount
                : 1;

            return Threads(threadCount);
        }

        public static string Threads(int threads)
        {        
            return $"-threads {threads} ";
        }

        public static string Input(Uri uri)
        {
            return Input(uri.AbsolutePath);
        }

        public static string Disable(Channel type)
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

        public static string Input(VideoInfo input)
        {
            return $"-i \"{input.FullName}\" ";
        }

        public static string Input(FileInfo input)
        {
            return $"-i \"{input.FullName}\" ";
        }

        public static string Output(FileInfo output)
        {
            return $"\"{output.FullName}\"";
        }

        public static string Output(string output)
        {
            return $"\"{output}\"";
        }

        public static string Input(string template)
        {
            return $"-i \"{template}\" ";
        }

        public static string Scale(VideoSize size, int width =-1)
        {
            return size == VideoSize.Original ? string.Empty : Scale(width, (int)size);
        }

        public static string Scale(int width, int height)
        {
            return $"-vf scale={width}:{height} ";
        }

        public static string Scale(Size size)
        {
            return Scale(size.Width, size.Height);
        }

        public static string Size(Size? size)
        {
            if (!size.HasValue) return string.Empty;

            var formatedSize = $"{size.Value.Width}x{size.Value.Height}";

            return $"-s {formatedSize} ";
        }

        public static string ForceFormat(VideoCodec codec)
        {
            return $"-f {codec.ToString().ToLower()} ";
        }

        public static string BitStreamFilter(Channel type, Filter filter)
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

        public static string Copy(Channel type = Channel.Both)
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

        public static string Seek(TimeSpan? seek)
        {
            return !seek.HasValue ? string.Empty : $"-ss {seek} ";
        }

        public static string FrameOutputCount(int number)
        {
            return $"-vframes {number} ";
        }

        public static string Loop(int count)
        {
            return $"-loop {count} ";
        }

        public static string FinalizeAtShortestInput(bool applicable)
        {
            return applicable ? "-shortest " : string.Empty;
        }

        public static string InputConcat(IEnumerable<string> paths)
        {
            return $"-i \"concat:{string.Join(@"|", paths)}\" ";
        }

        public static string FrameRate(double frameRate)
        {
            return $"-r {frameRate} ";
        }

        public static string StartNumber(int v)
        {
            return $"-start_number {v} ";
        }
    }
}