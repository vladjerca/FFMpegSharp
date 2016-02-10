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
                throw new Exception("FFProbe root is not configured in app config. Missing key 'ffmpegRoot'.");

            string path = root + "\\ffprobe.exe";
            if (!File.Exists(path))
                throw new Exception(string.Format("FFProbe cannot be found in the in {0}...", path));
        }
    }
}
