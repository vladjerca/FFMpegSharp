using FFMpegSharp.Enums;
using FFMpegSharp.FFMPEG.Enums;
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

            try
            {              
                VideoInfo input = VideoInfo.FromFileInfo(Input);

                input.ConvertTo(type, output, Speed.SuperFast, VideoSize.Original, AudioQuality.Ultra, true);

                var duration1 = new VideoInfo(output.FullName).Duration;
                var duration2 = input.Duration;

                var outputVideo = new VideoInfo(output.FullName);

                return File.Exists(output.FullName) && (outputVideo.Duration == input.Duration || (outputVideo.Width == input.Width && outputVideo.Height == input.Height) );
            }
            finally
            {
                if (File.Exists(output.FullName))
                    File.Delete(output.FullName);
            }
        }

        [TestMethod]
        public void Video_ToMP4()
        {
            Assert.IsTrue(Convert(VideoType.MP4));
        }

        [TestMethod]
        public void Video_ToTS()
        {
            Assert.IsTrue(Convert(VideoType.TS));
        }

        [TestMethod]
        public void Video_ToWEBM()
        {
            Assert.IsTrue(Convert(VideoType.WebM));
        }

        [TestMethod]
        public void Video_ToOGV()
        {
            Assert.IsTrue(Convert(VideoType.OGV));
        }

        [TestMethod]
        public void Video_Snapshot()
        {
            var output = Input.OutputLocation(ImageType.PNG);

            try
            {
                var input = VideoInfo.FromFileInfo(Input);

                using (var result = input.Snapshot(output))
                {
                    Assert.IsTrue(File.Exists(output.FullName));
                }
            }
            finally
            {
                if (File.Exists(output.FullName))
                    File.Delete(output.FullName);
            }            
        }
    }
}
