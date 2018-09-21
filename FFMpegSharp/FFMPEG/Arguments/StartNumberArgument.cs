using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class StartNumberArgument : Argument<int>
    {
        public StartNumberArgument()
        {
        }

        public StartNumberArgument(int value) : base(value)
        {
        }

        public override ArgumentsFlag Flag => ArgumentsFlag.StartNumber;

        public override string GetStringValue()
        {
            return ArgumentsStringifier.StartNumber(Value);
        }
    }
}
