using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jatan.Core;
using Jatan.Models;

namespace TestUI
{
    public partial class GameBoardViewer : UserControl
    {
        // We'll imagine a 7x7 grid. Sizes are actually percentages of the total size.
        private const float RelativeTileWidth = 100 / 7f;
        public static readonly Dictionary<ResourceTypes, Color> ResourceColorMap;
        private Dictionary<Hexagon, PointF[]> _hexPointMap;

        public GameBoard GameBoard { get; set; }

        public event EventHandler<Hexagon> HexagonClicked;
        public event EventHandler<HexEdge> HexEdgeClicked;
        public event EventHandler<HexPoint> HexPointClicked;
        
        private static class BoardColors
        {
            public static readonly Color Brick = Color.Firebrick;
            public static readonly Color Wood = Color.DarkGreen;
            public static readonly Color Wheat = Color.Gold;
            public static readonly Color Sheep = Color.GreenYellow;
            public static readonly Color Ore = Color.DimGray;
            public static readonly Color Desert = Color.AntiqueWhite;
            public static readonly Color Water = Color.LightBlue;
        }

        static GameBoardViewer()
        {
            ResourceColorMap = new Dictionary<ResourceTypes, Color>
            {
                {ResourceTypes.Brick, BoardColors.Brick},
                {ResourceTypes.Wood, BoardColors.Wood},
                {ResourceTypes.Wheat, BoardColors.Wheat},
                {ResourceTypes.Sheep, BoardColors.Sheep},
                {ResourceTypes.Ore, BoardColors.Ore},
                {ResourceTypes.None, BoardColors.Desert}
            };
        }

        public GameBoardViewer()
        {
            InitializeComponent();

            _hexPointMap = new Dictionary<Hexagon, PointF[]>();
            this.BackColor = Color.AliceBlue;
            this.ForeColor = Color.Black;
            this.DoubleBuffered = true;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // The size has changed, so the point map is invalidated.
            _hexPointMap.Clear();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // TODO: Detect hex clicks
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            // When clicked, refresh the control.
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var gfx = e.Graphics;
            if (this.GameBoard == null)
            {
                gfx.Clear(this.BackColor);
                return;
            }

            gfx.Clear(BoardColors.Water);
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            if (this.GameBoard.ResourceTiles.Any())
            {
                // Draw ports
                foreach (var port in this.GameBoard.Ports)
                {
                    DrawPort(gfx, port.Key, port.Value);
                }
                // Draw resource tiles
                foreach (var tile in this.GameBoard.ResourceTiles)
                {
                    DrawResourceTile(gfx, tile.Key, tile.Value);
                }
                // Draw robber
                DrawRobber(gfx, this.GameBoard.RobberLocation);
                // Draw roads
                foreach (var road in this.GameBoard.Roads)
                {
                    DrawRoad(gfx, road.Key, road.Value);
                }
                // Draw buildings
                foreach (var building in this.GameBoard.Buildings)
                {
                    DrawBuilding(gfx, building.Key, building.Value);
                }
            }
        }

        private void DrawRoad(Graphics gfx, HexEdge edge, Road road)
        {
            var linePoints = HexEdgeToAbsolutePoints(edge);
            if (linePoints != null)
            {
                Pen p1 = new Pen(Color.Black, 5f);
                Pen p2 = new Pen(PlayerToColor(road.PlayerId), 4f);
                gfx.DrawLine(p1, linePoints[0], linePoints[1]);
                gfx.DrawLine(p2, linePoints[0], linePoints[1]);
                p1.Dispose();
                p2.Dispose();
            }
        }

        private void DrawBuilding(Graphics gfx, HexPoint point, Building building)
        {
            var centerPoint = HexPointToAbsolutePoint(point);
            if (centerPoint != PointF.Empty) // Should never draw a building at (0,0)...
            {
                Brush b = new SolidBrush(PlayerToColor(building.PlayerId));
                Pen p = new Pen(Color.Black, 1f);

                var relativeSize = (building.Type == BuildingTypes.Settlement) ? (RelativeTileWidth)*(1/5f) : (RelativeTileWidth)*(2/5f);
                var width = this.Width*relativeSize/100f;
                var height = this.Height*relativeSize/100f;
                var rect = centerPoint.GetRect(width, height);

                gfx.FillRectangle(b, rect);
                gfx.DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height);

                b.Dispose();
                p.Dispose();
            }
        }

        private void DrawResourceTile(Graphics gfx, Hexagon hex, ResourceTile resource)
        {
            Brush b = new SolidBrush(ResourceColorMap[resource.Resource]);
            Brush b2 = new SolidBrush(this.ForeColor);
            Pen p = new Pen(Color.Black, 1f);

            var hexPointsAbsolute = HexagonToAbsolutePoints(hex);
            gfx.FillPolygon(b, hexPointsAbsolute);
            gfx.DrawPolygon(p, hexPointsAbsolute);

            // Draw the dice number needed to collect
            var boundingBox = hexPointsAbsolute.GetBoundingBox();
            var centerOfBox = boundingBox.GetCenter();
            var size = RelativeToAbsolute(RelativeTileWidth / 2f, RelativeTileWidth / 2f);
            var wordsBox = centerOfBox.GetRect(size.X, size.Y);
            var textToDraw = string.Format("{0}\r\n({1},{2})", resource.RetrieveNumber, hex.X, hex.Y);
            gfx.DrawString(textToDraw, DefaultFont, b2, wordsBox);

            p.Dispose();
            b.Dispose();
            b2.Dispose();
        }

        private void DrawPort(Graphics gfx, HexEdge edge, Port port)
        {
            var linePoints = HexEdgeToAbsolutePoints(edge);
            if (linePoints != null)
            {
                Color c = port.Resource == ResourceTypes.None ? Color.Black : ResourceColorMap[port.Resource];
                Brush hb = new HatchBrush(HatchStyle.Wave, c, BoardColors.Water);
                Pen p = new Pen(c, 2f);
                Pen bp = new Pen(Color.Black, 1f);

                var box = linePoints.GetBoundingBox();
                gfx.DrawEllipse(bp, box.OffsetSize(2, 2));
                gfx.DrawEllipse(p, box);
                gfx.FillEllipse(hb, box);

                bp.Dispose();
                hb.Dispose();
                p.Dispose();
            }
        }

        private void DrawRobber(Graphics gfx, Hexagon hex)
        {
            var hexPointsAbsolute = HexagonToAbsolutePoints(hex);
            var boundingBox = hexPointsAbsolute.GetBoundingBox();
            var centerOfBox = boundingBox.GetCenter();
            var rect = centerOfBox.GetRect(8, 8);
            rect.Offset(RelativeToAbsolute(0, -RelativeTileWidth / 4));
            Brush b = new SolidBrush(Color.Black);
            gfx.FillEllipse(b, rect);
            //gfx.FillPie(b, rect.ToRectangle(), (4.712f - .25f), (.5f));
            b.Dispose();
        }

        private PointF[] GetIdentityHexagonPoints()
        {
            return new PointF[]
            {
                new PointF(0, 0.5f),
                new PointF(0.5f, 0.25f),
                new PointF(0.5f, -0.25f),
                new PointF(0, -0.5f),
                new PointF(-0.5f, -0.25f),
                new PointF(-0.5f, 0.25f),
            };
        }

        private Color PlayerToColor(int playerId)
        {
            switch (playerId)
            {
                case 0: return Color.Blue;
                case 1: return Color.Red;
                case 2: return Color.Green;
                case 3: return Color.White;
                case 4: return Color.Yellow;
                case 5: return Color.Orange;
                default: return Color.Black;
            }
        }

        private PointF[] GetRelativeHexegonPoints(Hexagon hex)
        {
            PointF[] points = GetIdentityHexagonPoints();
            var identityOffset = GetIdentityHexOffset(hex);
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X = (points[i].X + identityOffset.X) * RelativeTileWidth;
                points[i].Y = (points[i].Y + identityOffset.Y) * RelativeTileWidth;
            }
            return points;
        }

        private PointF[] ConvertRelativePointsToAbsolute(PointF[] points)
        {
            var newPoints = new PointF[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                // Offset by 50% so that 0,0 is actually in the center of the control.
                var newPoint = RelativeToAbsolute(50f + points[i].X, 50f + points[i].Y);
                newPoints[i] = newPoint;
            }
            return newPoints;
        }

        private PointF GetIdentityHexOffset(Hexagon hex)
        {
            var newX = hex.X - (hex.Y * 0.5f);
            return new PointF(newX, -0.75f * hex.Y);
        }

        private PointF[] HexEdgeToAbsolutePoints(HexEdge hexEdge)
        {
            var h1 = HexagonToAbsolutePoints(hexEdge.Hex1);
            var h2 = HexagonToAbsolutePoints(hexEdge.Hex2);
            var linePoints = h1.Intersect(h2).ToArray();
            if (linePoints.Length == 2)
                return linePoints;
            return null;
        }

        private PointF HexPointToAbsolutePoint(HexPoint hexPoint)
        {
            var h1 = HexagonToAbsolutePoints(hexPoint.Hex1);
            var h2 = HexagonToAbsolutePoints(hexPoint.Hex2);
            var h3 = HexagonToAbsolutePoints(hexPoint.Hex3);
            var centerPoint = h1.Intersect(h2).Intersect(h3).ToArray();
            if (centerPoint.Length == 1)
                return centerPoint[0];
            return PointF.Empty;
        }

        private PointF RelativeToAbsolute(PointF relativePoint)
        {
            return RelativeToAbsolute(relativePoint.X, relativePoint.Y);
        }

        private PointF RelativeToAbsolute(float x, float y)
        {
            var w = this.Width;
            var h = this.Height;
            return new PointF(w * (x / 100f), h * (y / 100f));
        }

        private PointF[] HexagonToAbsolutePoints(Hexagon hex)
        {
            if (_hexPointMap.ContainsKey(hex))
                return _hexPointMap[hex];
            var hexPointsRelative = GetRelativeHexegonPoints(hex);
            var hexPointsAbsolute = ConvertRelativePointsToAbsolute(hexPointsRelative);
            _hexPointMap[hex] = hexPointsAbsolute;
            return hexPointsAbsolute;
        }

    }
}
