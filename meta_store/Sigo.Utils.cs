using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace meta_store
{
    partial class Sigo
    {
        public static object Create(params object[] keys)
        {
            object tree = null;
            var i = 0;
            Debug.Assert(keys.Length % 2 == 0);
            while (i < keys.Length - 1)
            {
                var key = keys[i++].ToString();
                var value = keys[i++];
                tree = Set1(tree, key, value);
            }

            return tree;
        }
        public static void KeyValidate(object key)
        {
            if (key == null) throw  new ArgumentNullException("key");
            if (key is string s)
            {
                if (string.IsNullOrEmpty(s)) throw new ArgumentException("key is empty");

                if (s.Contains('/')) throw new ArgumentException("key contains '/'");
            }
        }

        public static object Set1(object obj, string key, object value)
        {
            if (obj is Sigo tree)
            {
                var old = Get1(tree, key);
                if (Equals(old, value)) return tree;

                if (tree.frozen)
                {
                    var data = new Dictionary<string, object>(tree.data);
                    data[key] = value;
                    return new Sigo
                    {
                        data = data,
                        frozen = false
                    };
                }
                else
                {
                    tree.data[key] = value;
                    return tree;
                }
            }
            else
            {
                return new Sigo
                {
                    data = new Dictionary<string, object> {{key, value}},
                    frozen = false
                };
            }
        }

        public static object Get1(object obj, string key)
        {
            if (obj is Sigo tree)
            {
                return tree.TryGetValue(key, out var value) ? value : null;
            }

            return null;
        }

        public static void Freeze(object obj)
        {
            if (obj is Sigo tree)
            {
                if (!tree.frozen)
                {
                    tree.frozen = true;
                    foreach (var v in tree.Values)
                    {
                        Freeze(v);
                    }
                }
                
            }
        }

        public static bool IsFrozen(object obj)
        {
            if (obj is Sigo tree)
            {
                return tree.frozen;
            }

            return true;
        }

        public static object Set(object obj, string[] keys, int from, object value)
        {
            if (from >= keys.Length) return value;
            var key = keys[from];

            return Set1(obj, key, Set(Get1(obj, key), keys, from + 1, value));
        }

        public static object Set(object obj, string path, object value)
        {
            return Set(obj, ToKeys(path), 0, value);
        }

        public static string[] ToKeys(string path)
        {
            return path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        }

        public static object Get(object obj, string path)
        {
            foreach (var key in ToKeys(path))
            {
                if(obj is null) break;
                obj = Get1(obj, key);
            }

            return obj;
        }
    }
}