using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Core
{
    /// <summary>
    /// A struct to represent a hexagon point
    /// </summary>
    public struct HexPoint
    {
        /// <summary>
        /// The first hexagon that the point is touching
        /// </summary>
        public readonly Hexagon Hex1;

        /// <summary>
        /// The second hexagon that the point is touching
        /// </summary>
        public readonly Hexagon Hex2;

        /// <summary>
        /// The third hexagon that the point is touching
        /// </summary>
        public readonly Hexagon Hex3;

        /// <summary>
        /// Creates a new hexagon point
        /// </summary>
        public HexPoint(int x1, int y1, int x2, int y2, int x3, int y3)
            : this(new Hexagon(x1, y1), new Hexagon(x2, y2), new Hexagon(x3, y3))
        {
        }

        /// <summary>
        /// Creates a new hexagon point
        /// </summary>
        public HexPoint(Hexagon hex1, Hexagon hex2, Hexagon hex3)
        {
            Hex1 = hex1;
            Hex2 = hex2;
            Hex3 = hex3;

            if (!hex1.IsTouching(hex2) || !hex2.IsTouching(hex3) || !hex1.IsTouching(hex3))
                throw new ArgumentException(string.Format("Invalid hexagon point: {0}.", this));
        }

        /// <summary>
        /// Returns the three hexes as a list.
        /// </summary>
        public IList<Hexagon> GetHexes()
        {
            return new List<Hexagon>() { Hex1, Hex2, Hex3 };
        }

        /// <summary>
        /// Gets the three surrounding points.
        /// </summary>
        public IList<HexPoint> GetNeighborPoints()
        {
            var thisPoint = this;
            var edges = GetNeighborEdges();
            var allPoints = new List<HexPoint>();
            foreach (var edge in edges)
            {
                allPoints.AddRange(edge.GetPoints());
            }
            var neighborPoints = allPoints.Where(p => p != thisPoint).ToList();
            System.Diagnostics.Debug.Assert(neighborPoints.Count == 3, "A point should have exactly 3 neighbor points.");
            return neighborPoints;
        }

        /// <summary>
        /// Gets the three edges touching this point.
        /// </summary>
        public IList<HexEdge> GetNeighborEdges()
        {
            return new List<HexEdge>()
            {
                new HexEdge(Hex1, Hex2),
                new HexEdge(Hex1, Hex3),
                new HexEdge(Hex2, Hex3)
            };
        }

        /// <summary>
        /// Returns the hexagon point coordinates as a string.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{{{0}, {1}, {2}}}", Hex1, Hex2, Hex3);
        }

        #region Equals

        public override bool Equals(object obj)
        {
            return obj is HexPoint && this == (HexPoint)obj;
        }

        public override int GetHashCode()
        {
            return Hex1.GetHashCode() ^ Hex2.GetHashCode() ^ Hex3.GetHashCode();
        }

        public static bool operator ==(HexPoint a, HexPoint b)
        {
            return ((a.Hex1 == b.Hex1 && a.Hex2 == b.Hex2 && a.Hex3 == b.Hex3) ||
                    (a.Hex1 == b.Hex1 && a.Hex2 == b.Hex3 && a.Hex3 == b.Hex2) ||
                    (a.Hex1 == b.Hex2 && a.Hex2 == b.Hex1 && a.Hex3 == b.Hex3) ||
                    (a.Hex1 == b.Hex2 && a.Hex2 == b.Hex3 && a.Hex3 == b.Hex1) ||
                    (a.Hex1 == b.Hex3 && a.Hex2 == b.Hex1 && a.Hex3 == b.Hex2) ||
                    (a.Hex1 == b.Hex3 && a.Hex2 == b.Hex2 && a.Hex3 == b.Hex1));
        }

        public static bool operator !=(HexPoint a, HexPoint b)
        {
            return !(a == b);
        }

        #endregion
    }
}
