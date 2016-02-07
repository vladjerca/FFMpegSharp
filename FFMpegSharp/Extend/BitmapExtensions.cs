using FFMpegSharp.FFMPEG;
using System;
using System.Drawing;
using System.IO;

namespace FFMpegSharp.Extend
{
    public static class BitmapExtensions
    {
        public static VideoInfo AddAudio(this Bitmap poster, FileInfo audio, FileInfo output)
        {
            string destination = string.Format("{0}.png", Environment.TickCount);

            poster.Save(destination);

            var success = new FFMpeg().PosterWithAudio(new FileInfo(destination), audio, output);

            if (!success)
                throw new OperationCanceledException("Could not add audio.");

            return new VideoInfo(output);
        }
    }
}
