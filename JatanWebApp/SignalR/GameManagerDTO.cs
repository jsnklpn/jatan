using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.GameLogic;
using Jatan.Models;

namespace JatanWebApp.SignalR
{
    /// <summary>
    /// GameManager data transfer object.
    /// </summary>
    public class GameManagerDTO
    {
        public GameBoardDTO GameBoard { get; set; }
        public GameState GameState { get; set; }
        public List<PlayerDTO> Players { get; set; }

        public GameManagerDTO(GameManager manager)
        {
            this.GameBoard = new GameBoardDTO(manager.GameBoard);
            this.GameState = manager.GameState;
            this.Players = new List<PlayerDTO>();
            manager.Players.ForEach(p => this.Players.Add(new PlayerDTO(p)));
        }
    }
}