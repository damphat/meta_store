using meta_store.Utils;
using System.Collections.Generic;
using System.Diagnostics;

namespace meta_store
{
    partial class Sigo
    {
        // Nguyên lý
        // 1) b che a
        // 2) muốn kết quả reference bên nào? "", "a", "b", "ab", "ba"
        // 3) một phần của b bị freeze nếu được sử dụng làm kết quả
        // 4) object freeze khi duy chuyển vào/ra store
        public static object Merge(object a, object b)
        {
            if (ReferenceEquals(a, b)) return a;
            if (b is Sigo sb)
            {
                if (a is Sigo sa)
                {
                    return Merge(sa, sb);
                }
                else
                {
                    return Bits.HasM(sb.flag) ? Merge(Elements[7], sb) : a;
                }
            }
            else
            {
                if (object.Equals(a, b)) return a; else return b;
            }
        }

        public static Sigo AddLM(Sigo sigo, int lm)
        {
            Debug.Assert((lm & Bits.LM) == lm);

            var rf = sigo.flag | lm;
            if (rf != sigo.flag)
            {
                if (rf < 256) return Elements[rf & 7];

                if (Bits.IsFrozen(rf))
                {
                    return new Sigo(new Dictionary<string, object>(sigo.data), Bits.RemoveFrozen(rf));
                }
                else
                {
                    sigo.flag = rf;
                }
            }
            return sigo;
        }

        // TODO use cache
        // TODO freeze parts of b if they are used in the result
        // e??0 * x => x | lm
        // e??1 * x => x | lm
        // x * e0 => x
        // x * e7 => e7
        // x * e??R => e[x.flag | e.flag]
        // x * e??0 => x | e.lm
        public static object Merge(Sigo a, Sigo b)
        {
            var fa = a.flag;
            var fb = b.flag;

            if (Bits.HasR(fb))
            {
                var fr = (fa | fb) & 7;
                Sigo ret = Elements[fr]; // TODO how to cache?

                if (fb >= 256)
                {
                    var eqa = (fa & 7) == fr;
                    var eqb = (fb & 7) == fr;
                    foreach (var e in b)
                    {
                        var k = e.Key;
                        var ak = Get1(a, k);
                        var bk = e.Value;
                        var rk = Merge(ak, bk);

                        if (eqa & !ReferenceEquals(ak, rk)) eqa = false;
                        if (eqb & !ReferenceEquals(bk, rk)) eqb = false;

                        ret = Add1(ret, k, rk);
                    }
                    if (eqa && (ret.Count == a.Count)) return a; // {x:1, y:1} * {x: 1} =  {x:1}
                    if (eqb) return b; // {x:1, y:1} * {x: 1} =  {x:1}
                    return ret;
                }

                return ret;
            }
            else
            {
                var fr = (fa | fb) & 7;
                Sigo ret = a;

                ret = AddLM(ret, Bits.LM & fb);

                if (fb >= 256)
                {
                    var eqa = (fa & 7) == fr;
                    var eqb = (fb & 7) == fr;
                    foreach (var e in b)
                    {
                        var k = e.Key;
                        var ak = Get1(a, k);
                        var bk = e.Value;
                        var rk = Merge(ak, bk);

                        if (eqa & !ReferenceEquals(ak, rk)) eqa = false;
                        if (eqb & !ReferenceEquals(bk, rk)) eqb = false;

                        ret = Set1Tree(ret, k, rk);
                    }
                    if (eqa && (ret.Count == a.Count)) return a;
                    if (eqb && (ret.Count == b.Count)) return b;
                    return ret;
                }

                return ret;

            }
        }
    }
}