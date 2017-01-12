using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.GameLogic;
using JatanWebApp.Models.DAL;
using JatanWebApp.Models.ViewModels;

namespace JatanWebApp.SignalR
{
    public class GameLobby
    {
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
        public List<string> Players { get; private set; }
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
        public GameLobby(string gameName, string ownerName, CreateGameViewModel model)
        {
            this.Name = gameName;
            this.Password = model.Password;
            this.Owner = ownerName;

            this.MaxNumberOfPlayers = model.MaxNumberOfPlayers;
            this.Players = new List<string>();

            this.InProgress = false;

            this.GameManager = new GameManager();
            this.GameManager.Settings.ScoreNeededToWin = model.WinScore;
            this.GameManager.Settings.RobberMode = model.RobberMode;
            this.GameManager.Settings.TurnTimeLimit = model.TurnTimeLimit;
            this.GameManager.Settings.CardCountLossThreshold = model.CardLossThreshold;
        }
    }
}