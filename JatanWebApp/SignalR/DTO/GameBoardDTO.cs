using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.Core;
using Jatan.Models;

namespace JatanWebApp.SignalR.DTO
{
    /// <summary>
    /// Gameboard data transfer object.
    /// </summary>
    public class GameBoardDTO
    {
        public Dictionary<Hexagon, ResourceTile> ResourceTiles { get; set; }
        public Dictionary<HexEdge, Road> Roads { get; set; }
        public Dictionary<HexPoint, Building> Buildings { get; set; }
        public Dictionary<HexEdge, Port> Ports { get; set; }
        public Hexagon RobberLocation { get; set; }
        public RobberMode RobberMode { get; set; }

        public GameBoardDTO(GameBoard board, bool includeBoardConstants)
        {
            if (includeBoardConstants)
            {
                this.ResourceTiles = new Dictionary<Hexagon, ResourceTile>(board.ResourceTiles);
                this.Ports = new Dictionary<HexEdge, Port>(board.Ports);
            }
            this.Roads = new Dictionary<HexEdge, Road>(board.Roads);
            this.Buildings = new Dictionary<HexPoint, Building>(board.Buildings);
            this.RobberLocation = board.RobberLocation;
            this.RobberMode = board.RobberMode;
        }
    }
}