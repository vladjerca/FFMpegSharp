using FFMpegSharp.FFMPEG.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class BitStreamFilterArgument : Argument<Channel, Filter>
    {
        public BitStreamFilterArgument()
        {
        }

        public BitStreamFilterArgument(Channel first, Filter second) : base(first, second)
        {
        }

        public override ArgumentsFlag Flag => ArgumentsFlag.BitStreamArgument;

        public override string GetStringValue()
        {
            return ArgumentsStringifier.BitStreamFilter(First, Second);
        }
    }
}
