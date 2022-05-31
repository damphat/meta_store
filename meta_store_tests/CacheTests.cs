using meta_store;
using NUnit.Framework;

namespace meta_store_tests
{
    public class CacheTests
    {
        [Test]
        public void Test1()
        {
            var c = new Cache();
            c.Push();
            c.Add("x", 1);
        }
    }
}
