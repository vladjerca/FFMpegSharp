using System;
using System.IO;
using FFMpegSharp.FFMPEG;

namespace FFMpegSharp.Extend
{
    public static class UriExtensions
    {
        public static VideoInfo SaveStream(this Uri uri, FileInfo output)
        {
            var success = new FFMpeg().SaveM3U8Stream(uri, output);

            if (!success)
                throw new OperationCanceledException("Could not save stream.");

            return new VideoInfo(output);
        }
    }
}