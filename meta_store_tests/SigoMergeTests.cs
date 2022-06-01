using meta_store;
using NUnit.Framework;

namespace meta_store_tests
{
    public class SigoMergeTests
    {
        [Test]
        public void EqualsTest()
        {
            Assert.True(Sigo.Equals(1, 1));
            Assert.True(Sigo.Equals(Sigo.State(), Sigo.State()));
            Assert.True(Sigo.Equals(Sigo.State("x", 1, "y", 2), Sigo.State("y", 2, "x", 1)));
            Assert.True(Sigo.Equals(Sigo.Update(), Sigo.Update()));
            Assert.True(Sigo.Equals(Sigo.Update("x", 1, "y", 2), Sigo.Update("y", 2, "x", 1)));
        }

        [Test]
        public void NotEqualsTest()
        {
            Assert.False(Sigo.Equals(1, 2));
            Assert.False(Sigo.Equals(Sigo.State(), Sigo.Update()));
            Assert.False(Sigo.Equals(Sigo.State("x", 1), Sigo.Update("x", 1)));
            Assert.False(Sigo.Equals(Sigo.State("x", 1, "y", 1), Sigo.State("x", 1, "y", 1, "z", 1)));
            Assert.False(Sigo.Equals(Sigo.State("x", 1), Sigo.State("x", 2)));
            Assert.False(Sigo.Equals(Sigo.State("x", 1), Sigo.State("y", 1)));
        }

        [Test]
        public void UpdateTest()
        {
            var state = Sigo.State("name", "phat");
            var update = Sigo.Update("name", "phat");

            Assert.AreEqual(false, Sigo.IsUpdate(state));
            Assert.AreEqual(false, Sigo.IsUpdate(1));

            Assert.AreEqual(true, Sigo.IsUpdate(update));
        }

        [Test]
        public void MergeTest_xb_return_b()
        {
            Assert.AreEqual("b", Sigo.Merge("a", "b"));
            Assert.AreEqual("b", Sigo.Merge(Sigo.State(), "b"));
            Assert.AreEqual("b", Sigo.Merge(Sigo.Update(), "b"));
            Assert.AreEqual("b", Sigo.Merge(Sigo.State("name", "phat"), "b"));
            Assert.AreEqual("b", Sigo.Merge(Sigo.Update("name", "phat"), "b"));
        }

        [Test]
        public void MergeTest_xs_return_s()
        {
            var s = Sigo.State("name", "phat");

            Assert.AreEqual(s, Sigo.Merge("a", s));
            Assert.AreEqual(s, Sigo.Merge(Sigo.State(), s));
            Assert.AreEqual(s, Sigo.Merge(Sigo.Update(), s));
            Assert.AreEqual(s, Sigo.Merge(Sigo.State("name", "phat"), s));
            Assert.AreEqual(s, Sigo.Merge(Sigo.Update("name", "phat"), s));
        }

        [Test]
        public void MergeTest_pu()
        {
            Assert.AreEqual("a", Sigo.Merge("a", Sigo.Update()));

            Assert.True(Sigo.Equals(
                Sigo.State("name", "phat"),
                Sigo.Merge("a", Sigo.Update("name", "phat"))
                )
            );
        }

        [Test]
        public void MergeTest_state_keep() =>
            // keep
            Assert.True(Sigo.Equals(
                    Sigo.State("x", 1),
                    Sigo.Merge(Sigo.State("x", 1), Sigo.Update("x", 1))
                )
            );

        [Test]
        public void MergeTest_state_change() =>
            // change
            Assert.True(Sigo.Equals(
                    Sigo.State("x", 2),
                    Sigo.Merge(Sigo.State("x", 1), Sigo.Update("x", 2))
                )
            );

        [Test]
        public void MergeTest_state_add() =>
            // add
            Assert.True(Sigo.Equals(
                    Sigo.State("x", 1, "y", 1),
                    Sigo.Merge(Sigo.State("x", 1), Sigo.Update("y", 1))
                )
            );

        [Test]
        public void MergeTest_state_remove() =>
            // remove
            Assert.True(Sigo.Equals(
                    Sigo.State("x", 1),
                    Sigo.Merge(Sigo.State("x", 1, "y", 1), Sigo.Update("y", Sigo.State()))
                )
            );

        [Test]
        public void MergeTest_update_keep() =>
            // keep
            Assert.True(Sigo.Equals(
                    Sigo.Update("x", 1),
                    Sigo.Merge(Sigo.Update("x", 1), Sigo.Update("x", 1))
                )
            );

        [Test]
        public void MergeTest_update_change() =>
            // change
            Assert.True(Sigo.Equals(
                    Sigo.Update("x", 2),
                    Sigo.Merge(Sigo.Update("x", 1), Sigo.Update("x", 2))
                )
            );

        [Test]
        public void MergeTest_update_add() =>
            // add
            Assert.True(Sigo.Equals(
                    Sigo.Update("x", 1, "y", 1),
                    Sigo.Merge(Sigo.Update("x", 1), Sigo.Update("y", 1))
                )
            );

        [Test]
        public void MergeTest_update_remove() =>
            // remove
            Assert.True(Sigo.Equals(
                    Sigo.Update("x", 1, "y", Sigo.Create(7)),
                    Sigo.Merge(Sigo.Update("x", 1, "y", 1), Sigo.Update("y", Sigo.Delete()))
                )
            );
    }
}
