using FFMpegSharp.FFMPEG.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public interface IArgumentBuilder
    {
        string BuildArguments(ArgumentsContainer container);
        string BuildArguments(ArgumentsContainer container, ArgumentsFlag flag);

        string BuildArguments(ArgumentsContainer container, FileInfo input, FileInfo output);
        string BuildArguments(ArgumentsContainer container, FileInfo input, FileInfo output, ArgumentsFlag flag);
    }
}
