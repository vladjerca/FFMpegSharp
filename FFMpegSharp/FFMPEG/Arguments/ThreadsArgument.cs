using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class ThreadsArgument : Argument<int>
    {
        public ThreadsArgument()
        {
        }

        public ThreadsArgument(int value) : base(value)
        {
        }

        public ThreadsArgument(bool isMultiThreaded) : 
            base(isMultiThreaded
                ? Environment.ProcessorCount
                : 1)
        {
        }

        public override string GetStringValue()
        {
            return ArgumentsStringifier.Threads(Value);
        }
    }
}
