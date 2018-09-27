using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class ConcatArgument : Argument<IEnumerable<string>>, IEnumerable<string>
    {
        public ConcatArgument()
        {
            Value = new List<string>();
        }

        public ConcatArgument(IEnumerable<string> value) : base(value)
        {
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        public override string GetStringValue()
        {
            return ArgumentsStringifier.InputConcat(Value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
