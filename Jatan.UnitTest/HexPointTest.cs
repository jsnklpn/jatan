using System;
using System.Collections.Generic;
using System.Linq;
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

        [TestMethod]
        public void TestNeighboringHexagonY0_SharesTwoHexPoints()
        {
            var hexagon1 = new Hexagon(0, 0);
            var neighbor = new Hexagon(1, 0);
            var allPoints = new HashSet<HexPoint>();

            hexagon1.GetPoints().ToList().ForEach(hp => allPoints.Add(hp));
            neighbor.GetPoints().ToList().ForEach(hp => allPoints.Add(hp));

            Assert.AreEqual(10, allPoints.Count, "Expect neighboring hexagons on even Y axis to share 2 HexPoints");
        }

        [TestMethod]
        public void TestNeighboringHexagonY1_SharesTwoHexPoints()
        {
            var hexagon1 = new Hexagon(0, 1);
            var neighbor = new Hexagon(1, 1);
            var allPoints = new HashSet<HexPoint>();

            hexagon1.GetPoints().ToList().ForEach(hp => allPoints.Add(hp));
            neighbor.GetPoints().ToList().ForEach(hp => allPoints.Add(hp));

            Assert.AreEqual(10, allPoints.Count, "Expect neighboring hexagons on uneven Y axis to share 2 HexPoints");
        }

    }
}
