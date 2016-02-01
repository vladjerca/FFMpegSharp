using FFMpegSharp.FFMPEG;
using System;
using System.IO;

namespace FFMpegSharp
{
    public class VideoInfo
    {
        private FileInfo _File;

        public TimeSpan Duration { get; set; }
        public string AudioFormat { get; set; }
        public string VideoFormat { get; set; }
        public string Ratio { get; set; }
        public double FrameRate { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public double Size { get; set; }

        public VideoInfo(FileInfo fileInfo)
        {
            if(!fileInfo.Exists)
                throw new ArgumentException(string.Format("Input file {0} does not exist!", fileInfo.FullName));

            _File = fileInfo;

            new FFProbe().ParseVideoInfo(this);
        }

        public VideoInfo(string path) : this(new FileInfo(path)) { }

        public override string ToString()
        {
            return "Video Path : " + Path + "\n" +
                   "Video Root : " + RootDirectory + "\n" +
                   "Video Name: " + FileName + "\n" +
                   "Video Extension : " + Extension + "\n" +
                   "Video Duration : " + Duration + "\n" +
                   "Audio Format : " + AudioFormat + "\n" +
                   "Video Format : " + VideoFormat + "\n" +
                   "Aspect Ratio : " + Ratio + "\n" +
                   "Framerate : " + FrameRate + "fps\n" +
                   "Resolution : " + Width + "x" + Height + "\n" +
                   "Size : " + Size + " Mb";
        }

        public string RootDirectory { get { return _File.Directory.FullName + "\\"; } }

        public string FileName { get { return _File.Name; } }

        public string Path { get { return _File.FullName; } }

        public string Extension { get { return _File.Extension; } }

        public void Delete()
        {
            _File.Delete();
        }

        public static VideoInfo FromFileInfo(FileInfo fileInfo)
        {
            return new VideoInfo(fileInfo.FullName);
        }

        public static VideoInfo FromPath(string path)
        {
            return new VideoInfo(path);
        }
    }
}
