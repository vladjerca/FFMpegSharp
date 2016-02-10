using FFMpegSharp.FFMPEG.Enums;
using System;
using System.Drawing;
using System.IO;

namespace FFMpegSharp.FFMPEG.Atomic
{
    internal static class Arguments
    {
        internal static string Speed(Speed speed)
        {
            return string.Format("-preset {0} ", speed.ToString().ToLower());
        }

        internal static string Speed(int cpu)
        {
            return string.Format("-quality good -cpu-used {0} -deadline realtime ", cpu);
        }

        internal static string Audio(AudioCodec codec, AudioQuality bitrate)
        {
            return string.Format("-codec:a {0} -b:a {1}k -strict experimental ", codec.ToString().ToLower(), (int)bitrate);
        }

        internal static string Video(VideoCodec codec, int bitrate = 0)
        {
            string video = string.Format("-codec:v {0} ", codec.ToString().ToLower());

            if(bitrate > 0)
                video += string.Format("-b:v {0}k ", bitrate);

            return video;
        }

        internal static string Threads(bool multiThread)
        {
            string threadCount = multiThread ?
                Environment.ProcessorCount.ToString() : "1";

            return string.Format("-threads {0} ", threadCount);
        }

        internal static string Input(Uri uri)
        {
            return string.Format("-i \"{0}\" ", uri.AbsoluteUri);
        }

        internal static string Disable(Channel type)
        {
            switch (type)
            {
                case Channel.Video: return "-vn ";
                case Channel.Audio: return "-an ";
                default: return string.Empty;
            }
        }

        internal static string Input(VideoInfo input)
        {
            return string.Format("-i \"{0}\" ", input.FullPath);
        }

        internal static string Input(FileInfo input)
        {
            return string.Format("-i \"{0}\" ", input.FullName);
        }

        internal static string Output(FileInfo output)
        {
            return string.Format("\"{0}\"", output);
        }

        internal static string Scale(VideoSize size)
        {
            if (size == VideoSize.Original)
                return string.Empty;

            return string.Format("-vf scale={0} ", (int)size);
        }

        internal static string Size(Size? size)
        {
            string formatedSize = string.Format("{0}x{1}", size.Value.Width, size.Value.Height);

            return string.Format("-s {0} ", formatedSize);
        }

        internal static string ForceFormat(VideoCodec codec)
        {
            return string.Format("-f {0} ", codec.ToString().ToLower());
        }
        
        internal static string BitStreamFilter(Filter filter)
        {
            return string.Format("-bsf:v {0} ", filter.ToString().ToLower());
        }

        internal static string Copy(Channel type = Channel.Both)
        {
            switch (type)
            {
                case Channel.Audio: return "-c:a copy ";
                case Channel.Video: return "-c:v copy ";
                default: return "-c copy ";
            }
        }

        internal static string Seek(TimeSpan? seek)
        {
            return string.Format("-ss {0} ", seek.Value);
        }

        internal static string FrameOutputCount(int number)
        {
            return string.Format("-vframes {0} ", number);
        }

        public static string Loop(int count)
        {
            return string.Format("-loop {0} ", count);
        }

        public static string FinalizeAtShortestInput()
        {
            return "-shortest ";
        }

        public static string InputConcat(string[] paths)
        {
            return string.Format("-i \"concat:{0}\" ", string.Join(@"|", (object[])paths));
        }
    }
}
