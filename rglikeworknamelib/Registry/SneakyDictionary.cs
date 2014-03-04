using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rglikeworknamelib.Registry
{
    public class SneakyDictionary<TKey, TValue> {
        private Dictionary<TKey, TValue> dictionary_;

        public SneakyDictionary() {
            dictionary_ = new Dictionary<TKey, TValue>();
        }

        public SneakyDictionary(Dictionary<TKey, TValue> dictionary) {
            dictionary_ = dictionary;
        }

        public TValue this[TKey key] { get { return dictionary_[key]; } }

        public IEnumerable<TKey> Keys { get { return dictionary_.Keys; } }
        public IEnumerable<TValue> Values { get { return dictionary_.Values; } }
        public bool ContainsValue(TValue val)
        {
            return dictionary_.ContainsValue(val);
        }
        public bool ContainsKey(TKey key)
        {
            return dictionary_.ContainsKey(key);
        }

        private void Add(TKey key, TValue value)
        {
            dictionary_.Add(key, value);
        }
}
}
