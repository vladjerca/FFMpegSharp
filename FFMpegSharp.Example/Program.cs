using System;
using System.IO;
using System.Linq;
using FFMpegSharp.FFMPEG;

namespace FFMpegSharp.Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var encoder = new FFMpeg();

            // Bind Progress Handler
            encoder.OnProgress += percentage => { Console.WriteLine("Progress {0}%", percentage); };

            foreach (var input in args.Select(fileLocation => new VideoInfo(fileLocation)))
            {
                // Start Encoding
                encoder.ToMp4(input, new FileInfo(input.FullName.Replace(input.Extension, ".mp4")));
            }
        }
    }
}