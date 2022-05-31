using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace meta_store
{
    partial class Sigo
    {
        private static readonly Sigo[] Elements = Enumerable.Range(0, 8).Select(i => new Sigo(new Dictionary<string, object>(), i + Bits.F)).ToArray();

        public static object Create(int flag) => Elements[flag & 7];

        public static object Create1(int flag, params object[] kvs)
        {
            Debug.Assert(kvs.Length % 2 == 0);

            var ret = Create(flag);
            var i = 0;
            while (i < kvs.Length - 1)
            {
                ret = Set1(ret, kvs[i++].ToString(), kvs[i++]);
            }

            return ret;
        }

        public static object Create(int flag, params object[] pvs)
        {
            Debug.Assert(pvs.Length % 2 == 0);

            var ret = Create(flag & Bits.LMR);
            var i = 0;
            while (i < pvs.Length - 1)
            {
                ret = Set(ret, pvs[i++].ToString(), pvs[i++]);
            }

            return ret;
        }

        public static object State(params object[] pvs)
        {
            return Create(Bits.MR, pvs);
        }

        public static object Update(params object[] pvs)
        {
            return Create(0, pvs);
        }

        public static object Delete()
        {
            return Create(Bits.MR);
        }

        public static int GetFlag(object obj)
        {
            return obj is Sigo tree ? tree.flag : Bits.FPLMR;
        }

        public static Sigo Add1(Sigo tree, string key, object value)
        {
            var rf = tree.flag;
            var vf = GetFlag(value);
            rf = Bits.LeftEffect(rf, vf);

            if (Bits.IsDef(rf, vf))
            {
                // SetFlags
                if (rf == tree.flag)
                {
                    return tree;
                }

                if (Bits.IsEmpty(rf))
                {
                    return Elements[Bits.Proton(rf)];
                }

                if (Bits.IsFrozen(rf))
                {
                    return new Sigo(new Dictionary<string, object>(tree.data), Bits.RemoveFrozen(rf));
                }
                else
                {
                    tree.flag = rf;
                    return tree;
                }
            }
            else
            {
                rf = Bits.CountUp(rf);
                if (Bits.IsFrozen(rf))
                {
                    // TODO create dictionary with capacity = tree.data.count + 1
                    var dict = new Dictionary<string, object>(tree.data)
                        {
                            { key, value }
                        };
                    return new Sigo(dict, Bits.RemoveFrozen(rf));
                }
                else
                {
                    tree.flag = rf;
                    tree.data.Add(key, value);
                    return tree;
                }
            }
        }


        public static Sigo Set1Tree(Sigo tree, string key, object value)
        {
            if (tree.TryGetValue(key, out var old))
            {
                // TODO khi Merge gọi Set1?
                // TODO nếu chuyển thành ReferenceEqual?
                if (Is(value, old))
                {
                    return tree;
                }

                var rf = tree.flag;
                var vf = GetFlag(value);
                rf = Bits.LeftEffect(rf, vf);

                if (Bits.IsDef(rf, vf))
                {
                    // remove
                    rf = Bits.CountDown(rf);
                    if (Bits.IsEmpty(rf))
                    {
                        return Elements[Bits.Proton(rf)];
                    }

                    if (Bits.IsFrozen(rf))
                    {
                        var dict = new Dictionary<string, object>(tree.data);
                        dict.Remove(key);
                        return new Sigo(dict, Bits.RemoveFrozen(rf));
                    }
                    else
                    {
                        tree.flag = rf;
                        tree.data.Remove(key);
                        return tree;
                    }
                }
                else
                {
                    // change
                    if (Bits.IsFrozen(rf))
                    {
                        var dict = new Dictionary<string, object>(tree.data)
                        {
                            [key] = value
                        };

                        return new Sigo(dict, Bits.RemoveFrozen(rf));
                    }
                    else
                    {
                        tree.flag = rf;
                        tree.data[key] = value;
                        return tree;
                    }
                }
            }
            else return Add1(tree, key, value);
        }

        public static object Set1(object obj, string key, object value)
        {
            if (obj is Sigo tree) return Set1Tree(tree, key, value);

            switch (GetFlag(value) & Bits.CPLMR)
            {
                case Bits.MR: return obj;
                case Bits.LMR: return Elements[Bits.LMR];
                default: return Set1Tree(Elements[Bits.LMR], key, value);
            }
        }

        public static object Get1(object obj, string key)
        {
            if (obj is Sigo tree)
            {
                return tree.TryGetValue(key, out var value) ? value : Create((tree.flag & Bits.R) * Bits.MR);
            }

            return Create(Bits.MR);
        }

        public static object Freeze(object obj)
        {
            if (obj is Sigo tree)
            {
                if (Bits.IsFrozen(tree.flag)) return obj;
                tree.flag = Bits.AddFrozen(tree.flag);
                foreach (var v in tree.Values) Freeze(v);
            }

            return obj;
        }

        public static bool IsFrozen(object obj)
        {
            return Bits.IsFrozen(GetFlag(obj));
        }

        public static bool IsUpdate(object obj)
        {
            return (GetFlag(obj) & 1) == 0;
        }

        public static object Set(object obj, IReadOnlyList<string> keys, int from, object value)
        {
            if (from >= keys.Count) return value;
            var key = keys[from];

            return Set1(obj, key, Set(Get1(obj, key), keys, from + 1, value));
        }

        public static object Set(object obj, string path, object value)
        {
            if (string.IsNullOrEmpty(path))
            {
                return value;
            }

            if (!path.Contains('/'))
            {
                return Set1(obj, path, value);
            }

            var keys = path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            
            return Set(obj, keys, 0, value);
        }
        
        /// <summary>
        /// Detect if a sigo which has children are itself
        /// </summary>
        public static bool IsLooped(object sigo)
        {
            switch (GetFlag(sigo) & Bits.CPLMR)
            {
                case 0:
                case 3:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// <code>
        /// sigo.Get("user/name") => sigo.Get1("user").Get1("name")
        /// sigo.Get("user") => sigo.Get1("user")
        /// sigo.Get("/") => sigo
        /// </code>
        /// </summary>
        public static object Get(object sigo, string path)
        {
            if (IsLooped(sigo))
            {
                return sigo;
            }

            if (string.IsNullOrEmpty(path))
            {
                return sigo;
            }

            if (!path.Contains('/'))
            {
                return Get1(sigo, path);
            }

            var keys = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var key in keys)
            {
                sigo = Get1(sigo, key);
                if (IsLooped(sigo))
                {
                    return sigo;
                }
            }

            return sigo;
        }


        /// <summary>
        /// Deep equals
        /// </summary>
        public static bool Equals(Sigo a, Sigo b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a.flag != b.flag) return false;
            if (a.data.Count != b.data.Count) return false;

            foreach (var e in a.data)
            {
                var k = e.Key;
                if (!Equals(e.Value, Sigo.Get1(b, k))) return false;
            }

            return true;
        }

        /// <summary>
        /// Deep equals
        /// </summary>
        public new static bool Equals(object a, object b)
        {
            if (a is Sigo sa && b is Sigo sb) return Equals(sa, sb);
            return object.Equals(a, b);
        }

        public static bool Is(object a, object b)
        {
            if (a is Sigo  || b is Sigo ) return ReferenceEquals(a, b);
            return object.Equals(a, b);
        }
    }
}