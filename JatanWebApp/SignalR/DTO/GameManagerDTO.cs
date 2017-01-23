using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.Core;
using Jatan.GameLogic;
using Jatan.Models;

namespace JatanWebApp.SignalR.DTO
{
    /// <summary>
    /// GameManager data transfer object.
    /// </summary>
    public class GameManagerDTO
    {
        public int MyPlayerId { get; set; }
        public GameBoardDTO GameBoard { get; set; }
        public GameState GameState { get; set; }
        public PlayerTurnState PlayerTurnState { get; set; }
        public int ActivePlayerId { get; set; }
        public RollResult CurrentDiceRoll { get; set; }
        public List<PlayerDTO> Players { get; set; }

        public GameManagerDTO(GameLobby lobby, int requestingPlayerId, bool includeBoardConstants, bool includeAvatarPaths)
        {
            var manager = lobby.GameManager;

            this.MyPlayerId = requestingPlayerId;
            this.GameBoard = new GameBoardDTO(manager.GameBoard, includeBoardConstants);
            this.GameState = manager.GameState;
            this.PlayerTurnState = manager.PlayerTurnState;
            this.ActivePlayerId = (manager.ActivePlayer != null) ? manager.ActivePlayer.Id : -1;
            this.CurrentDiceRoll = manager.CurrentDiceRoll;
            this.Players = new List<PlayerDTO>();
            foreach (var p in manager.Players.OrderBy(p => p.Id))
            {
                var playerDto = new PlayerDTO(p, (p.Id == requestingPlayerId));
                if (includeAvatarPaths)
                {
                    playerDto.AvatarPath = lobby.AvatarPaths.ContainsKey(p.Name) ? lobby.AvatarPaths[p.Name] : null;
                }
                this.Players.Add(playerDto);
            }
        }
    }
}