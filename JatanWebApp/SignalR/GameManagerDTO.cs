using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.GameLogic;
using Jatan.Models;

namespace JatanWebApp.SignalR
{
    public class GameManagerDTO
    {
        public GameBoardDTO GameBoard { get; set; }
        public GameState GameState { get; set; }

        public GameManagerDTO(GameManager manager)
        {
            this.GameBoard = new GameBoardDTO(manager.GameBoard);
            this.GameState = manager.GameState;
        }
    }
}