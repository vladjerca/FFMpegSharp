using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace FFMpegSharp.FFMPEG
{
    public abstract class FFBase : IDisposable
    {
        protected Process process;
        protected string configuredRoot;

        public FFBase()
        {
            configuredRoot = ConfigurationManager.AppSettings["ffmpegRoot"];

            if (configuredRoot.EndsWith("\\"))
                configuredRoot = configuredRoot.Substring(0, configuredRoot.Length - 1);
        }

        protected void RunProcess(string args, string processPath, bool rStandardInput = false, bool rStandardOutput = false, bool rStandardError = false)
        {
            if (IsWorking)
                throw new InvalidOperationException("The current FFMpeg process is busy with another operation. Create a new object for parallel executions.");

            process = new Process();
            IsKillFaulty = false;
            process.StartInfo.FileName = processPath;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            
            process.StartInfo.RedirectStandardInput = rStandardInput;
            process.StartInfo.RedirectStandardOutput = rStandardOutput;
            process.StartInfo.RedirectStandardError = rStandardError;
        }

        /// <summary>
        /// Is 'true' when an exception is thrown during process kill (for paranoia level users).
        /// </summary>
        public bool IsKillFaulty { get; private set; }

        /// <summary>
        /// Returns true if the associated process is still alive/running.
        /// </summary>
        public bool IsWorking
        {
            get
            {
                bool processHasExited;

                try
                {
                    processHasExited = process.HasExited;
                }
                catch
                {
                    processHasExited = true;
                }

                return !processHasExited && Process.GetProcesses().Any(x => x.Id == process.Id);
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
