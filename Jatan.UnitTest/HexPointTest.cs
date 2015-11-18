using System;
using Jatan.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jatan.UnitTest
{
    [TestClass]
    public class HexPointTest
    {
        [TestMethod]
        public void TestSerialize()
        {
            var points = new HexPoint[]
            {
                new HexPoint(0, 0, 1, 0, 1, 1),
                new HexPoint(-2, -2, -2, -1, -1, -1),
                new HexPoint(int.MaxValue - 1, int.MaxValue - 1, int.MaxValue, int.MaxValue - 1, int.MaxValue, int.MaxValue)
            };

            foreach (var point in points)
            {
                var str = point.ToString();
                var actual = new HexPoint();
                actual.FromString(str);
                Assert.AreEqual(point, actual, "The FromString method must produce the exact object that called the ToString method.");
            }
        }
    }
}
