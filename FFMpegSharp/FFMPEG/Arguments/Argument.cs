using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public abstract class Argument
    {
        public abstract ArgumentsFlag Flag { get; }

        public abstract string GetStringValue();

        public override string ToString()
        {
            return GetStringValue();
        }
    }

    public abstract class Argument<T> : Argument
    {
        private T _value;
        public T Value { get => _value; set { CheckValue(value); _value = value; } }

        public Argument() { }

        public Argument(T value)
        {
            Value = value;
        }

        protected virtual void CheckValue(T value)
        {
            
        }
    }

    public abstract class Argument<T1, T2> : Argument
    {

        private T1 _first;
        private T2 _second;

        public T1 First { get => _first; set { CheckFirst(_first); _first = value; } }
        public T2 Second { get => _second; set { CheckSecond(_second); _second = value; } }

        public Argument() { }

        public Argument(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        protected virtual void CheckFirst(T1 value)
        {

        }

        protected virtual void CheckSecond(T2 value)
        {

        }
    }
}
