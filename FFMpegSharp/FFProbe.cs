using FFMpegSharp.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace FFMpegSharp
{
    public class FFProbe
    {
        private string _ffprobePath;
        private Process _process;

        public FFProbe(string rootPath = null)
        {
            if (rootPath == null)
                rootPath = ConfigurationManager.AppSettings["ffmpegRoot"];

            FFProbeHelper.RootExceptionCheck(rootPath);

            _ffprobePath = rootPath + "\\ffprobe.exe";
        }

        private string _RunProcess(string args)
        {
            _process = new Process();
            _process.StartInfo.FileName = _ffprobePath;
            _process.StartInfo.Arguments = args;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.RedirectStandardOutput = true;
            string output = null;
            try
            {
                _process.Start();
                output = _process.StandardOutput.ReadToEnd();
            }
            catch (Exception)
            {
                output = "";
            }
            finally
            {
                _process.WaitForExit();
                _process.Close();
            }
            return output;
        }

        /// <summary>
        /// Probes the targeted video file and retrieves all available details.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <returns>A video info object containing all details necessary.</returns>
        public VideoInfo GetVideoInfo(string source)
        {
            return GetVideoInfo(new VideoInfo(source));
        }

        /// <summary>
        /// Probes the targeted video file and retrieves all available details.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <returns>A video info object containing all details necessary.</returns>
        public VideoInfo GetVideoInfo(VideoInfo source)
        {
            string jsonOutput = _RunProcess(string.Format("-v quiet -print_format json -show_streams {0}", source.Path));

            Dictionary<string, dynamic> dict =
                (new JavaScriptSerializer()).Deserialize<Dictionary<string, dynamic>>(jsonOutput);
            int vid = dict["streams"][0]["codec_type"] == "video" ? 0 : 1,
                aud = 1 - vid;

            // Get video duration
            double numberData;
            numberData = double.Parse(dict["streams"][vid]["duration"]);
            source.Duration = TimeSpan.FromSeconds(numberData);
            source.Duration = source.Duration.Subtract(TimeSpan.FromMilliseconds(source.Duration.Milliseconds));

            // Get video size in megabytes
            double videoSize = double.Parse(dict["streams"][vid]["bit_rate"]) * numberData / 8388608,
                    audioSize = 0;

            // Get audio format - wrap for exceptions if the video has no audio
            try
            {
                source.AudioFormat = dict["streams"][aud]["codec_name"];
                audioSize = double.Parse(dict["streams"][aud]["bit_rate"]) * double.Parse(dict["streams"][aud]["duration"]) / 8388608;
            }
            catch (Exception) { source.AudioFormat = "none"; }

            // Get video format
            source.VideoFormat = dict["streams"][vid]["codec_name"];

            // Get video width
            source.Width = dict["streams"][vid]["width"];

            // Get video height
            source.Height = dict["streams"][vid]["height"];

            source.Size = Math.Round(videoSize + audioSize, 2);

            // Get video aspect ratio
            int cd = FFProbeHelper.GCD(source.Width, source.Height);
            source.Ratio = source.Width / cd + ":" + source.Height / cd;

            // Get video framerate
            string[] fr = ((string)dict["streams"][vid]["r_frame_rate"]).Split('/');
            source.FrameRate = Math.Round(double.Parse(fr[0]) / double.Parse(fr[1]), 3);

            return source;
        }

        /// <summary>
        /// Sets the video info for a specific file.
        /// </summary>
        /// <param name="source">Targeted video file.</param>
        public void SetVideoInfo(ref VideoInfo source)
        {
            source = GetVideoInfo(source);
        }

        public void Kill()
        {
            if (!_process.HasExited)
                _process.Kill();
        }
    }
}
