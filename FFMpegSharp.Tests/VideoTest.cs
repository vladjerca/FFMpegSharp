using FFMpegSharp.Enums;
using FFMpegSharp.FFMPEG.Enums;
using FFMpegSharp.Tests.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FFMpegSharp.Tests
{
    [TestClass]
    public class VideoTest : BaseTest
    {
        public VideoTest() : base() { }

        public bool Convert(VideoType type, bool multithread = false)
        {
            var output = Input.OutputLocation(type);

            try
            {              
                VideoInfo input = VideoInfo.FromFileInfo(Input);

                input.ConvertTo(type, output, Speed.SuperFast, VideoSize.Original, AudioQuality.Ultra, multithread);

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
        public void Video_ToMP4_MultiThread()
        {
            Assert.IsTrue(Convert(VideoType.MP4, true));
        }

        [TestMethod]
        public void Video_ToTS_MultiThread()
        {
            Assert.IsTrue(Convert(VideoType.TS, true));
        }

        [TestMethod]
        public void Video_ToOGV_MultiThread()
        {
            Assert.IsTrue(Convert(VideoType.OGV, true));
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

        [TestMethod]
        public void Video_Join()
        {
            var output = Input.OutputLocation(VideoType.MP4);
            var newInput = Input.OutputLocation(VideoType.MP4, "duplicate");
            try
            {
                VideoInfo input = VideoInfo.FromFileInfo(Input);                
                File.Copy(input.FullName, newInput.FullName);
                VideoInfo input2 = VideoInfo.FromFileInfo(newInput);

                input.JoinWith(output, false, input2);

                var duration1 = new VideoInfo(output.FullName).Duration;
                var duration2 = input.Duration;

                var outputVideo = new VideoInfo(output.FullName);

                Assert.IsTrue(File.Exists(output.FullName) && (outputVideo.Duration - input.Duration == input.Duration || (outputVideo.Width == input.Width && outputVideo.Height == input.Height)));
            }
            finally
            {
                if (File.Exists(output.FullName))
                    File.Delete(output.FullName);

                if (File.Exists(newInput.FullName))
                    File.Delete(newInput.FullName);
            }
        }
    }
}
