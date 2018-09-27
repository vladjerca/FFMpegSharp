using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class FrameRateArgument : Argument<double>
    {
        public FrameRateArgument()
        {
        }

        public FrameRateArgument(double value) : base(value)
        {
        }

        public override string GetStringValue()
        {
            return ArgumentsStringifier.FrameRate(Value);
        }
    }
}
