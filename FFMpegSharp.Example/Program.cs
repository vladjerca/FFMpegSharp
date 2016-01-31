using FFMpegSharp.FFMPEG;
using System;
using System.Configuration;

namespace FFMpegSharp.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            string  inputFile = "<INPUT_LOCATION>",
                    outputFile = "<OUTPUT_DIR>\\<OUTPUT_FILENAME>.<OUTPUT_EXTENSION>";

            FFMpeg encoder = new FFMpeg();

            // Bind Progress Handler
            encoder.OnProgress += encoder_OnProgress;

            // Start Encoding
            encoder.ToMP4(inputFile, outputFile);
        }

        static void encoder_OnProgress(int percentage)
        {
            Console.WriteLine("Progress {0}%", percentage);
        }
    }
}
