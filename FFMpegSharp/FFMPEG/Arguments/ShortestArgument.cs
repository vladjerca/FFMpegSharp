using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class ShortestArgument : Argument<bool>
    {
        public ShortestArgument()
        {
        }

        public ShortestArgument(bool value) : base(value)
        {
        }

        public override ArgumentsFlag Flag => ArgumentsFlag.Shortest;

        public override string GetStringValue()
        {
            return ArgumentsStringifier.FinalizeAtShortestInput(Value);
        }
    }
}
