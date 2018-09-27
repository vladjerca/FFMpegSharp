using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class CpuSpeedArgument : Argument<int>
    {
        public CpuSpeedArgument()
        {
        }

        public CpuSpeedArgument(int value) : base(value)
        {
        }

        public override string GetStringValue()
        {
            return ArgumentsStringifier.Speed(Value);
        }
    }
}
