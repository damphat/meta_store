using meta_store;
using NUnit.Framework;

namespace meta_store_tests
{
    public class FromTests
    {
        [Test]
        public void Test1()
        {
            Assert.AreEqual("abc", Sigo.From("abc"));
        }

        [Test]
        public void Test2()
        {
            var a = Sigo.State("x", 1, "y", 1);
            Assert.AreSame(a, Sigo.From(a));
        }

        [Test]
        public void Test3()
        {
            var a = Sigo.State("x", 1, "y", 1);
            Assert.AreSame(a, Sigo.From(a));
        }
    }
}
