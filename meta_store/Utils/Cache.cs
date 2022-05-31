using System.Collections.Generic;

namespace meta_store
{
    public class Cache
    {
        readonly List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
        readonly Stack<int> indexes = new Stack<int>();
        public void Push()
        {
            indexes.Push(list.Count);
        }

        public void Add(string key, object value)
        {
            list.Add(new KeyValuePair<string, object>(key, value));
        }

        public object Pop(object a)
        {

            var from = indexes.Pop();
            while (from < list.Count)
            {
                var e = list[from];
                a = Sigo.Set1(a, e.Key, e.Value);
                from++;
            }
            return a;
        }

        public void Cancel()
        {
            var from = indexes.Pop();
            list.RemoveRange(from, list.Count - from);
        }
    }
}
