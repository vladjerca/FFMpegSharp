using System.IO;
using FFMpegSharp.Enums;
using FFMpegSharp.FFMPEG.Enums;
using FFMpegSharp.Tests.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FFMpegSharp.Tests
{
    [TestClass]
    public class VideoTest : BaseTest
    {
        public bool Convert(VideoType type, bool multithread = false)
        {
            var output = Input.OutputLocation(type);

            try
            {
                var input = VideoInfo.FromFileInfo(Input);

                input.ConvertTo(type, output, Speed.SuperFast, VideoSize.Original, AudioQuality.Ultra, multithread);

                var outputVideo = new VideoInfo(output.FullName);

                return File.Exists(output.FullName) &&
                       (outputVideo.Duration == input.Duration ||
                        (outputVideo.Width == input.Width && outputVideo.Height == input.Height));
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
            Assert.IsTrue(Convert(VideoType.Mp4));
        }

        [TestMethod]
        public void Video_ToTS()
        {
            Assert.IsTrue(Convert(VideoType.Ts));
        }

        [TestMethod]
        public void Video_ToWEBM()
        {
            Assert.IsTrue(Convert(VideoType.WebM));
        }

        [TestMethod]
        public void Video_ToOGV()
        {
            Assert.IsTrue(Convert(VideoType.Ogv));
        }

        [TestMethod]
        public void Video_ToMP4_MultiThread()
        {
            Assert.IsTrue(Convert(VideoType.Mp4, true));
        }

        [TestMethod]
        public void Video_ToTS_MultiThread()
        {
            Assert.IsTrue(Convert(VideoType.Ts, true));
        }

        [TestMethod]
        public void Video_ToOGV_MultiThread()
        {
            Assert.IsTrue(Convert(VideoType.Ogv, true));
        }

        [TestMethod]
        public void Video_Snapshot()
        {
            var output = Input.OutputLocation(ImageType.Png);

            try
            {
                var input = VideoInfo.FromFileInfo(Input);

                using (input.Snapshot(output))
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
            var output = Input.OutputLocation(VideoType.Mp4);
            var newInput = Input.OutputLocation(VideoType.Mp4, "duplicate");
            try
            {
                var input = VideoInfo.FromFileInfo(Input);
                File.Copy(input.FullName, newInput.FullName);
                var input2 = VideoInfo.FromFileInfo(newInput);

                input.JoinWith(output, false, input2);

                var outputVideo = new VideoInfo(output.FullName);

                Assert.IsTrue(File.Exists(output.FullName) &&
                              (outputVideo.Duration - input.Duration == input.Duration ||
                               (outputVideo.Width == input.Width && outputVideo.Height == input.Height)));
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