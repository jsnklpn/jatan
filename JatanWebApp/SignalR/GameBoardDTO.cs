using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.Core;
using Jatan.Models;

namespace JatanWebApp.SignalR
{
    public class GameBoardDTO
    {
        public Dictionary<Hexagon, ResourceTile> ResourceTiles { get; set; }

        public GameBoardDTO(GameBoard board)
        {
            this.ResourceTiles = new Dictionary<Hexagon, ResourceTile>(board.ResourceTiles);
        }
    }
}