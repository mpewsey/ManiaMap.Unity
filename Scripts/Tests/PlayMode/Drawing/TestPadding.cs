using NUnit.Framework;

namespace MPewsey.ManiaMap.Unity.Drawing.Tests
{
    public class TestPadding
    {
        [Test]
        public void TestInit()
        {
            var x = new Padding(100);
            Assert.AreEqual(100, x.Top);
            Assert.AreEqual(100, x.Bottom);
            Assert.AreEqual(100, x.Left);
            Assert.AreEqual(100, x.Right);

            var y = new Padding(100, 200, 300, 400);
            Assert.AreEqual(100, y.Left);
            Assert.AreEqual(200, y.Top);
            Assert.AreEqual(300, y.Right);
            Assert.AreEqual(400, y.Bottom);
        }

        [Test]
        public void TestToString()
        {
            var x = new Padding(1);
            Assert.IsTrue(x.ToString().StartsWith("Padding("));
        }

        [Test]
        public void TestGetHashCode()
        {
            var x = new Padding(1);
            var y = new Padding(1);
            var z = new Padding(2);
            Assert.AreEqual(x.GetHashCode(), y.GetHashCode());
            Assert.AreNotEqual(x.GetHashCode(), z.GetHashCode());
        }

        [Test]
        public void TestEquals()
        {
            var x = new Padding(1);
            Assert.IsFalse(x.Equals(null));
        }

        [Test]
        public void TestEqualsOperator()
        {
            var x = new Padding(1);
            var y = new Padding(1);
            var z = new Padding(2);
            Assert.IsTrue(x == y);
            Assert.IsFalse(x == z);
        }

        [Test]
        public void TestDoesNotEqualOperator()
        {
            var x = new Padding(1);
            var y = new Padding(1);
            var z = new Padding(2);
            Assert.IsTrue(x != z);
            Assert.IsFalse(x != y);
        }
    }
}
