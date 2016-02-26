using FFMpegSharp.FFMPEG.Exceptions;
using System;
using System.IO;

namespace FFMpegSharp.Helpers
{
    public class FFProbeHelper
    {
        public static int GCD(int first, int second)
        {
            while (first != 0 && second != 0)
            {
                if (first > second)
                    first -= second;
                else second -= first;
            }
            return first == 0 ? second : first;
        }

        public static void RootExceptionCheck(string root)
        {
            if (root == null)
                throw new FFMpegException(FFMpegExceptionType.Dependency, "FFProbe root is not configured in app config. Missing key 'ffmpegRoot'.");

            var target = Environment.Is64BitProcess ? "x64" : "x86";
            
            string path = root + string.Format("\\{0}\\ffprobe.exe", target);

            if (!File.Exists(path))
                throw new FFMpegException(FFMpegExceptionType.Dependency, string.Format("FFProbe cannot be found in the in {0}...", path));
        }
    }
}
