using FFMpegSharp.FFMPEG.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class ScaleArgument : Argument<Size>
    {
        public ScaleArgument()
        {
        }

        public ScaleArgument(Size value) : base(value)
        {
        }

        public ScaleArgument(int width, int heignt) : base(new Size(width, heignt))
        {
        }

        public ScaleArgument(VideoSize videosize)
        {
            Value = videosize == VideoSize.Original ? new Size(-1, -1) : new Size(-1, (int)videosize);
        }

        public override string GetStringValue()
        {
            return ArgumentsStringifier.Scale(Value);
        }
    }
}
