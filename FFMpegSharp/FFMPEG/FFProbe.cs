using FFMpegSharp.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace FFMpegSharp.FFMPEG
{
    public sealed class FFProbe : FFBase, IDisposable
    {
        #region Private Members & Methods
        private string ffprobePath;

        private string RunProcess(string args)
        {
            RunProcess(args, ffprobePath, rStandardOutput: true);

            string output = null;

            try
            {
                process.Start();
                output = process.StandardOutput.ReadToEnd();
            }
            catch (Exception)
            {
                output = "";
            }
            finally
            {
                process.WaitForExit();
                process.Close();
            }

            return output;
        }
        #endregion

        public FFProbe() : base()
        {
            FFProbeHelper.RootExceptionCheck(configuredRoot);

            ffprobePath = configuredRoot + "\\ffprobe.exe";
        }

        /// <summary>
        /// Probes the targeted video file and retrieves all available details.
        /// </summary>
        /// <param name="source">Source video file.</param>
        /// <returns>A video info object containing all details necessary.</returns>
        public VideoInfo ParseVideoInfo(string source)
        {
            return ParseVideoInfo(new VideoInfo(source));
        }

        /// <summary>
        /// Probes the targeted video file and retrieves all available details.
        /// </summary>
        /// <param name="info">Source video file.</param>
        /// <returns>A video info object containing all details necessary.</returns>
        public VideoInfo ParseVideoInfo(VideoInfo info)
        {
            string jsonOutput = RunProcess(string.Format("-v quiet -print_format json -show_streams \"{0}\"", info.Path));

            Dictionary<string, dynamic> dict =
                (new JavaScriptSerializer()).Deserialize<Dictionary<string, dynamic>>(jsonOutput);
            int vid = dict["streams"][0]["codec_type"] == "video" ? 0 : 1,
                aud = 1 - vid;

            // Get video duration
            double numberData = 0;
            try
            {
                numberData = double.Parse(dict["streams"][vid]["duration"]);
                info.Duration = TimeSpan.FromSeconds(numberData);
                info.Duration = info.Duration.Subtract(TimeSpan.FromMilliseconds(info.Duration.Milliseconds));
            }
            catch (Exception) { info.Duration = TimeSpan.MinValue;  }
            
            

            // Get video size in megabytes
            double videoSize = 0,
                    audioSize = 0;

            try
            {
                info.VideoFormat = dict["streams"][vid]["codec_name"];
                videoSize = double.Parse(dict["streams"][vid]["bit_rate"]) * numberData / 8388608;
            }
            catch(Exception) { info.VideoFormat = "none"; }

            // Get audio format - wrap for exceptions if the video has no audio
            try
            {
                info.AudioFormat = dict["streams"][aud]["codec_name"];
                audioSize = double.Parse(dict["streams"][aud]["bit_rate"]) * double.Parse(dict["streams"][aud]["duration"]) / 8388608;
            }
            catch (Exception) { info.AudioFormat = "none"; }

            // Get video format
            

            // Get video width
            info.Width = dict["streams"][vid]["width"];

            // Get video height
            info.Height = dict["streams"][vid]["height"];

            info.Size = Math.Round(videoSize + audioSize, 2);

            // Get video aspect ratio
            int cd = FFProbeHelper.GCD(info.Width, info.Height);
            info.Ratio = info.Width / cd + ":" + info.Height / cd;

            // Get video framerate
            string[] fr = ((string)dict["streams"][vid]["r_frame_rate"]).Split('/');
            info.FrameRate = Math.Round(double.Parse(fr[0]) / double.Parse(fr[1]), 3);

            return info;
        }
    }
}
