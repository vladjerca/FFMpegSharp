using FFMpegSharp.Tests.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FFMpegSharp.Tests
{
    [TestClass]
    public class VideoTest : BaseTest
    {
        public VideoTest() : base() { }

        public bool Convert(VideoType type)
        {
            var output = Input.OutputLocation(type);

            VideoInfo input = VideoInfo.FromFileInfo(Input);

            try { 
                switch (type)
                {
                    case VideoType.MP4:
                        Encoder.ToMP4(input, output); break;
                    case VideoType.OGV:
                        Encoder.ToOGV(input, output); break;
                    case VideoType.TS:
                        Encoder.ToTS(input, output); break;
                    case VideoType.WebM:
                        Encoder.ToWebM(input, output); break;
                }

                return File.Exists(output.FullName);

            }
            finally
            {
                if (File.Exists(output.FullName))
                    File.Delete(output.FullName);
            }
        }

        [TestMethod]
        public void Convert_ToMP4()
        {
            Assert.IsTrue(Convert(VideoType.MP4));
        }

        [TestMethod]
        public void Convert_ToTS()
        {
            Assert.IsTrue(Convert(VideoType.TS));
        }

        [TestMethod]
        public void Convert_ToWEBM()
        {
            Assert.IsTrue(Convert(VideoType.WebM));
        }

        [TestMethod]
        public void Convert_ToOGV()
        {
            Assert.IsTrue(Convert(VideoType.WebM));
        }

        [TestMethod]
        public void Thumbnail_Extract()
        {
            var output = Input.OutputLocation(ImageType.PNG);
            try
            {
                Encoder.SaveThumbnail(VideoInfo.FromFileInfo(Input), output);

                Assert.IsTrue(File.Exists(output.FullName));
            }
            finally
            {
                if (File.Exists(output.FullName))
                    File.Delete(output.FullName);
            }            
        }
    }
}
