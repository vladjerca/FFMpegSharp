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
            var output = Input.OutputLocation(VideoType.MP4);

            try
            {
                Encoder.Mute(VideoInfo.FromFileInfo(Input), output);
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
            var output = Input.OutputLocation(AudioType.MP3);

            try {
                Encoder.SaveAudio(VideoInfo.FromFileInfo(Input), output);

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
            var output = Input.OutputLocation(VideoType.MP4);
            try { 
                Encoder.AddAudio(VideoInfo.FromFileInfo(VideoLibrary.LocalVideoNoAudio), VideoLibrary.LocalAudio, output);

                Assert.IsTrue(File.Exists(output.FullName));
            }
            finally
            {
                if (File.Exists(output.FullName))
                    output.Delete();
            }
        }

        [TestMethod]
        public void Audio_AddPoster()
        {
            var output = Input.OutputLocation(VideoType.MP4);

            try
            {
                Encoder.AddPosterToAudio(VideoLibrary.LocalCover, VideoLibrary.LocalAudio, output);
                Assert.IsTrue(File.Exists(output.FullName));
            }
            finally
            {
                if (File.Exists(output.FullName))
                    output.Delete();
            }
        }
    }
}
