using meta_store;
using NUnit.Framework;

namespace meta_store_tests
{
    public class StoreListenerTests
    {
        [Test]
        public void Test_listenersCount()
        {
            static void h1(object v) { }

            static void h2(object v) { }

            var root = new Store();
            Assert.AreEqual(0, root.listenerCount);
            root.AddListener(h1);
            Assert.AreEqual(1, root.listenerCount);
            root.AddListener(h1);
            Assert.AreEqual(1, root.listenerCount);

            root.AddListener(h2);
            Assert.AreEqual(2, root.listenerCount);

            root.RemoveListener(h1);
            Assert.AreEqual(1, root.listenerCount);
            root.RemoveListener(h2);
            Assert.AreEqual(0, root.listenerCount);
            root.RemoveListener(h2);
            Assert.AreEqual(0, root.listenerCount);
        }

        [Test]
        public void Test_listenersCount_name()
        {
            static void h1(object v) { }

            static void h2(object v) { }

            var root = new Store();
            var name = root.At("user/name");
            Assert.AreEqual(0, root.listenerCount);
            name.AddListener(h1);
            Assert.AreEqual(1, root.listenerCount);
            name.AddListener(h1);
            Assert.AreEqual(1, root.listenerCount);

            name.AddListener(h2);
            Assert.AreEqual(2, root.listenerCount);

            name.RemoveListener(h1);
            Assert.AreEqual(1, root.listenerCount);
            name.RemoveListener(h2);
            Assert.AreEqual(0, root.listenerCount);
            name.RemoveListener(h2);
            Assert.AreEqual(0, root.listenerCount);
        }

        [Test]
        public void Test_dirty()
        {
            var root = new Store();
            Assert.AreEqual(0, root.dirty);
            
            // null to null
            root.Set(null);
            Assert.AreEqual(0, root.dirty);

            // null to 1
            root.Set("1");
            Assert.AreEqual(1, root.dirty);

            // TODO null to 1 then to 2, why not dirty should be still 1?
            root.Set("2");
            Assert.AreEqual(2, root.dirty);

            // 2 to 2
            root.Set("2");
            Assert.AreEqual(2, root.dirty);

            // TODO 2 to null (original value)
            root.Set(null);
            Assert.AreEqual(3, root.dirty);
        }

        [Test]
        public void Test_dirty_down()
        {
            var root = new Store();
            var name = root.At("name");
            Assert.AreEqual(0, root.dirty);
            name.Set(null);
            Assert.AreEqual(0, root.dirty);
            name.Set("1");
            Assert.AreEqual(1, root.dirty);
            name.Set("2");
            Assert.AreEqual(2, root.dirty);
        }

        [Test]
        public void Test_dirty_up()
        {
            var root = new Store();
            var name = root.At("name");
            Assert.AreEqual(0, name.dirty);

            name.Set(Sigo.State("name", null));
            Assert.AreEqual(0, name.dirty);

            name.Set(Sigo.State("name", "1"));
            Assert.AreEqual(1, name.dirty);

            name.Set(Sigo.State("name", "2"));
            Assert.AreEqual(2, name.dirty);
        }
    }
}
