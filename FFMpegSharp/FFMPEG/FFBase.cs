using System;
using System.Configuration;
using System.Diagnostics;

namespace FFMpegSharp.FFMPEG
{
    public abstract class FFBase : IDisposable
    {
        protected Process process;
        protected string configuredRoot;

        public FFBase()
        {
            configuredRoot = ConfigurationManager.AppSettings["ffmpegRoot"];
        }

        protected void RunProcess(string args, string processPath, bool rStandardInput = false, bool rStandardOutput = false, bool rStandardError = false)
        {
            process = new Process();

            process.StartInfo.FileName = processPath;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            
            process.StartInfo.RedirectStandardInput = rStandardInput;
            process.StartInfo.RedirectStandardOutput = rStandardOutput;
            process.StartInfo.RedirectStandardError = rStandardError;
        }

        /// <summary>
        /// Returns true if the killing the associated process throws an exception
        /// </summary>
        public bool IsKillFaulty { get; private set; }

        /// <summary>
        /// Returns true if the associated process is till alive/running
        /// </summary>
        public bool IsWorking
        {
            get
            {
                return process != null && !process.HasExited && Process.GetProcessById(process.Id) != null;
            }
        }

        public void Kill()
        {
            try
            {
                if (IsWorking)
                    process.Kill();
            }
            catch
            {
                IsKillFaulty = true;
            }
        }

        public void Dispose()
        {
            if (process != null)
            {
                process.Dispose();
            }
        }
    }
}
