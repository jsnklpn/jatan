using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Jatan.Core.Serialization;

namespace Jatan.Core
{
    /// <summary>
    /// Struct to represent a hexagon location
    /// </summary>
    [TypeConverter(typeof(StringTypeConverter<Hexagon>))]
    public struct Hexagon : IStringSerializable
    {
        /// <summary>
        /// The X-cordinate
        /// </summary>
        public int X;

        /// <summary>
        /// The Y-cordinate
        /// </summary>
        public int Y;

        /// <summary>
        /// Creates a new hexagon at [x,y]
        /// </summary>
        public Hexagon(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets the hexagon at the origin
        /// </summary>
        public static Hexagon Zero
        {
            get { return new Hexagon(0, 0); }
        }

        /// <summary>
        /// Returns true if this hexagon is touching the given hexagon.
        /// </summary>
        public bool IsTouching(Hexagon hex2)
        {
            if (this.X == hex2.X) return Math.Abs(hex2.Y - this.Y) == 1;
            if (this.Y == hex2.Y) return Math.Abs(hex2.X - this.X) == 1;
            return ((hex2.X == this.X + 1 && hex2.Y == this.Y + 1) ||
                    (hex2.X == this.X - 1 && hex2.Y == this.Y - 1));
        }

        /// <summary>
        /// Gets a list of all neighboring hexagons.
        /// </summary>
        public IList<Hexagon> GetNeighbors()
        {
            return new List<Hexagon>()
            {
                new Hexagon(X + 1, Y),
                new Hexagon(X, Y - 1),
                new Hexagon(X - 1, Y - 1),
                new Hexagon(X - 1, Y),
                new Hexagon(X, Y + 1),
                new Hexagon(X + 1, Y + 1),
            };
        }

        /// <summary>
        /// Gets a list of all the hexagon edges.
        /// </summary>
        public IList<HexEdge> GetEdges()
        {
            var neighbors = GetNeighbors();
            var hex1 = this;
            return neighbors.Select(hex2 => new HexEdge(hex1, hex2)).ToList();
        }

        /// <summary>
        /// Gets the hexagon edge that is in the specified direction.
        /// </summary>
        public HexEdge GetEdge(EdgeDir direction)
        {
            return GetEdges()[(int)direction];
        }

        /// <summary>
        /// Gets a list of all the hexagon points.
        /// </summary>
        public IList<HexPoint> GetPoints()
        {
            var points = new List<HexPoint>();
            var neighbors = GetNeighbors();
            var hex1 = this;
            for (int i = 0; i < 6; i++)
            {
                var next = (i + 1) % 6;
                points.Add(new HexPoint(hex1, neighbors[i], neighbors[next]));
            }
            return points;
        }

        /// <summary>
        /// Gets the hexagon point that is in the specified direction.
        /// </summary>
        public HexPoint GetPoint(PointDir direction)
        {
            return GetPoints()[(int)direction];
        }

        /// <summary>
        /// Indicates if the hexagon contains the given edge.
        /// </summary>
        public bool ContainsEdge(HexEdge edge)
        {
            return GetEdges().Contains(edge);
        }

        /// <summary>
        /// Indicates if the hexagon contains the given point.
        /// </summary>
        public bool ContainsPoint(HexPoint point)
        {
            return GetPoints().Contains(point);
        }

        /// <summary>
        /// Returns the hexagon coordinates as a string.
        /// </summary>
        public override string ToString()
        {
            return string.Format("({0},{1})", X, Y);
        }

        /// <summary>
        /// Creates this object from a string.
        /// </summary>
        public void FromString(string value)
        {
            try
            {
                var split = value.Trim('(', ')', ' ').Split(',');
                X = int.Parse(split[0]);
                Y = int.Parse(split[1]);
            }
            catch (Exception)
            {
                throw new ArgumentException("Cannot parse Hexagon string. Must be in the format \"(X,Y)\"", "value");
            }
        }

        #region Equals

        public override bool Equals(object obj)
        {
            return obj is Hexagon && this == (Hexagon)obj;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(Hexagon a, Hexagon b)
        {
            return (a.X == b.X && a.Y == b.Y);
        }

        public static bool operator !=(Hexagon a, Hexagon b)
        {
            return !(a == b);
        }

        #endregion
    }
}
