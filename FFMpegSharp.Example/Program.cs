using FFMpegSharp.FFMPEG;
using System;
using System.IO;

namespace FFMpegSharp.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            FFMpeg encoder = new FFMpeg();

            // Bind Progress Handler
            encoder.OnProgress += (double percentage) => {
                Console.WriteLine("Progress {0}%", percentage);
            };
            
            foreach(var fileLocation in args)
            {
                var input = new VideoInfo(fileLocation);
                // Start Encoding
                encoder.ToMP4(input, new FileInfo(input.FullPath.Replace(input.Extension, ".mp4")));
            }
        }
    }
}
