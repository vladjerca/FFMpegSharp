using FFMpegSharp.FFMPEG.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class ForceFormatArgument : Argument<VideoCodec>
    {
        public ForceFormatArgument()
        {
        }

        public ForceFormatArgument(VideoCodec value) : base(value)
        {
        }

        public override ArgumentsFlag Flag => ArgumentsFlag.ForceFormat;

        public override string GetStringValue()
        {
            return ArgumentsStringifier.ForceFormat(Value);
        }
    }
}
