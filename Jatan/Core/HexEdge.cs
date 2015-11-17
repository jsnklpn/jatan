using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Core
{
    /// <summary>
    /// A struct to represent a hexagon edge
    /// </summary>
    public struct HexEdge
    {
        /// <summary>
        /// The first hexagon that the edge is touching
        /// </summary>
        public readonly Hexagon Hex1;

        /// <summary>
        /// The second hexagon that the edge is touching
        /// </summary>
        public readonly Hexagon Hex2;

        /// <summary>
        /// Creates a new hexagon edge
        /// </summary>
        public HexEdge(int x1, int y1, int x2, int y2)
            : this(new Hexagon(x1, y1), new Hexagon(x2, y2))
        {
        }

        /// <summary>
        /// Creates a new hexagon edge
        /// </summary>
        public HexEdge(Hexagon hex1, Hexagon hex2)
        {
            Hex1 = hex1;
            Hex2 = hex2;

            if (!hex1.IsTouching(hex2))
                throw new ArgumentException(string.Format("Invalid hexagon edge: {0}.", this));
        }

        /// <summary>
        /// Returns the hexagons as a list.
        /// </summary>
        public IList<Hexagon> GetHexagons()
        {
            return new List<Hexagon>() { Hex1, Hex2 };
        }

        /// <summary>
        /// Gets the two points which make up this edge.
        /// </summary>
        public IList<HexPoint> GetPoints()
        {
            var points1 = Hex1.GetPoints();
            var points2 = Hex2.GetPoints();
            var result = points1.Intersect(points2).ToList();
            System.Diagnostics.Debug.Assert(result.Count == 2, "A hexagon edge can have only 2 points.");
            return result;
        }

        /// <summary>
        /// Gets the 4 neighbor hexagon edges.
        /// </summary>
        public IList<HexEdge> GetNeighborEdges()
        {
            var result = new List<HexEdge>();
            var myPoints = GetPoints();
            var thisEdge = this;

            var p = myPoints[0];
            var otherHex = p.GetHexes().Single(h => thisEdge.Hex1 != h && thisEdge.Hex2 != h);
            result.Add(new HexEdge(thisEdge.Hex1, otherHex));
            result.Add(new HexEdge(thisEdge.Hex2, otherHex));

            p = myPoints[1];
            otherHex = p.GetHexes().Single(h => thisEdge.Hex1 != h && thisEdge.Hex2 != h);
            result.Add(new HexEdge(thisEdge.Hex1, otherHex));
            result.Add(new HexEdge(thisEdge.Hex2, otherHex));

            System.Diagnostics.Debug.Assert(result.Count == 4, "There must be exactly 4 edge neighbors.");
            return result;
        }

        /// <summary>
        /// Indicates if the edge contains the given point.
        /// </summary>
        public bool ContainsPoint(HexPoint point)
        {
            return GetPoints().Contains(point);
        }

        /// <summary>
        /// Indicates if this edge is touching another edge.
        /// </summary>
        public bool IsTouching(HexEdge edge)
        {
            return GetNeighborEdges().Contains(edge);
        }

        /// <summary>
        /// Returns the hexagon edge coordinates as a string.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{{{0}, {1}}}", Hex1, Hex2);
        }

        #region Equals

        public override bool Equals(object obj)
        {
            return obj is HexEdge && this == (HexEdge)obj;
        }

        public override int GetHashCode()
        {
            return Hex1.GetHashCode() ^ Hex2.GetHashCode();
        }

        public static bool operator ==(HexEdge a, HexEdge b)
        {
            return ((a.Hex1 == b.Hex1 && a.Hex2 == b.Hex2) || (a.Hex1 == b.Hex2 && a.Hex2 == b.Hex1));
        }

        public static bool operator !=(HexEdge a, HexEdge b)
        {
            return !(a == b);
        }

        #endregion
    }
}
