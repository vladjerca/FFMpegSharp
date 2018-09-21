using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public enum ArgumentsFlag
    {
        Speed = 1,
        CpuSpeed = 2,
        AudioCodec = 4,
        VideoCodec = 8,
        Threads = 16,
        Input = 32,
        Size = 64,
        Scale = 128,
        Output = 256,
        ForceFormat = 512,
        BitStreamArgument = 1024,
        Copy = 2048,
        Seek = 4096,
        FrameOutputCount = 8192,
        Loop = 16384,
        Shortest = 32768,
        Concat = 65536,
        FrameRate = 131072,
        StartNumber = 262144
    }
}
