using meta_store.Utils;
using System.Collections.Generic;
using System.Linq;

namespace meta_store
{
    public partial class Store
    {
        public Store Parent => parent;

        public Store Root
        {
            get
            {
                var ret = this;

                while (ret.parent != null)
                {
                    ret = ret.parent;
                }
                return ret;
            }
        }

        public string Path
        {
            get
            {
                var store = this;
                var path = store.key ?? "";

                store = store.parent;

                while (store != null)
                {
                    if (!string.IsNullOrEmpty(store.key))
                    {
                        path = store.key + '/' + path;
                    }

                    store = store.parent;
                }

                return path;
            }
        }

        public string Key => key;

        private object state;
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
            if (string.IsNullOrEmpty(path)) return this;
            return !Paths.ShouldSplit(path) ? At1(path) : Paths.Split(path).Aggregate(this, (current, k) => current.At1(k));
        }

        public object Get1(string key)
        {
            var ret = Sigo.Get1(this.state, key);
            Sigo.Freeze(ret);
            return ret;
        }

        public object Get(string path)
        {
            return Sigo.Freeze(Sigo.Get(this.state, path));
        }

        public object Get()
        {
            return Sigo.Freeze(state);
        }

        public void Set1(string key, object value)
        {
            this.state = Sigo.Set1(state, key, Sigo.Freeze(value));
        }

        public void Set(string path, object value)
        {
            this.state = Sigo.Set(state, path, Sigo.Freeze(value));
        }

        // tương đương với root.Set(path, value)
        public void Set(object value)
        {
            Sigo.Freeze(value);

            if (this.state != value)
            {
                this.state = value;
                dirty++;
                SetDown();
                SetUp();
            }
        }

        void SetUp()
        {
            if (this.children == null) return;
            foreach (var child in children)
            {
                var v = Sigo.Get1(this.state, child.Key);
                if (child.Value.state != v)
                {
                    child.Value.state = v;
                    child.Value.dirty++;
                    child.Value.SetUp();
                }
            }
        }

        void SetDown()
        {
            var p = this.parent;
            if (p != null)
            {
                p.state = Sigo.Set1(p.state, key, state);
                p.dirty++;
                p.SetDown();
            }
        }
    }
}
