using FFMpegSharp.FFMPEG.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class AudioCodecArgument : Argument<AudioCodec>
    {
        public int Bitrate { get; protected set; } = (int)AudioQuality.Normal;

        public AudioCodecArgument()
        {
        }

        public AudioCodecArgument(AudioCodec value) : base(value)
        {
        }

        public AudioCodecArgument(AudioCodec value, AudioQuality bitrate) : base(value)
        {
            Bitrate = (int)bitrate;
        }

        public AudioCodecArgument(AudioCodec value, int bitrate) : base(value)
        {
            Bitrate = bitrate;
        }
        
        public override string GetStringValue()
        {
            return ArgumentsStringifier.Audio(Value, Bitrate);
        }
    }
}
