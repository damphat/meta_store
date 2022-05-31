using meta_store.Utils;
using Sigobase.Language;
using System.Text;

namespace meta_store
{
    partial class Sigo
    {
        public static string ToString(object sigo, Writer writer = null)
        {
            writer = writer ?? Writer.JsPretty;
            return writer.WriteSigo(new StringBuilder(), sigo, 0).ToString();
        }

        public static object Parse(string src)
        {
            return new SigoParser(src).Parse();
        }
    }
}