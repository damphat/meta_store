using System.Collections.Generic;
using NUnit.Framework;

namespace meta_store_tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Create()
        {
            var s = Sigo.Create("name", "phat");

            Assert.AreEqual(new Dictionary<string, object> {{"name", "phat"}}, s);
        }

        [Test]
        public void Get1()
        {
            Assert.Pass();
        }

        [Test]
        public void Set1()
        {
            Assert.Pass();
        }

        [Test]
        public void Get()
        {
            Assert.Pass();
        }

        [Test]
        public void Set()
        {
            Assert.Pass();
        }

        [Test]
        public void XXX()
        {
            Assert.Pass();
        }
    }
}