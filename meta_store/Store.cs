using meta_store.Utils;
using System.Collections.Generic;
using System.Linq;

namespace meta_store
{
    public partial class Store
    {
        private object state;
        private object action;
        private Store parent;
        private string key;
        private Dictionary<string, Store> children;

        public Store At1(string key)
        {
            children = children ?? new Dictionary<string, Store>();

            return children.TryGetValue(key, out var value)
                ? value
                : children[key] = new Store
                {
                    key = key,
                    parent = this,
                };
        }

        public Store At(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return this;
            }

            return Paths.ShouldSplit(path) ? Paths.Split(path).Aggregate(this, (current, k) => current.At1(k)) : At1(path);
        }

        public object Get1(string key)
        {
            var ret = Sigo.Get1(state, key);
            Sigo.Freeze(ret);
            return ret;
        }

        public object Get(string path) => Sigo.Freeze(Sigo.Get(state, path));

        public object Get() => Sigo.Freeze(state);

        public void Set1(string key, object value) => state = Sigo.Set1(state, key, Sigo.Freeze(value));

        public void Set(string path, object value) => state = Sigo.Set(state, path, Sigo.Freeze(value));

        // tương đương với root.Set(path, value)
        public void Set(object value)
        {
            Sigo.Freeze(value);

            if (state != value)
            {
                state = value;
                dirty++;
                SetDown();
                SetUp();
            }
        }

        private void SetUp()
        {
            if (children == null)
            {
                return;
            }

            foreach (var child in children)
            {
                var v = Sigo.Get1(state, child.Key);
                if (child.Value.state != v)
                {
                    child.Value.state = v;
                    child.Value.dirty++;
                    child.Value.SetUp();
                }
            }
        }

        private void SetDown()
        {
            var p = parent;
            if (p != null)
            {
                p.state = Sigo.Set1(p.state, key, state);
                p.dirty++;
                p.SetDown();
            }
        }
    }
}
