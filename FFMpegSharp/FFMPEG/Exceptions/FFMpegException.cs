using System;

namespace FFMpegSharp.FFMPEG.Exceptions
{
    public enum FFMpegExceptionType
    {
        Dependency,
        Conversion,
        File
    }

    public class FFMpegException : Exception
    {
        public FFMpegException(FFMpegExceptionType type)
        {
            Type = type;
        }

        public FFMpegException(FFMpegExceptionType type, string message) : base(message)
        {
            Type = type;
        }

        public FFMpegException(FFMpegExceptionType type, string message, FFMpegException innerException)
            : base(message, innerException)
        {
            Type = type;
        }

        public FFMpegExceptionType Type { get; set; }
    }
}