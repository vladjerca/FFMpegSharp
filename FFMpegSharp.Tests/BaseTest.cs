using FFMpegSharp.FFMPEG;
using FFMpegSharp.Tests.Resources;
using System.IO;

namespace FFMpegSharp.Tests
{
    public class BaseTest
    {
        protected FFMpeg Encoder;
        protected FileInfo Input;

        public BaseTest()
        {
            Encoder = new FFMpeg();
            Input = new FileInfo(VideoLibrary.LocalVideo);
        }
    }
}
