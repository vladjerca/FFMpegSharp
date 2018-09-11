using FFMpegSharp.Enums;
using FFMpegSharp.FFMPEG;
using FFMpegSharp.FFMPEG.Enums;
using FFMpegSharp.Tests.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace FFMpegSharp.Tests
{
    [TestClass]
    public class VideoTest : BaseTest
    {
        public bool Convert(VideoType type, bool multithreaded = false, VideoSize size = VideoSize.Original)
        {
            var output = Input.OutputLocation(type);

            try
            {
                var input = VideoInfo.FromFileInfo(Input);

                Encoder.Convert(input, output, type, size: size, multithreaded: multithreaded);

                var outputVideo = new VideoInfo(output.FullName);

                return File.Exists(output.FullName) &&
                       outputVideo.Duration == input.Duration &&
                       (
                           (
                           size == VideoSize.Original &&
                           outputVideo.Width == input.Width &&
                           outputVideo.Height == input.Height
                           ) ||
                           (
                           size != VideoSize.Original &&
                           outputVideo.Width != input.Width &&
                           outputVideo.Height != input.Height &&
                           outputVideo.Width == (int)size
                           )
                       );
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
        public void Video_ToOGV_Resize()
        {
            Assert.IsTrue(Convert(VideoType.Ogv, true, VideoSize.Ed));
        }

        [TestMethod]
        public void Video_ToMP4_Resize()
        {
            Assert.IsTrue(Convert(VideoType.Mp4, true, VideoSize.Ed));
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

                using (var bitmap = Encoder.Snapshot(input, output))
                {
                    Assert.AreEqual(input.Width, bitmap.Width);
                    Assert.AreEqual(input.Height, bitmap.Height);
                    Assert.AreEqual(bitmap.RawFormat, ImageFormat.Png);
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

                var result = Encoder.Join(output, input, input2);
                
                Assert.IsTrue(File.Exists(output.FullName));
                Assert.AreEqual(input.Duration.TotalSeconds * 2, result.Duration.TotalSeconds);
                Assert.AreEqual(input.Height, result.Height);
                Assert.AreEqual(input.Width, result.Width);
            }
            finally
            {
                if (File.Exists(output.FullName))
                    File.Delete(output.FullName);

                if (File.Exists(newInput.FullName))
                    File.Delete(newInput.FullName);
            }
        }

        [TestMethod]
        public void Video_Join_Image_Sequence()
        {
            try
            {
                var imageSet = new List<ImageInfo>();
                Directory.EnumerateFiles(VideoLibrary.ImageDirectory.FullName)
                    .Where(file => file.ToLower().EndsWith(".png"))
                    .ToList()
                    .ForEach(file =>
                    {
                        for (int i = 0; i < 15; i++)
                        {
                            imageSet.Add(new ImageInfo(file));
                        }
                    });

                var result = Encoder.JoinImageSequence(VideoLibrary.ImageJoinOutput, images: imageSet.ToArray());

                VideoLibrary.ImageJoinOutput.Refresh();

                Assert.IsTrue(VideoLibrary.ImageJoinOutput.Exists);
                Assert.AreEqual(3, result.Duration.Seconds);
                Assert.AreEqual(imageSet.First().Width, result.Width);
                Assert.AreEqual(imageSet.First().Height, result.Height);
            }
            finally
            {
                VideoLibrary.ImageJoinOutput.Refresh();
                if (VideoLibrary.ImageJoinOutput.Exists)
                {
                    VideoLibrary.ImageJoinOutput.Delete();
                }
            }
        }
    }
}