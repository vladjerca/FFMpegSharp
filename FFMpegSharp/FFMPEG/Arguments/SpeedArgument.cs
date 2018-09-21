using FFMpegSharp.FFMPEG.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class SpeedArgument : Argument<Speed>
    {
        public SpeedArgument()
        {            
        }

        public SpeedArgument(Speed value) : base(value)
        {
        }

        public override ArgumentsFlag Flag => ArgumentsFlag.Speed;

        public override string GetStringValue()
        {
            return ArgumentsStringifier.Speed(Value);
        }
    }
}
