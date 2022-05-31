using meta_store;
using NUnit.Framework;

namespace meta_store_tests
{
    public class StoreTests
    {
        [Test]
        public void Test_root()
        {
            var root = new Store();
            Assert.AreEqual(null, root.Key);
            Assert.AreEqual(null, root.Parent);
            Assert.AreEqual(root, root.Root);
            Assert.AreEqual("", root.Path);
        }

        [Test]
        public void Test_At_name()
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
        public void Test_At_name_first()
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

            Assert.AreEqual(Sigo.State("name", "phat"), root.Get());
        }

        [Test]
        public void Test_GetSet_name_first_down()
        {
            var root = new Store();
            var first = root.At("name/first");

            first.Set("phat");

            Assert.AreEqual("phat", first.Get());
            Assert.AreEqual(Sigo.State("first", "phat"), first.Parent.Get());

            Assert.AreEqual(Sigo.State("name", Sigo.State("first", "phat")), first.Root.Get());
        }

        [Test]
        public void Test_GetSet_name_first_up()
        {
            var root = new Store();
            var first = root.At("name/first");

            root.Set(Sigo.State("name", Sigo.State("first", "phat")));

            Assert.AreEqual("phat", first.Get());
            Assert.AreEqual(Sigo.State("first", "phat"), first.Parent.Get());
            Assert.AreEqual(Sigo.State("name", Sigo.State("first", "phat")), first.Root.Get());
        }

    }
}
