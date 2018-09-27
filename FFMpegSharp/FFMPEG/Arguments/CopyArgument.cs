using FFMpegSharp.FFMPEG.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class CopyAudioArgument : Argument
    {
        public CopyAudioArgument()
        {
        }

        public override string GetStringValue()
        {
            return ArgumentsStringifier.Copy(Channel.Audio);
        }
    }

    public class CopyVideoArgument : Argument
    {
        public CopyVideoArgument()
        {
        }

        public override string GetStringValue()
        {
            return ArgumentsStringifier.Copy(Channel.Video);
        }
    }
}
