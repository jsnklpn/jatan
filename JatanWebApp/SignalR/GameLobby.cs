using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.Core;
using Jatan.GameLogic;
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
        /// Gets the players (usernames) currently in the game.
        /// </summary>
        public HashSet<string> Players { get; private set; }
        /// <summary>
        /// The player (username) who created the game.
        /// </summary>
        public string Owner { get; private set; }
        /// <summary>
        /// Indicates if the game has started. If true, players will no longer be allowed to join.
        /// </summary>
        public bool InProgress { get; private set; }

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
            this.Players = new HashSet<string> {ownerName};

            this.InProgress = false;

            this.GameManager = new GameManager();
            this.GameManager.Settings.ScoreNeededToWin = model.WinScore;
            this.GameManager.Settings.RobberMode = model.RobberMode;
            this.GameManager.Settings.TurnTimeLimit = model.TurnTimeLimit;
            this.GameManager.Settings.CardCountLossThreshold = model.CardLossThreshold;
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame()
        {
            if (this.InProgress) return;

            this.InProgress = true;
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

            Players.Add(userName);

            return ActionResult.CreateSuccess();
        }

        /// <summary>
        /// Makes a play abandon a game.
        /// </summary>
        public void AbandonGame(string userName)
        {
            this.Players.Remove(userName);

            // TODO
            // this.GameManager.PlayerAbandonGame()
        }
    }
}