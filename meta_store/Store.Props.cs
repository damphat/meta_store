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
    }
}
