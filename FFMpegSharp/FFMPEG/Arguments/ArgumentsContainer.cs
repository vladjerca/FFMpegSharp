using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegSharp.FFMPEG.Arguments
{
    public class ArgumentsContainer : IDictionary<Type, Argument>
    {
        Dictionary<Type, Argument> _args;

        public ArgumentsContainer()
        {
            _args = new Dictionary<Type, Argument>();
        }

        public Argument this[Type key] { get => ((IDictionary<Type, Argument>)_args)[key]; set => ((IDictionary<Type, Argument>)_args)[key] = value; }

        public ICollection<Type> Keys => ((IDictionary<Type, Argument>)_args).Keys;

        public ICollection<Argument> Values => ((IDictionary<Type, Argument>)_args).Values;

        public int Count => ((IDictionary<Type, Argument>)_args).Count;

        public bool IsReadOnly => ((IDictionary<Type, Argument>)_args).IsReadOnly;

        public void Add(Type key, Argument value)
        {
            throw new InvalidOperationException("Not supported operation");
        }

        public void Add(KeyValuePair<Type, Argument> item)
        {
            throw new InvalidOperationException("Not supported operation");
        }

        public void Clear()
        {
            ((IDictionary<Type, Argument>)_args).Clear();
        }

        public bool Contains(KeyValuePair<Type, Argument> item)
        {
            return ((IDictionary<Type, Argument>)_args).Contains(item);
        }


        public void Add(Argument value)
        {
            ((IDictionary<Type, Argument>)_args).Add(value.GetType(), value);
        }        

        public bool ContainsInputOutput()
        {
            return  ((ContainsKey(typeof(InputArgument)) && !ContainsKey(typeof(ConcatArgument))) ||
                    (!ContainsKey(typeof(InputArgument)) && ContainsKey(typeof(ConcatArgument))))
                    && ContainsKey(typeof(OutputArgument));
        }

        public bool ContainsKey(Type key)
        {
            return ((IDictionary<Type, Argument>)_args).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<Type, Argument>[] array, int arrayIndex)
        {
            ((IDictionary<Type, Argument>)_args).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<Type, Argument>> GetEnumerator()
        {
            return ((IDictionary<Type, Argument>)_args).GetEnumerator();
        }

        public bool Remove(Type key)
        {
            return ((IDictionary<Type, Argument>)_args).Remove(key);
        }

        public bool Remove(KeyValuePair<Type, Argument> item)
        {
            return ((IDictionary<Type, Argument>)_args).Remove(item);
        }

        public bool TryGetValue(Type key, out Argument value)
        {
            return ((IDictionary<Type, Argument>)_args).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<Type, Argument>)_args).GetEnumerator();
        }
    }
}
