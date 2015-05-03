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
        public static readonly string LocalVideo = ".\\Resources\\input.mp4";
        public static readonly string LocalVideoNoAudio = ".\\Resources\\mute.mp4";
        public static readonly string LocalAudio = ".\\Resources\\audio.mp3";
        public static readonly string LocalCover = ".\\Resources\\cover.png";

        public static string OutputLocation(this FileInfo file, VideoType type)
        {
            return OutputLocation(file, type, "_converted");
        }

        public static string OutputLocation(this FileInfo file, AudioType type)
        {
            return OutputLocation(file, type, "_audio");
        }

        public static string OutputLocation(this FileInfo file, ImageType type)
        {
            return OutputLocation(file, type, "_screenshot");
        }

        private static string OutputLocation(FileInfo file, Enum type, string keyword)
        {
            return file.Directory.FullName + "\\" + file.Name.Replace(file.Extension, keyword + "." + type.ToString().ToLower());
        }
    }
}
