using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.GameLogic;
using Jatan.Models;

namespace JatanWebApp.SignalR.DTO
{
    /// <summary>
    /// GameManager data transfer object.
    /// </summary>
    public class GameManagerDTO
    {
        public GameBoardDTO GameBoard { get; set; }
        public GameState GameState { get; set; }
        public int CurrentDiceRoll { get; set; }
        public List<PlayerDTO> Players { get; set; }

        public GameManagerDTO(GameManager manager, int requestingPlayerId, bool includeBoardConstants)
        {
            this.GameBoard = new GameBoardDTO(manager.GameBoard, includeBoardConstants);
            this.GameState = manager.GameState;
            this.CurrentDiceRoll = manager.CurrentDiceRoll;
            this.Players = new List<PlayerDTO>();
            foreach (var p in manager.Players)
            {
                this.Players.Add(new PlayerDTO(p, (p.Id == requestingPlayerId)));
            }
        }
    }
}