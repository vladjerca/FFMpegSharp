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
            string output = Input.OutputLocation(type);

            try { 
                
                if (File.Exists(output))
                    File.Delete(output);

                switch (type)
                {
                    case VideoType.MP4:
                        Encoder.ToMP4(Input.FullName, output); break;
                    case VideoType.OGV:
                        Encoder.ToOGV(Input.FullName, output); break;
                    case VideoType.TS:
                        Encoder.ToTS(Input.FullName, output); break;
                    case VideoType.WebM:
                        Encoder.ToWebM(Input.FullName, output); break;
                }

                return File.Exists(output);

            }
            finally
            {
                if (File.Exists(output))
                    File.Delete(output);
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
            string output = Input.OutputLocation(ImageType.PNG);
            try
            {
                Encoder.SaveThumbnail(Input.FullName, output);

                Assert.IsTrue(File.Exists(output));
            }
            finally
            {
                if (File.Exists(output))
                    File.Delete(output);
            }            
        }
    }
}
