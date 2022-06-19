using meta_store.Utils;
using System.Collections.Generic;

namespace meta_store
{
    public partial class Sigo
    {
        private static HashSet<string> KeysOf(object a, object b)
        {
            var keys = new HashSet<string>();
            if (a is Sigo sa)
            {
                keys.UnionWith(sa.data.Keys);
            }
            if (b is Sigo sb)
            {
                keys.UnionWith(sb.data.Keys);
            }
            return keys;
        }

        public static TreeLevel level;

        public static object Merge(object a, object b, TreeLevel level = null)
        {
            Sigo.level = level ?? TreeLevel.Create();
            return Merge2(a, b, Sigo.level).Item2;
        }

        public static (bool, object) Merge2(object a, object b, TreeLevel level)
        {
            level.a = a;
            level.b = b;

            if (ReferenceEquals(a, b))
            {
                level.Ret(false, a, "===");
                return (false, a);
            }

            if (b is Sigo sb)
            {
                // * {b: 1}
                if (a is Sigo sa)
                {
                    // {a:1} * {b:1}
                    return Merge2(sa, sb, level);
                } else
                {
                    // 'a' * {b: 1}
                    if ((sb.flag & 2) != 0) {
                        return Merge2(Elements[7], sb, level); 
                    } else 
                    { 
                        level.Ret(false, a, "'a'*nom");
                        return (false, a); 
                    };
                }
            } else
            {
                if(object.Equals(a, b))
                {
                    // 'b' * 'b'
                    level.Ret(false, a, "'a'*'a'");
                    return (false, a);
                } else
                {
                    // ('a' | {a: 1}) * 'b'
                    if (a is Sigo sa)
                    {
                        // {a: 1} * 1
                        // removeChildren(a)
                        level.Ret(true, b, "'b'*'b'");
                        return (true, b);
                    }
                    else
                    {
                        // 'a' * 'b'
                        level.Ret(true, b, "'b'*'b'");
                        return (true, b);
                    }
                }                
            }
        }

        // 
        public static (bool, Sigo) Merge2(Sigo sa, Sigo sb, TreeLevel level)
        {
            level.sa = sa;
            level.sb = sb;
            var rf = (sa.flag | sb.flag) & 7;
            var r = Elements[rf];
            var rc = 0;
            bool changed = rf != (sa.flag & 7);

            foreach (var k in KeysOf(sa, sb))
            {
                var ak = Get1(sa, k);
                var bk = Get1(sb, k);
                var lk = level.Next(k);

                var (childChanged, rk) = Merge2(ak, bk, lk);
                lk.rc = childChanged; 
                lk.r = rk;

                (rc, r) = MergeChild(r, k, rk, childChanged);
                lk.action = rc;
                if (rc != 0) changed = true;
            }

            return changed ? (true, r) : (false, sa);
        }

        public static (int, object) MergeChild(int rf , bool ah,  object ak, bool bh, object bk, string k)
        {
            if (ah)
            {
                if (ReferenceEquals(ak, bk))
                {
                    return (0, ak);
                }

                var (rc, rk) = Merge2(ak, bk, level, null);
                var vf = GetFlag(rk);

                if (Bits.IsDef(rf, vf))
                {
                    // remove
                    rf = Bits.CountDown(rf);
                    if (Bits.IsEmpty(rf))
                    {
                        return (2, Elements[Bits.Proton(rf)]);
                    }

                    if (Bits.IsFrozen(rf))
                    {
                        var dict = new Dictionary<string, object>(r.data);
                        dict.Remove(key);
                        return (2, new Sigo(dict, Bits.RemoveFrozen(rf)));
                    }
                    else
                    {
                        r.flag = rf;
                        r.data.Remove(key);
                        return (2, r);
                    }
                }
                else
                {
                    // change
                    if (Bits.IsFrozen(rf))
                    {
                        var dict = new Dictionary<string, object>(r.data)
                        {
                            [key] = rk
                        };

                        return (3, new Sigo(dict, Bits.RemoveFrozen(rf)));
                    }
                    else
                    {
                        r.flag = rf;
                        r.data[key] = rk;
                        return (3, r);
                    }
                }
            }
            else
            {
                var rf = r.flag;
                var vf = GetFlag(rk);

                if (Bits.IsDef(rf, vf))
                {
                    return (0, r);
                }
                else
                {
                    rf = Bits.CountUp(rf);
                    if (Bits.IsFrozen(rf))
                    {
                        // TODO create dictionary with capacity = tree.data.count + 1
                        var dict = new Dictionary<string, object>(r.data)
                        {
                            { key, rk }
                        };

                        return (1, new Sigo(dict, Bits.RemoveFrozen(rf)));
                    }
                    else
                    {
                        r.flag = rf;
                        r.data.Add(key, rk);
                        return (1, r);
                    }
                }
            }
        }

        public static (int, Sigo) MergeSet1(Sigo r, string key, object rk, bool c)
        {
            if (r.TryGetValue(key, out var old))
            {
                if (ReferenceEquals(rk, old))
                {
                    return c ? (3, r) : (0, r);
                }

                var rf = r.flag;
                var vf = GetFlag(rk);

                if (Bits.IsDef(rf, vf))
                {
                    // remove
                    rf = Bits.CountDown(rf);
                    if (Bits.IsEmpty(rf))
                    {
                        return (2, Elements[Bits.Proton(rf)]);
                    }

                    if (Bits.IsFrozen(rf))
                    {
                        var dict = new Dictionary<string, object>(r.data);
                        dict.Remove(key);
                        return (2, new Sigo(dict, Bits.RemoveFrozen(rf)));
                    }
                    else
                    {
                        r.flag = rf;
                        r.data.Remove(key);
                        return (2, r);
                    }
                }
                else
                {
                    // change
                    if (Bits.IsFrozen(rf))
                    {
                        var dict = new Dictionary<string, object>(r.data)
                        {
                            [key] = rk
                        };

                        return (3, new Sigo(dict, Bits.RemoveFrozen(rf)));
                    }
                    else
                    {
                        r.flag = rf;
                        r.data[key] = rk;
                        return (3, r);
                    }
                }
            }
            else
            {
                return MergeAdd1(r, key, rk, c );
            }
        }

        public static (int, Sigo) MergeAdd1(Sigo r, string key, object rk, bool c)
        {
            var rf = r.flag;
            var vf = GetFlag(rk);

            if (Bits.IsDef(rf, vf))
            {
                return (0, r);
            }
            else
            {
                rf = Bits.CountUp(rf);
                if (Bits.IsFrozen(rf))
                {
                    // TODO create dictionary with capacity = tree.data.count + 1
                    var dict = new Dictionary<string, object>(r.data)
                        {
                            { key, rk }
                        };

                    return (1, new Sigo(dict, Bits.RemoveFrozen(rf)));
                }
                else
                {
                    r.flag = rf;
                    r.data.Add(key, rk);
                    return (1, r);
                }
            }
        }
    }
}
