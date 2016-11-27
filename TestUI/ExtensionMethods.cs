using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUI
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Adds spaces to a CamelCase string.
        /// </summary>
        public static string CamelSpaces(this string str)
        {
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            str = str.Trim();
            var result = str[0].ToString();
            for (int i = 1; i < str.Length; i++)
            {
                if (uppercase.Contains(str[i]) && (!uppercase.Contains(str[i - 1]) || ((i != str.Length - 1) && !uppercase.Contains(str[i + 1]))))
                {
                    result += " ";
                }
                result += str[i];
            }
            return result;
        }

        /// <summary>
        /// Gets the luminance of a color.
        /// </summary>
        public static float GetLuminance(this Color color)
        {
            return (0.2126f * color.R + 0.7152f * color.G + 0.0722f * color.B);
        }

        /// <summary>
        /// Gets a bounding box surrounding a series of points.
        /// </summary>
        public static RectangleF GetBoundingBox(this IEnumerable<PointF> points, bool inflateIfTooThin = true)
        {
            float xMax = float.MinValue;
            float yMax = float.MinValue;
            float xMin = float.MaxValue;
            float yMin = float.MaxValue;
            foreach (var p in points)
            {
                if (p.X > xMax) xMax = p.X;
                if (p.Y > yMax) yMax = p.Y;
                if (p.X < xMin) xMin = p.X;
                if (p.Y < yMin) yMin = p.Y;
            }
            var width = xMax - xMin;
            var height = yMax - yMin;
            var x = xMin;
            var y = yMin;

            if (inflateIfTooThin)
            {
                // when the width or height is really small, set them equal to their partner length.
                if (width < 0.001f)
                {
                    width = height;
                    x -= width/2f; // recenter the box
                }
                else if (height < 0.001f)
                {
                    height = width;
                    y -= height/2f; // recenter the box
                }
            }

            return new RectangleF(x, y, width, height);
        }

        /// <summary>
        /// Adds an (x, y) offset to all points.
        /// </summary>
        public static PointF[] Offset(this PointF[] points, float x, float y)
        {
            var count = points.Count();
            PointF[] result = new PointF[count];
            for (int i = 0; i < count; i++)
            {
                var p = points[i];
                result[i] = new PointF(p.X + x, p.Y + y);
            }
            return result;
        }

        /// <summary>
        /// Gets a rect centered at this point
        /// </summary>
        public static RectangleF GetRect(this PointF point, float width, float height)
        {
            return new RectangleF(point.X - (width / 2f), point.Y - (height / 2f), width, height);
        }

        /// <summary>
        /// Gets the centerpoint of a rectangle
        /// </summary>
        public static PointF GetCenter(this RectangleF rectF)
        {
            return new PointF(rectF.X + (rectF.Width / 2f), rectF.Y + (rectF.Height / 2f));
        }

        /// <summary>
        /// Changes the size of a rectangle, keeping the current center point.
        /// </summary>
        public static RectangleF ChangeSize(this RectangleF rectF, float width, float height)
        {
            return GetRect(GetCenter(rectF), width, height);
        }

        /// <summary>
        /// Changes the size of a rectangle, keeping the current center point.
        /// </summary>
        public static RectangleF OffsetSize(this RectangleF rectF, float x, float y)
        {
            return ChangeSize(rectF, rectF.Width + x, rectF.Height + y);
        }

        /// <summary>
        /// Changes the size of a rectangle, keeping the current center point.
        /// </summary>
        public static RectangleF ChangeScale(this RectangleF rectF, float xScale, float yScale)
        {
            return GetRect(GetCenter(rectF), rectF.X * xScale, rectF.Y * yScale);
        }

        /// <summary>
        /// Changes the size of a rectangle, keeping the current center point.
        /// </summary>
        public static RectangleF ChangeScale(this RectangleF rectF, float scale)
        {
            return ChangeScale(rectF, scale, scale);
        }

        /// <summary>
        /// Converts to a Rectangle
        /// </summary>
        public static Rectangle ToRectangle(this RectangleF rectF)
        {
            return new Rectangle((int)rectF.X, (int)rectF.Y, (int)rectF.Width, (int)rectF.Height);
        }
    }
}
