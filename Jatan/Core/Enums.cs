using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Core
{
    /// <summary>
    /// The corner of a hexagon that is pointed on the top and bottom.
    /// </summary>
    public enum PointDir
    {
        BottomRight = 0,
        Bottom,
        BottomLeft,
        TopLeft,
        Top,
        TopRight,
    }

    /// <summary>
    /// The side of a hexagon that is pointed on the top and bottom.
    /// </summary>
    public enum EdgeDir
    {
        Right = 0,
        BottomRight,
        BottomLeft,
        Left,
        TopLeft,
        TopRight
    }
}
