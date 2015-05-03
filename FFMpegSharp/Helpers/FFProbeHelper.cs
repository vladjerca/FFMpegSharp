using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if (!File.Exists(root + "\\ffprobe.exe"))
                throw new Exception("FFProbe cannot be found in the root directory!");
        }
    }
}
