using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class LoopArgument : Argument<int>
    {
        public LoopArgument()
        {
        }

        public LoopArgument(int value) : base(value)
        {
        }

        public override string GetStringValue()
        {
            return ArgumentsStringifier.Loop(Value);
        }
    }
}
