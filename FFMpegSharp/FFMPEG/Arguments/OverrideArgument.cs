using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class OverrideArgument : Argument
    {
        public OverrideArgument()
        {
        }

        public override string GetStringValue()
        {
            return "-y";
        }
    }
}
