using FFMpegSharp.FFMPEG.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class VideoCodecArgument : Argument<VideoCodec>
    {
        public int Bitrate { get; protected set; } = 0;

        public VideoCodecArgument()
        {
        }

        public VideoCodecArgument(VideoCodec value) : base(value)
        {
        }

        public VideoCodecArgument(VideoCodec value, int bitrate) : base(value)
        {
            Bitrate = bitrate;
        }

        public override ArgumentsFlag Flag => ArgumentsFlag.VideoCodec;

        public override string GetStringValue()
        {
            return ArgumentsStringifier.Video(Value, Bitrate);
        }
    }
}
