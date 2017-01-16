using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.Core;
using Jatan.GameLogic;
using Jatan.Models;
using JatanWebApp.Helpers;
using JatanWebApp.Models.DAL;
using JatanWebApp.Models.ViewModels;

namespace JatanWebApp.SignalR
{
    public class GameLobby
    {
        /// <summary>
        /// The unique identifier for this game lobby.
        /// </summary>
        public string Uid { get; private set; }
        /// <summary>
        /// The name that players will see when searching for games to join.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The optional password to join the game.
        /// </summary>
        public string Password { get; private set; }
        /// <summary>
        /// Indicates if a password is needed to join the game.
        /// </summary>
        public bool RequiresPassword { get { return !string.IsNullOrEmpty(this.Password); } }
        /// <summary>
        /// The maximum number of players allowed to join the game.
        /// </summary>
        public int MaxNumberOfPlayers { get; private set; }
        /// <summary>
        /// The Jatan game manager object.
        /// </summary>
        public GameManager GameManager { get; private set; }
        /// <summary>
        /// The player (username) who created the game.
        /// </summary>
        public string Owner { get; private set; }
        /// <summary>
        /// Gets the players (usernames) currently in the game.
        /// </summary>
        public List<string> Players { get { return this.GameManager.Players.Select(p => p.Name).ToList(); } }
        /// <summary>
        /// Indicates if the game has started. If true, players will no longer be allowed to join.
        /// </summary>
        public bool InProgress { get { return this.GameManager.GameState != GameState.NotStarted; } }

        /// <summary>
        /// GameLobby constructor
        /// </summary>
        public GameLobby(string ownerName, CreateGameViewModel model)
        {
            this.Uid = Guid.NewGuid().ToString();

            this.Name = model.DisplayName;
            this.Password = model.Password;
            this.Owner = ownerName;

            this.MaxNumberOfPlayers = model.MaxNumberOfPlayers;

            this.GameManager = new GameManager();
            this.GameManager.Settings.ScoreNeededToWin = model.WinScore;
            this.GameManager.Settings.RobberMode = model.RobberMode;
            this.GameManager.Settings.TurnTimeLimit = model.TurnTimeLimit;
            this.GameManager.Settings.CardCountLossThreshold = model.CardLossThreshold;

            this.GameManager.AddPlayer(ownerName);
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame()
        {
            if (this.InProgress) return;
            this.GameManager.StartNewGame();
        }

        /// <summary>
        /// Completely cancels a game.
        /// </summary>
        public void CancelGame()
        {
            // TODO
        }

        /// <summary>
        /// 
        /// </summary>
        public Jatan.Core.ActionResult JoinGame(string userName, string password)
        {
            if (this.InProgress)
                return ActionResult.CreateFailed("This game is in progress.");

            if (this.Players.Count >= this.MaxNumberOfPlayers)
                return ActionResult.CreateFailed("This game is full.");

            if (this.RequiresPassword && password != this.Password)
                return ActionResult.CreateFailed("The password is incorrect.");

            this.GameManager.AddPlayer(userName);

            return ActionResult.CreateSuccess();
        }

        /// <summary>
        /// Makes a play abandon a game.
        /// </summary>
        public void AbandonGame(string userName)
        {
            var player = this.GameManager.GetPlayerFromName(userName);
            if (player != null)
            {
                var result = this.GameManager.PlayerAbandonGame(player.Id);
            }
        }

        // Temp dode for testing
        private static GameManager DoInitialPlacements(GameManager manager)
        {
            // This setup method will create a 3-player game with the center and far-right hexagons fully surrounded.

            int PLAYER_0 = 0;
            int PLAYER_1 = 1;
            int PLAYER_2 = 2;

            if (manager.Players.Count < 3)
                manager.AddPlayer("Billy"); // PLAYER_0
            if (manager.Players.Count < 3)
                manager.AddPlayer("John"); // PLAYER_1
            if (manager.Players.Count < 3)
                manager.AddPlayer("Greg"); // PLAYER_2

            manager.StartNewGame();

            // player 0
            manager.PlayerPlaceBuilding(PLAYER_0, Jatan.Models.BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.Top));
            manager.PlayerPlaceRoad(PLAYER_0, Hexagon.Zero.GetEdge(EdgeDir.TopRight));

            // player 1
            manager.PlayerPlaceBuilding(PLAYER_1, Jatan.Models.BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.BottomRight));
            manager.PlayerPlaceRoad(PLAYER_1, Hexagon.Zero.GetEdge(EdgeDir.Right));

            // player 2
            manager.PlayerPlaceBuilding(PLAYER_2, Jatan.Models.BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.BottomLeft));
            manager.PlayerPlaceRoad(PLAYER_2, Hexagon.Zero.GetEdge(EdgeDir.Left));

            // Create around a different hexagon since the middle is filled up.
            Hexagon otherHex = new Hexagon(2, 0);

            manager.PlayerPlaceBuilding(PLAYER_2, Jatan.Models.BuildingTypes.Settlement, otherHex.GetPoint(PointDir.BottomLeft));
            manager.PlayerPlaceRoad(PLAYER_2, otherHex.GetEdge(EdgeDir.Left));

            // player 1
            manager.PlayerPlaceBuilding(PLAYER_1, Jatan.Models.BuildingTypes.Settlement, otherHex.GetPoint(PointDir.BottomRight));
            manager.PlayerPlaceRoad(PLAYER_1, otherHex.GetEdge(EdgeDir.Right));

            // player 0
            manager.PlayerPlaceBuilding(PLAYER_0, Jatan.Models.BuildingTypes.Settlement, otherHex.GetPoint(PointDir.Top));
            manager.PlayerPlaceRoad(PLAYER_0, otherHex.GetEdge(EdgeDir.TopRight));

            return manager;
        }
    }
}