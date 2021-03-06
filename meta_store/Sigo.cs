using System.Collections;
using System.Collections.Generic;

namespace meta_store
{
    public partial class Sigo : IReadOnlyDictionary<string, object>
    {
        private readonly Dictionary<string, object> data;
        private int flag;

        private Sigo(Dictionary<string, object> data, int flag)
        {
            this.data = data;
            this.flag = flag;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)data).GetEnumerator();

        public int Count => data.Count;

        public bool ContainsKey(string key) => data.ContainsKey(key);

        public bool TryGetValue(string key, out object value) => data.TryGetValue(key, out value);

        public object this[string key] => data[key];

        public IEnumerable<string> Keys => ((IReadOnlyDictionary<string, object>)data).Keys;

        public IEnumerable<object> Values => ((IReadOnlyDictionary<string, object>)data).Values;
    }
}