using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class FrameOutputCountArgument : Argument<int>
    {
        public FrameOutputCountArgument()
        {
        }

        public FrameOutputCountArgument(int value) : base(value)
        {
        }

        public override string GetStringValue()
        {
            return ArgumentsStringifier.FrameOutputCount(Value);
        }
    }
}
