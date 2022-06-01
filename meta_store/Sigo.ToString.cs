using meta_store.Language;
using meta_store.Utils;
using System.Text;

namespace meta_store
{
    public partial class Sigo
    {
        public static string ToString(object sigo, Writer writer = null)
        {
            writer = writer ?? Writer.JsPretty;
            return writer.WriteSigo(new StringBuilder(), sigo, 0).ToString();
        }

        public static object Parse(string src) => new SigoParser(src).Parse();
    }
}