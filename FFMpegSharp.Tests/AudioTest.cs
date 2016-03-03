using System.Drawing;
using System.IO;
using FFMpegSharp.Enums;
using FFMpegSharp.Extend;
using FFMpegSharp.Tests.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FFMpegSharp.Tests
{
    [TestClass]
    public class AudioTest : BaseTest
    {
        [TestMethod]
        public void Audio_Remove()
        {
            var output = Input.OutputLocation(VideoType.Mp4);

            try
            {
                VideoInfo.FromFileInfo(Input).Mute(output);

                Assert.IsTrue(File.Exists(output.FullName));
            }
            finally
            {
                if (File.Exists(output.FullName))
                    output.Delete();
            }
        }

        [TestMethod]
        public void Audio_Save()
        {
            var output = Input.OutputLocation(AudioType.Mp3);

            try
            {
                VideoInfo.FromFileInfo(Input).ExtractAudio(output);

                Assert.IsTrue(File.Exists(output.FullName));
            }
            finally
            {
                if (File.Exists(output.FullName))
                    output.Delete();
            }
        }

        [TestMethod]
        public void Audio_Add()
        {
            var output = Input.OutputLocation(VideoType.Mp4);
            try
            {
                var input = VideoInfo.FromFileInfo(VideoLibrary.LocalVideoNoAudio);
                input.ReplaceAudio(VideoLibrary.LocalAudio, output);

                Assert.AreEqual(input.Duration, VideoInfo.FromFileInfo(output).Duration);
                Assert.IsTrue(File.Exists(output.FullName));
            }
            finally
            {
                if (File.Exists(output.FullName))
                    output.Delete();
            }
        }

        [TestMethod]
        public void Image_AddAudio()
        {
            var output = Input.OutputLocation(VideoType.Mp4);

            try
            {
                var result = new Bitmap(VideoLibrary.LocalCover.FullName).AddAudio(VideoLibrary.LocalAudio, output);

                Assert.IsTrue(result.Exists);
            }
            finally
            {
                if (File.Exists(output.FullName))
                    output.Delete();
            }
        }
    }
}