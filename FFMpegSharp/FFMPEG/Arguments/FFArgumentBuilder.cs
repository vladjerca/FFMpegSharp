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
                if(a.Key != ArgumentsFlag.Input && a.Key != ArgumentsFlag.Output && a.Key != ArgumentsFlag.Concat)
                {
                    sb.Append(a.Value.GetStringValue());
                }
            }

            sb.Append(container[ArgumentsFlag.Output].GetStringValue());

            return sb.ToString();
        }

        public string BuildArguments(ArgumentsContainer container, ArgumentsFlag flag)
        {
            if (!container.ContainsInputOutput())
                throw new ArgumentException("No input or output parameter found", nameof(container));

            CheckContainerException(container);
            StringBuilder sb = new StringBuilder();

            sb.Append(GetInput(container).GetStringValue());

            foreach (var a in container)
            {
                if (a.Key != ArgumentsFlag.Input && a.Key != ArgumentsFlag.Output && flag.HasFlag(a.Key) && a.Key != ArgumentsFlag.Concat)
                {
                    sb.Append(a.Value.GetStringValue());
                }
            }

            sb.Append(container[ArgumentsFlag.Output].GetStringValue());

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
                if (a.Key != ArgumentsFlag.Input && a.Key != ArgumentsFlag.Output && a.Key != ArgumentsFlag.Concat)
                {
                    sb.Append(a.Value.GetStringValue());
                }
            }

            sb.Append(outputA.GetStringValue());

            return sb.ToString();
        }

        public string BuildArguments(ArgumentsContainer container, FileInfo input, FileInfo output, ArgumentsFlag flag)
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
                if (a.Key != ArgumentsFlag.Input && a.Key != ArgumentsFlag.Output && flag.HasFlag(a.Key) && a.Key != ArgumentsFlag.Concat)
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
            //foreach(var kv in container)
            //{
            //    switch(kv.Value.Flag)
            //    {

            //    }
            //}
        }

        private void CheckExtensionOfOutputExtension(ArgumentsContainer container, FileInfo output)
        {
            if(container.ContainsKey(ArgumentsFlag.VideoCodec))
            {
                var codec = (VideoCodecArgument)container[ArgumentsFlag.VideoCodec];
                FFMpegHelper.ExtensionExceptionCheck(output, FileExtension.ForCodec(codec.Value));
            }
        }

        public Argument GetInput(ArgumentsContainer container)
        {
            if (container.ContainsKey(ArgumentsFlag.Input))
                return container[ArgumentsFlag.Input];
            else if (container.ContainsKey(ArgumentsFlag.Concat))
                return container[ArgumentsFlag.Concat];
            else
                throw new ArgumentException("No inputs found");
        }
    }
}
