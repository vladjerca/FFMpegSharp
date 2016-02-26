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
        public FFMpegExceptionType Type { get; set; }

        public FFMpegException(FFMpegExceptionType type) : base() { Type = type; }

        public FFMpegException(FFMpegExceptionType type, string message) : base(message) { Type = type; }

        public FFMpegException(FFMpegExceptionType type,  string message, FFMpegException innerException) : base(message, innerException) { Type = type; }
    }
}
