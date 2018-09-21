using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class SeekArgument : Argument<TimeSpan>
    {
        public SeekArgument()
        {
        }

        public SeekArgument(TimeSpan value) : base(value)
        {
        }

        public override ArgumentsFlag Flag => ArgumentsFlag.Seek;

        public override string GetStringValue()
        {
            return ArgumentsStringifier.Seek(Value);
        }
    }
}
