using FFMpegSharp.FFMPEG.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class CopyArgument : Argument<Channel>
    {
        public CopyArgument()
        {
        }

        public CopyArgument(Channel value) : base(value)
        {
        }

        public override ArgumentsFlag Flag => ArgumentsFlag.Copy;

        public override string GetStringValue()
        {
            return ArgumentsStringifier.Copy(Value);
        }
    }
}
