using FFMpegSharp.Tests.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FFMpegSharp.Tests
{
    [TestClass]
    public class AudioTest : BaseTest
    {
        [TestMethod]
        public void Audio_Remove()
        {
            string output = Input.OutputLocation(VideoType.MP4);

            try
            {
                Encoder.Mute(Input.FullName, output);
                Assert.IsTrue(File.Exists(output));
            }
            finally
            {
                if (File.Exists(output))
                    File.Delete(output);
            }
        }

        [TestMethod]
        public void Audio_Save()
        {
            string output = Input.OutputLocation(AudioType.MP3);
            if (File.Exists(output))
                File.Delete(output);

            Encoder.SaveAudio(Input.FullName, output);

            Assert.IsTrue(File.Exists(output));
        }

        [TestMethod]
        public void Audio_Add()
        {
            string output = Input.OutputLocation(VideoType.MP4);
            try { 
                Encoder.AddAudio(VideoLibrary.LocalVideoNoAudio, VideoLibrary.LocalAudio, output);

                Assert.IsTrue(File.Exists(output));
            }
            finally
            {
                if (File.Exists(output))
                    File.Delete(output);
            }
        }

        [TestMethod]
        public void Audio_AddPoster()
        {
            string output = Input.OutputLocation(VideoType.MP4);

            try
            {
                Encoder.AddPosterToAudio(VideoLibrary.LocalCover, VideoLibrary.LocalAudio, output);
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
