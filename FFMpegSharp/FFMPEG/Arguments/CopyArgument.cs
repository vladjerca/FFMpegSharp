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
            Value = Channel.Both;
        }

        public CopyArgument(Channel value = Channel.Both) : base(value)
        {
        }

        public override string GetStringValue()
        {
            return ArgumentsStringifier.Copy(Value);
        }
    }
}
