using System;
using System.IO;
using FFMpegSharp.FFMPEG.Atomic;
using FFMpegSharp.FFMPEG.Enums;
using FFMpegSharp.FFMPEG.Exceptions;

namespace FFMpegSharp.Helpers
{
    public static class FfMpegHelper
    {
        public static string GetScale(VideoSize size)
        {
            return Arguments.Scale(size);
        }

        public static void ConversionExceptionCheck(VideoInfo originalVideo, FileInfo convertedPath)
        {
            if (File.Exists(convertedPath.FullName))
                throw new FFMpegException(FFMpegExceptionType.File,
                    $"The output file: {convertedPath} already exists!");

            if (!File.Exists(originalVideo.FullName))
                throw new FFMpegException(FFMpegExceptionType.File,
                    $"Input {originalVideo.FullName} does not exist!");
        }

        public static void InputFilesExistExceptionCheck(params FileInfo[] paths)
        {
            foreach (var path in paths)
                if (!File.Exists(path.FullName))
                    throw new FFMpegException(FFMpegExceptionType.File, $"Input {path} does not exist!");
        }

        public static void ExtensionExceptionCheck(FileInfo output, string expected)
        {
            if (!expected.Equals(new FileInfo(output.FullName).Extension, StringComparison.OrdinalIgnoreCase))
                throw new FFMpegException(FFMpegExceptionType.File,
                    $"Invalid output file. File extension should be '{expected}' required.");
        }

        public static void RootExceptionCheck(string root)
        {
            if (root == null)
                throw new FFMpegException(FFMpegExceptionType.Dependency,
                    "FFMpeg root is not configured in app config. Missing key 'ffmpegRoot'.");

            var target = Environment.Is64BitProcess ? "x64" : "x86";

            var path = root + $"\\{target}\\ffmpeg.exe";

            if (!File.Exists(path))
                throw new FFMpegException(FFMpegExceptionType.Dependency,
                    "FFMpeg cannot be found in the root directory!");
        }

        public static class Extensions
        {
            public static readonly string Mp4 = ".mp4";
            public static readonly string Mp3 = ".mp3";
            public static readonly string Ts = ".ts";
            public static readonly string WebM = ".webm";
            public static readonly string Ogv = ".ogv";
            public static readonly string Png = ".png";
        }
    }
}