using FFMpegSharp.Enums;
using FFMpegSharp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class FFArgumentBuilder : IArgumentBuilder
    {
        public string BuildArguments(ArgumentsContainer container)
        {
            if (!container.ContainsInputOutput())
                throw new ArgumentException("No input or output parameter found", nameof(container));

            CheckContainerException(container);

            StringBuilder sb = new StringBuilder();

            sb.Append(GetInput(container).GetStringValue());

            foreach(var a in container)
            {
                if(!IsInputOrOutput(a.Key))
                {
                    sb.Append(a.Value.GetStringValue());
                }
            }

            sb.Append(container[typeof(OutputArgument)].GetStringValue());

            return sb.ToString();
        }


        public string BuildArguments(ArgumentsContainer container, FileInfo input, FileInfo output)
        {
            CheckContainerException(container);
            CheckExtensionOfOutputExtension(container, output);
            FFMpegHelper.ConversionExceptionCheck(input, output);


            StringBuilder sb = new StringBuilder();

            var inputA = new InputArgument(input);
            var outputA = new OutputArgument();

            sb.Append(inputA.GetStringValue());

            foreach (var a in container)
            {
                if (!IsInputOrOutput(a.Key))
                {
                    sb.Append(a.Value.GetStringValue());
                }
            }

            sb.Append(outputA.GetStringValue());

            return sb.ToString();
        }

        private void CheckContainerException(ArgumentsContainer container)
        {
            //TODO: implement arguments check            
        }

        private void CheckExtensionOfOutputExtension(ArgumentsContainer container, FileInfo output)
        {
            if(container.ContainsKey(typeof(VideoCodecArgument)))
            {
                var codec = (VideoCodecArgument)container[typeof(VideoCodecArgument)];
                FFMpegHelper.ExtensionExceptionCheck(output, FileExtension.ForCodec(codec.Value));
            }
        }

        public Argument GetInput(ArgumentsContainer container)
        {
            if (container.ContainsKey(typeof(InputArgument)))
                return container[typeof(InputArgument)];
            else if (container.ContainsKey(typeof(ConcatArgument)))
                return container[typeof(ConcatArgument)];
            else
                throw new ArgumentException("No inputs found");
        }

        private bool IsInputOrOutput(Argument arg)
        {
            return IsInputOrOutput(arg.GetType());
        }

        private bool IsInputOrOutput(Type arg)
        {
            return (arg == typeof(InputArgument)) || (arg == typeof(ConcatArgument)) || (arg == typeof(OutputArgument));
        }
    }
}
