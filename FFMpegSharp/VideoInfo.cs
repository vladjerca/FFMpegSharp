using FFMpegSharp.Enums;
using FFMpegSharp.FFMPEG;
using FFMpegSharp.FFMPEG.Enums;
using FFMpegSharp.FFMPEG.Extend;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace FFMpegSharp
{
    public class VideoInfo
    {
        private FFMpeg ffmpeg = null;
        private FFMpeg FFmpeg
        {
            get
            {
                if (ffmpeg != null && ffmpeg.IsWorking)
                    throw new InvalidOperationException("Cannot do other operations while the current is being processed!");

                return ffmpeg ?? (ffmpeg = new FFMpeg());
            }
        }

        private FileInfo file;

        public TimeSpan Duration { get; internal set; }
        public string AudioFormat { get; internal set; }
        public string VideoFormat { get; internal set; }
        public string Ratio { get; internal set; }
        public double FrameRate { get; internal set; }
        public int Height { get; internal set; }
        public int Width { get; internal set; }
        public double Size { get; internal set; }

        public static VideoInfo FromFileInfo(FileInfo fileInfo)
        {
            return FromPath(fileInfo.FullName);
        }

        public static VideoInfo FromPath(string path)
        {
            return new VideoInfo(path);
        }

        public VideoInfo(FileInfo fileInfo)
        {
            fileInfo.Refresh();

            if(!fileInfo.Exists)
                throw new ArgumentException(string.Format("Input file {0} does not exist!", fileInfo.FullName));

            file = fileInfo;

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

        public string RootDirectory { get { return file.Directory.FullName + "\\"; } }

        public string FileName { get { return file.Name; } }

        public string Path { get { return file.FullName; } }

        public string Extension { get { return file.Extension; } }

        public bool IsReadOnly { get { return file.IsReadOnly; } }

        public bool Exists { get { return File.Exists(Path); } }

        public DateTime CreationTime { get { return file.CreationTime; } }

        public DirectoryInfo Directory { get { return file.Directory; } }

        public FileStream FileOpen(FileMode mode)
        {
            return file.Open(mode);
        }

        public void MoveTo(DirectoryInfo destination)
        {
            string newLocation = string.Format("{0}\\{1}{2}", destination.FullName, FileName, Extension);
            file.MoveTo(newLocation);
            file = new FileInfo(newLocation);
        }

        public void Delete()
        {
            file.Delete();
        }

        public VideoInfo ConvertTo(VideoType type, FileInfo output, Speed speed, VideoSize size, AudioQuality audio, bool multithread = false)
        {
            bool success = false;

            switch (type)
            {
                case VideoType.MP4: success = FFmpeg.ToMP4(this, output, speed, size, audio, multithread); break;
                case VideoType.OGV: success = FFmpeg.ToOGV(this, output, size, audio, multithread); break;
                case VideoType.WebM: success = FFmpeg.ToWebM(this, output, size, audio, multithread); break;
                case VideoType.TS: success = FFmpeg.ToTS(this, output); break;
                default: throw new ArgumentException("Video type is not supported yet!");
            }

            if (!success)
                throw new OperationCanceledException("The conversion process could not be completed.");

            return FromFileInfo(output);
        }

        public bool Mute(FileInfo output)
        {
            return FFmpeg.Mute(this, output);
        }

        public bool ExtractAudio(FileInfo output)
        {
            return FFmpeg.ExtractAudio(this, output);
        }

        public bool ReplaceAudio(FileInfo audio, FileInfo output)
        {
            return FFmpeg.ReplaceAudio(this, audio, output);
        }

        public Bitmap Snapshot(Size? size = null, TimeSpan? captureTime = null)
        {
            FileInfo output = new FileInfo(string.Format("{0}.png", Environment.TickCount));

            var success = FFmpeg.Snapshot(this, output, size, captureTime);

            if (!success)
                throw new OperationCanceledException("Could not take snapshot!");

            output.Refresh();

            Bitmap result;

            using (Bitmap bmp = (Bitmap)Image.FromFile(output.FullName))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);

                    result = new Bitmap(ms);
                }
            }

            if(output.Exists)
            {
                output.Delete();
            }

            return result;
        }

        public Bitmap Snapshot(FileInfo output, Size? size = null, TimeSpan? captureTime = null)
        {
            var success = FFmpeg.Snapshot(this, output, size, captureTime);

            if (!success)
                throw new OperationCanceledException("Could not take snapshot!");

            Bitmap result;

            using (var bmp = (Bitmap)Bitmap.FromFile(output.FullName))
            {
                result = (Bitmap)bmp.Clone();
            }

            return result;
        }

        public VideoInfo JoinWith(FileInfo output, bool purgeSources = false, params VideoInfo[] videos)
        {
            var queuedVideos = videos.ToList();

            queuedVideos.Insert(0, this);

            var success = FFmpeg.Join(output.FullName, queuedVideos);

            if (!success)
                throw new OperationCanceledException("Could not join the videos.");

            if(purgeSources)
            {
                foreach(var video in videos)
                {
                    video.Delete();
                }
            }

            return new VideoInfo(output);
        }

        public void CancelOperation()
        {
            FFmpeg.Stop();
        }
    }
}
