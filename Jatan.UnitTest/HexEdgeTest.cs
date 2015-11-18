using System;
using Jatan.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jatan.UnitTest
{
    [TestClass]
    public class HexEdgeTest
    {
        [TestMethod]
        public void TestGetPoints()
        {
            var hex = new HexEdge(0, 0, 1, 0);
            var points = hex.GetPoints();
            Assert.AreEqual(2, points.Count, "A hex edge must have 2 points.");

            var msg = "Hex edge is missing a point.";
            Assert.IsTrue(points.Contains(new HexPoint(0, 0, 1, 0, 1, 1)), msg);
            Assert.IsTrue(points.Contains(new HexPoint(0, 0, 1, 0, 0, -1)), msg);
        }

        [TestMethod]
        public void TestGetNeighborEdges()
        {
            var hex = new HexEdge(0, 0, 1, 0);
            var neighbors = hex.GetNeighborEdges();
            Assert.AreEqual(4, neighbors.Count, "A hex edge must have 4 neighbors.");

            var msg = "Hex edge is missing a neighbor.";
            Assert.IsTrue(neighbors.Contains(new HexEdge(0, 0, 0, -1)), msg);
            Assert.IsTrue(neighbors.Contains(new HexEdge(1, 0, 0, -1)), msg);
            Assert.IsTrue(neighbors.Contains(new HexEdge(0, 0, 1, 1)), msg);
            Assert.IsTrue(neighbors.Contains(new HexEdge(1, 0, 1, 1)), msg);
        }

        [TestMethod]
        public void TestGetEdges()
        {
            var hex = new HexEdge(0, 0, 1, 0);
            var edges = hex.GetHexagons();
            Assert.AreEqual(2, edges.Count, "A hex edge must have 2 hexagons.");

            string msg = "Hex edge is missing a hexagon.";
            Assert.IsTrue(edges.Contains(new Hexagon(0, 0)), msg);
            Assert.IsTrue(edges.Contains(new Hexagon(1, 0)), msg);
        }

        [TestMethod]
        public void TestSerialize()
        {
            var edges = new HexEdge[]
            {
                new HexEdge(0, 0, 1, 0),
                new HexEdge(-2, -2, -1, -1),
                new HexEdge(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue - 1),
                new HexEdge(int.MinValue, int.MinValue, int.MinValue, int.MinValue + 1),
            };

            foreach (var edge in edges)
            {
                var str = edge.ToString();
                var actual = new HexEdge();
                actual.FromString(str);
                Assert.AreEqual(edge, actual, "The FromString method must produce the exact object that called the ToString method.");
            }
        }
    }
}
