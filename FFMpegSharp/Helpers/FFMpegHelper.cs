using FFMpegSharp.FFMPEG.Enums;
using System;
using System.IO;

namespace FFMpegSharp.Helpers
{
    public static class FFMpegHelper
    {
        public static string GetScale(VideoSize size)
        {
            string scale = "-vf scale=";

            switch (size)
            {
                case VideoSize.FullHD: scale += "-1:1080"; break;
                case VideoSize.HD: scale += "-1:720"; break;
                case VideoSize.ED: scale += "-1:480"; break;
                case VideoSize.LD: scale += "-1:360"; break;
                default: scale = ""; break;
            }

            return scale;
        }

        public static void ConversionExceptionCheck(VideoInfo originalVideo, FileInfo convertedPath)
        {
            if (File.Exists(convertedPath.FullName))
                throw new Exception(string.Format("The output file: {1} already exists!", convertedPath));

            if (!File.Exists(originalVideo.Path))
                throw new Exception(string.Format("Input {0} does not exist!", originalVideo.Path));
        }

        public static void InputFilesExistExceptionCheck(params FileInfo[] paths)
        {
            foreach(FileInfo path in paths)
                if (!File.Exists(path.FullName))
                    throw new Exception(string.Format("Input {0} does not exist!", path));
        }

        public static void ExtensionExceptionCheck(FileInfo output, string expected)
        {
            if (!expected.Equals(new FileInfo(output.FullName).Extension, StringComparison.OrdinalIgnoreCase))
                throw new Exception(string.Format("Invalid output file. File extension should be '{0}' required.", expected));
        }

        public static void RootExceptionCheck(string root)
        {
            if (root == null)
                throw new Exception("FFMpeg root is not configured in app config. Missing key 'ffmpegRoot'.");

            if (!File.Exists(root + "\\ffmpeg.exe"))
                throw new Exception("FFMpeg cannot be found in the root directory!");
        }
    }
}
