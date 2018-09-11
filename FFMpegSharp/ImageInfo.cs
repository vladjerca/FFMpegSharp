using FFMpegSharp.Enums;
using FFMpegSharp.FFMPEG;
using FFMpegSharp.Helpers;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace FFMpegSharp
{
    public partial class ImageInfo
    {
        private FFMpeg _ffmpeg;
        private FileInfo _file;

        public ConversionHandler OnConversionProgress;

        public ImageInfo(FileInfo fileInfo)
        {
            if (!fileInfo.Extension.ToLower().EndsWith(FileExtension.Png))
            {
                throw new Exception("Image joining currently suppors only .png file types");
            }

            fileInfo.Refresh();

            this.Size = fileInfo.Length / (1024 * 1024);

            using (var image = Image.FromFile(fileInfo.FullName))
            {
                this.Width = image.Width;
                this.Height = image.Height;
                var cd = FFProbeHelper.Gcd(this.Width, this.Height);
                this.Ratio = $"{this.Width / cd}:{this.Height / cd}";
            }


            if (!fileInfo.Exists)
                throw new ArgumentException($"Input file {fileInfo.FullName} does not exist!");

            _file = fileInfo;


        }

        /// <summary>
        ///     Create a video information object from a target path.
        /// </summary>
        /// <param name="path">Path to video.</param>
        public ImageInfo(string path) : this(new FileInfo(path))
        {
        }

        private FFMpeg FFmpeg
        {
            get
            {
                if (_ffmpeg != null && _ffmpeg.IsWorking)
                    throw new InvalidOperationException(
                        "Another operation is in progress, please wait for this to finish before launching another operation. To do multiple operations, create another ImageInfo object targeting that file.");

                return _ffmpeg ?? (_ffmpeg = new FFMpeg());
            }
        }

        /// <summary>
        ///     Aspect ratio.
        /// </summary>
        public string Ratio { get; internal set; }

        /// <summary>
        ///     Height of the image file.
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        ///     Width of the image file.
        /// </summary>
        public int Width { get; internal set; }

        /// <summary>
        ///     Image file size in MegaBytes (MB).
        /// </summary>
        public double Size { get; internal set; }

        /// <summary>
        ///     Gets the name of the file.
        /// </summary>
        public string Name => _file.Name;

        /// <summary>
        ///     Gets the full path of the file.
        /// </summary>
        public string FullName => _file.FullName;

        /// <summary>
        ///     Gets the file extension.
        /// </summary>
        public string Extension => _file.Extension;

        /// <summary>
        ///     Gets a flag indicating if the file is read-only.
        /// </summary>
        public bool IsReadOnly => _file.IsReadOnly;

        /// <summary>
        ///     Gets a flag indicating if the file exists (no cache, per call verification).
        /// </summary>
        public bool Exists => File.Exists(FullName);

        /// <summary>
        ///     Gets the creation date.
        /// </summary>
        public DateTime CreationTime => _file.CreationTime;

        /// <summary>
        ///     Gets the parent directory information.
        /// </summary>
        public DirectoryInfo Directory => _file.Directory;

        /// <summary>
        ///     See if ffmpeg process associated to this video is idle (not alive).
        /// </summary>
        public bool OperationIdle => !FFmpeg.IsWorking;

        /// <summary>
        ///     Create a image information object from a file information object.
        /// </summary>
        /// <param name="fileInfo">Image file information.</param>
        /// <returns></returns>
        public static ImageInfo FromFileInfo(FileInfo fileInfo)
        {
            return FromPath(fileInfo.FullName);
        }

        /// <summary>
        ///     Create a video information object from a target path.
        /// </summary>
        /// <param name="path">Path to video.</param>
        /// <returns></returns>
        public static ImageInfo FromPath(string path)
        {
            return new ImageInfo(path);
        }

        /// <summary>
        ///     Pretty prints the video information.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Video Path : " + FullName + Environment.NewLine +
                   "Video Root : " + Directory.FullName + Environment.NewLine +
                   "Video Name: " + Name + Environment.NewLine +
                   "Video Extension : " + Extension + Environment.NewLine +
                   "Aspect Ratio : " + Ratio + Environment.NewLine +
                   "Resolution : " + Width + "x" + Height + Environment.NewLine +
                   "Size : " + Size + " MB";
        }

        /// <summary>
        ///     Open a file stream.
        /// </summary>
        /// <param name="mode">Opens a file in a specified mode.</param>
        /// <returns>File stream of the video file.</returns>
        public FileStream FileOpen(FileMode mode)
        {
            return _file.Open(mode);
        }

        /// <summary>
        ///     Move file to a specific directory.
        /// </summary>
        /// <param name="destination"></param>
        public void MoveTo(DirectoryInfo destination)
        {
            var newLocation = $"{destination.FullName}\\{Name}{Extension}";
            _file.MoveTo(newLocation);
            _file = new FileInfo(newLocation);
        }

        /// <summary>
        ///     Delete the file.
        /// </summary>
        public void Delete()
        {
            _file.Delete();
        }

        /// <summary>
        ///     Join the image file with other image files.
        /// </summary>
        /// <param name="output">Output location of the resulting image file.</param>
        /// <param name="purgeSources">>Flag original file purging after conversion is done.</param>
        /// <param name="images">Videos that need to be joined to the image.</param>
        /// <returns>Video information object with the new image file.</returns>
        public VideoInfo JoinWith(FileInfo output, double frameRate = 30, bool purgeSources = false, params ImageInfo[] images)
        {
            var queuedImages = images.ToList();

            queuedImages.Insert(0, this);

            var success = FFmpeg.JoinImageSequence(output, frameRate, queuedImages.ToArray());

            if (!success)
                throw new OperationCanceledException("Could not join the images.");

            if (purgeSources)
            {
                foreach (var image in images)
                {
                    image.Delete();
                }
            }

            return new VideoInfo(output);
        }

        /// <summary>
        ///     Tell FFMpeg to stop the current process.
        /// </summary>
        public void CancelOperation()
        {
            FFmpeg.Stop();
        }
    }
}