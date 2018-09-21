using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class ArgumentsContainer : IDictionary<ArgumentsFlag, Argument>
    {
        Dictionary<ArgumentsFlag, Argument> _args;

        public ArgumentsContainer()
        {
            _args = new Dictionary<ArgumentsFlag, Argument>();
        }

        public Argument this[ArgumentsFlag key] { get => ((IDictionary<ArgumentsFlag, Argument>)_args)[key]; set => ((IDictionary<ArgumentsFlag, Argument>)_args)[key] = value; }

        public ICollection<ArgumentsFlag> Keys => ((IDictionary<ArgumentsFlag, Argument>)_args).Keys;

        public ICollection<Argument> Values => ((IDictionary<ArgumentsFlag, Argument>)_args).Values;

        public int Count => ((IDictionary<ArgumentsFlag, Argument>)_args).Count;

        public bool IsReadOnly => ((IDictionary<ArgumentsFlag, Argument>)_args).IsReadOnly;

        public void Add(ArgumentsFlag key, Argument value)
        {
            throw new InvalidOperationException("Not supported operation");
        }

        public void Add(Argument value)
        {
            ((IDictionary<ArgumentsFlag, Argument>)_args).Add(value.Flag, value);
        }

        public void Add(KeyValuePair<ArgumentsFlag, Argument> item)
        {
            throw new InvalidOperationException("Not supported operation");
        }

        public void Clear()
        {
            ((IDictionary<ArgumentsFlag, Argument>)_args).Clear();
        }

        public bool Contains(KeyValuePair<ArgumentsFlag, Argument> item)
        {
            return ((IDictionary<ArgumentsFlag, Argument>)_args).Contains(item);
        }

        public bool ContainsKey(ArgumentsFlag key)
        {
            return ((IDictionary<ArgumentsFlag, Argument>)_args).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<ArgumentsFlag, Argument>[] array, int arrayIndex)
        {
            ((IDictionary<ArgumentsFlag, Argument>)_args).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<ArgumentsFlag, Argument>> GetEnumerator()
        {
            return ((IDictionary<ArgumentsFlag, Argument>)_args).GetEnumerator();
        }

        public bool Remove(ArgumentsFlag key)
        {
            return ((IDictionary<ArgumentsFlag, Argument>)_args).Remove(key);
        }

        public bool Remove(KeyValuePair<ArgumentsFlag, Argument> item)
        {
            return ((IDictionary<ArgumentsFlag, Argument>)_args).Remove(item);
        }

        public bool TryGetValue(ArgumentsFlag key, out Argument value)
        {
            return ((IDictionary<ArgumentsFlag, Argument>)_args).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<ArgumentsFlag, Argument>)_args).GetEnumerator();
        }

        public bool ContainsInputOutput()
        {
            return  ((ContainsKey(ArgumentsFlag.Input) && !ContainsKey(ArgumentsFlag.Concat)) ||
                    (!ContainsKey(ArgumentsFlag.Input) && ContainsKey(ArgumentsFlag.Concat)))
                    && ContainsKey(ArgumentsFlag.Output);
        }
    }
}
