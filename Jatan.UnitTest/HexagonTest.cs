using System;
using System.Linq;
using Jatan.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jatan.UnitTest
{
    [TestClass]
    public class HexagonTest
    {
        [TestMethod]
        public void TestGetPoints()
        {
            var hex = new Hexagon(0, 0);
            var points = hex.GetPoints();
            Assert.AreEqual(6, points.Count, "A hexagon must have 6 points.");

            var msg = "Hexagon is missing a point.";
            Assert.IsTrue(points.Contains(new HexPoint(0, 0, 1, 1, 1, 0)), msg);
            Assert.IsTrue(points.Contains(new HexPoint(0, 0, 1, 0, 0, -1)), msg);
            Assert.IsTrue(points.Contains(new HexPoint(0, 0, 0, -1, -1, -1)), msg);
            Assert.IsTrue(points.Contains(new HexPoint(0, 0, -1, -1, -1, 0)), msg);
            Assert.IsTrue(points.Contains(new HexPoint(0, 0, -1, 0, 0, 1)), msg);
            Assert.IsTrue(points.Contains(new HexPoint(0, 0, 0, 1, 1, 1)), msg);
        }

        [TestMethod]
        public void TestGetNeighbors()
        {
            var hex = new Hexagon(0, 0);
            var neighbors = hex.GetNeighbors();
            Assert.AreEqual(6, neighbors.Count, "A hexagon must have 6 neighbors.");

            var msg = "Hexagon is missing a neighbor.";
            Assert.IsTrue(neighbors.Contains(new Hexagon(1, 1)), msg);
            Assert.IsTrue(neighbors.Contains(new Hexagon(1, 0)), msg);
            Assert.IsTrue(neighbors.Contains(new Hexagon(0, -1)), msg);
            Assert.IsTrue(neighbors.Contains(new Hexagon(-1, -1)), msg);
            Assert.IsTrue(neighbors.Contains(new Hexagon(-1, 0)), msg);
            Assert.IsTrue(neighbors.Contains(new Hexagon(0, 1)), msg);
        }

        [TestMethod]
        public void TestGetEdges()
        {
            var hex = new Hexagon(0, 0);
            var edges = hex.GetEdges();
            Assert.AreEqual(6, edges.Count, "A hexagon must have 6 edges.");

            string msg = "Hexagon is missing an edge.";
            Assert.IsTrue(edges.Contains(new HexEdge(0, 0, 1, 1)), msg);
            Assert.IsTrue(edges.Contains(new HexEdge(0, 0, 1, 0)), msg);
            Assert.IsTrue(edges.Contains(new HexEdge(0, 0, 0, -1)), msg);
            Assert.IsTrue(edges.Contains(new HexEdge(0, 0, -1, -1)), msg);
            Assert.IsTrue(edges.Contains(new HexEdge(0, 0, -1, 0)), msg);
            Assert.IsTrue(edges.Contains(new HexEdge(0, 0, 0, 1)), msg);
        }
    }
}
