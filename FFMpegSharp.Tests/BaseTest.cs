using System.IO;
using FFMpegSharp.FFMPEG;
using FFMpegSharp.Tests.Resources;

namespace FFMpegSharp.Tests
{
    public class BaseTest
    {
        protected FFMpeg Encoder;
        protected FileInfo Input;

        public BaseTest()
        {
            Encoder = new FFMpeg();
            Input = VideoLibrary.LocalVideo;
        }
    }
}