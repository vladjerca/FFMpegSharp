using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.Tests.Resources
{
    public enum VideoType
    {
        MP4,
        TS,
        WebM,
        OGV
    }

    public enum AudioType
    {
        MP3
    }

    public enum ImageType
    {
        PNG
    }

    public static class VideoLibrary
    {
        public static readonly FileInfo LocalVideo = new FileInfo(".\\Resources\\input.mp4");
        public static readonly FileInfo LocalVideoNoAudio = new FileInfo(".\\Resources\\mute.mp4");
        public static readonly FileInfo LocalAudio = new FileInfo(".\\Resources\\audio.mp3");
        public static readonly FileInfo LocalCover = new FileInfo(".\\Resources\\cover.png");

        public static FileInfo OutputLocation(this FileInfo file, VideoType type)
        {
            return OutputLocation(file, type, "_converted");
        }

        public static FileInfo OutputLocation(this FileInfo file, AudioType type)
        {
            return OutputLocation(file, type, "_audio");
        }

        public static FileInfo OutputLocation(this FileInfo file, ImageType type)
        {
            return OutputLocation(file, type, "_screenshot");
        }

        private static FileInfo OutputLocation(FileInfo file, Enum type, string keyword)
        {
            string originalLocation = file.Directory.FullName,
                   outputFile = file.Name.Replace(file.Extension, keyword + "." + type.ToString().ToLower());

            return new FileInfo(string.Format("{0}\\{1}", originalLocation, outputFile));
        }
    }
}
