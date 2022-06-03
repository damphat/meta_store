
using meta_store;
using NUnit.Framework;

namespace meta_store_tests
{
    public class ParseTests
    {
        [Test]
        public void ParseNumber()
        {
            Assert.AreEqual(1, Sigo.Parse("1"));            
            Assert.AreEqual(1.5, Sigo.Parse("1.5"));
            Assert.AreEqual(+1, Sigo.Parse("+1"));
            Assert.AreEqual(+1.5, Sigo.Parse("+1.5"));
            Assert.AreEqual(-1, Sigo.Parse("-1"));
            Assert.AreEqual(-1.5, Sigo.Parse("-1.5"));            
        }

        [Test]
        public void ParseNumber_types()
        {
            Assert.AreEqual(typeof(int), Sigo.Parse("1").GetType());
            Assert.AreEqual(typeof(float), Sigo.Parse("1.0").GetType());
            Assert.AreEqual(typeof(float), Sigo.Parse("1e0").GetType());

            Assert.AreEqual(typeof(int), Sigo.Parse("1i").GetType());
            Assert.AreEqual(typeof(uint), Sigo.Parse("1u").GetType());
            Assert.AreEqual(typeof(float), Sigo.Parse("1f").GetType());

            Assert.AreEqual(typeof(long), Sigo.Parse("1i8").GetType());
            Assert.AreEqual(typeof(ulong), Sigo.Parse("1u8").GetType());
            Assert.AreEqual(typeof(double), Sigo.Parse("1d").GetType());
        }

        [Test]
        public void ParseNumber_Infinity()
        {
            Assert.AreEqual(typeof(float), Sigo.Parse("NaN").GetType());
            Assert.AreEqual(typeof(float), Sigo.Parse("+Infinity").GetType());
            Assert.AreEqual(typeof(float), Sigo.Parse("-Infinity").GetType());
        }
    }
}
