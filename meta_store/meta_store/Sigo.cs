using System.Collections;

namespace meta_store
{
    public partial class Sigo : IReadOnlyDictionary<string, object>
    {
        private Dictionary<string, object> data;

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) data).GetEnumerator();
        }

        public int Count => data.Count;

        public bool ContainsKey(string key)
        {
            return data.ContainsKey(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return data.TryGetValue(key, out value);
        }

        public object this[string key] => data[key];

        public IEnumerable<string> Keys => ((IReadOnlyDictionary<string, object>) data).Keys;

        public IEnumerable<object> Values => ((IReadOnlyDictionary<string, object>) data).Values;
    }
}