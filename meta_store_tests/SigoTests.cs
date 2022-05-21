using System.Collections;
using System.Collections.Generic;
using meta_store;
using NUnit.Framework;

namespace meta_store_tests
{
    public class SigoTests
    {
        private object name;
        private object user;
        private IReadOnlyDictionary<string, object> expectedName;
        private IReadOnlyDictionary<string, object> expectedUser;


        [SetUp]
        public void Setup()
        {
            name = Sigo.Create(
                "first", "phat",
                "last", "dam");

            user = Sigo.Create(
                "name", name,
                "age", 40);

            expectedName = new Dictionary<string, object>
            {
                {"first", "phat"},
                {"last", "dam"},
            };

            expectedUser = new Dictionary<string, object>
            {
                {"name", name},
                {"age", 40},
            };
        }

        [Test]
        public void Create()
        {
            Assert.AreEqual(expectedName, name);

        }

        [Test]
        public void KeyValidation()
        {
            Sigo.KeyValidate("a_key");
            Assert.Catch(() => Sigo.KeyValidate(null));
            Assert.Catch(() => Sigo.KeyValidate(""));
            Assert.Catch(() => Sigo.KeyValidate("a/b"));
        }

        [Test]
        public void Get1()
        {
            Assert.AreEqual(null, Sigo.Get1("a_leaf", "a_key"));
            Assert.AreEqual("phat", Sigo.Get1(name, "first"));
            Assert.AreEqual(null, Sigo.Get1(name, "key_not_exist"));
        }

        [Test]
        public void Set1_not_freeze()
        {
            var s1 = Sigo.Set1(null, "k1", "v1");
            Assert.AreEqual(new Dictionary<string, object> { { "k1", "v1" }}, s1);
            
            var s2 = Sigo.Set1(s1, "k2", "v2");
            Assert.AreEqual(new Dictionary<string, object> { { "k1", "v1" }, { "k2", "v2" } }, s2);

            Assert.AreSame(s1, s2);
        }

        [Test]
        public void Set1_freeze_add()
        {
            var s1 = Sigo.Set1(null, "k1", "v1");
            Assert.AreEqual(new Dictionary<string, object> { { "k1", "v1" } }, s1);

            Sigo.Freeze(s1);
            Assert.IsTrue(Sigo.IsFrozen(s1));

            var s2 = Sigo.Set1(s1, "k2", "v2");
            Assert.AreEqual(new Dictionary<string, object> { { "k1", "v1" }, { "k2", "v2" } }, s2);

            Assert.AreNotSame(s1, s2);

            Assert.IsTrue(Sigo.IsFrozen(s1));
            Assert.IsFalse(Sigo.IsFrozen(s2));
        }

        [Test]
        public void Set1_freeze_change()
        {
            var s1 = Sigo.Set1(null, "k1", "v1");
            Assert.AreEqual(new Dictionary<string, object> { { "k1", "v1" } }, s1);

            Sigo.Freeze(s1);

            var s2 = Sigo.Set1(s1, "k1", "v1_changed");
            Assert.AreEqual(new Dictionary<string, object> { { "k1", "v1_changed" }}, s2);

            Assert.AreNotSame(s1, s2);

            Assert.IsTrue(Sigo.IsFrozen(s1));
            Assert.IsFalse(Sigo.IsFrozen(s2));
        }

        [Test]
        public void Split()
        {
            var a = "".Split('/');
            var b = "a/".Split('/');
            var c = "a/b".Split('/');
        }

        [Test]
        public void Get()
        {
            Assert.AreEqual(user, Sigo.Get(user, ""));
            Assert.AreEqual(name, Sigo.Get(user, "name"));
            Assert.AreEqual("phat", Sigo.Get(user, "name/first"));
            Assert.AreEqual(null, Sigo.Get(user, "name/first/not_a_key"));
            Assert.AreEqual(null, Sigo.Get(user, "name/not_a_key"));
        }

        [Test]
        public void Set()
        {
            var expected = new Dictionary<string, object> {{
                "name", new Dictionary<string, object> {{"first", "phat"}}
            }};

            var result = Sigo.Set(null, "name/first", "phat");

            Assert.AreEqual(expected, result);
        }        
    }
}