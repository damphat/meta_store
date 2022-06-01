using System.Collections;
using System.Globalization;

namespace meta_store
{
    partial class Sigo
    {
        public static Sigo From(IEnumerable list)
        {
            var sigo = Elements[3];
            var i = 0;
            foreach (var e in list)
            {
                sigo = Set1Tree(sigo, i.ToString(CultureInfo.InvariantCulture), From(e));
                i++;
            }

            return sigo;
        }

        public static object From(IDictionary dict)
        {
            object sigo = Elements[3];
            foreach (DictionaryEntry e in dict)
            {
                sigo = Set(sigo, e.Key.ToString(), From(e.Value));
            }

            return sigo;
        }

        public static object From(string o) => o;
        public static object From(Sigo o) => o;

        public static object From(object o)
        {
            switch (o)
            {
                case Sigo sigo:
                    return sigo;
                case string s: return s;
                case IDictionary dict:
                    return From(dict);
                case IEnumerable list:
                    return From(list);
                default:
                    return o;
            }
        }
    }
}