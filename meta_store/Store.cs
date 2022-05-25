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
            children ??= new Dictionary<string, Store>();

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
            return Sigo.ToKeys(path).Aggregate(this, (current, k) => current.At1(k));
        }

        public object Get1(string key)
        {
            var ret = Sigo.Get1(this.state, key);
            Sigo.Freeze(ret);
            return ret;
        }

        public object Get(string path)
        {
            var ret = Sigo.Get(this.state, path);
            Sigo.Freeze(ret);
            return ret;
        }

        public object Get()
        {
            Sigo.Freeze(state);
            return state;
        }

        public void Set1(string key, object value)
        {
            Sigo.Freeze(value);
            this.state = Sigo.Set1(state, key, value);
        }

        public void Set(string path, object value)
        {
            Sigo.Freeze(value);
            this.state = Sigo.Set(state, path, value);
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
