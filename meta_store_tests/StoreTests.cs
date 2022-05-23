using System.Collections.Generic;
using meta_store;
using Microsoft.VisualBasic;
using NUnit.Framework;

namespace meta_store_tests
{
    public class StoreTests
    {
        [Test]
        public void Root()
        {
            var root = new Store();
            Assert.AreEqual(null, root.Key);
            Assert.AreEqual(null, root.Parent);
            Assert.AreEqual(root, root.Root);
            Assert.AreEqual("", root.Path);
        }

        [Test]
        public void Test_name()
        {
            var root = new Store();
            var name = root.At("name");
            Assert.AreSame(root.At("name"), name);

            Assert.AreEqual("name", name.Key);
            Assert.AreSame(root, name.Parent);
            Assert.AreSame(root, name.Root);
            Assert.AreEqual("name", name.Path);
        }

        [Test]
        public void Test_name_first()
        {
            var root = new Store();
            var first = root.At("name/first");
            Assert.AreSame(root.At("name").At("first"), first);

            Assert.AreEqual("first", first.Key);
            Assert.AreSame(root.At("name"), first.Parent);
            Assert.AreSame(root, first.Root);
            Assert.AreEqual("name/first", first.Path);
        }

        [Test]
        public void Test_GetSet_root()
        {
            var root = new Store();
            Assert.AreEqual(null, root.Get());

            root.Set("v");
            Assert.AreEqual("v", root.Get());
        }

        [Test]
        public void Test_GetSet_name()
        {
            var root = new Store();
            var name = root.At("name");

            name.Set("phat");

            Assert.AreEqual("phat", name.Get());

            Assert.AreEqual(Sigo.Create("name", "phat"), root.Get());
        }

        [Test]
        public void Test_GetSet_name_first_down()
        {
            var root = new Store();
            var first = root.At("name/first");

            first.Set("phat");

            Assert.AreEqual("phat", first.Get());
            Assert.AreEqual(Sigo.Create("first", "phat"), first.Parent.Get());

            Assert.AreEqual(Sigo.Create("name", Sigo.Create("first", "phat")), first.Root.Get());
        }

        [Test]
        public void Test_GetSet_name_first_up()
        {
            var root = new Store();
            var first = root.At("name/first");

            root.Set(Sigo.Create("name", Sigo.Create("first", "phat")));

            Assert.AreEqual("phat", first.Get());
            Assert.AreEqual(Sigo.Create("first", "phat"), first.Parent.Get());
            Assert.AreEqual(Sigo.Create("name", Sigo.Create("first", "phat")), first.Root.Get());
        }

        [Test]
        public void Test_listeners()
        {
            var root = new Store();
            root.Set("1");
            var ret = new List<object>();
            
            root.AddListener(v => ret.Add(v));

            Assert.AreEqual(new [] {"1"}, ret);
        }
    }
}
