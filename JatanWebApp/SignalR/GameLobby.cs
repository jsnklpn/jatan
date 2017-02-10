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
using JatanWebApp.SignalR.DTO;

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
        /// Gets the players (usernames) currently in the game, ordered by playerId.
        /// </summary>
        public List<string> Players { get { return this.GameManager.Players.OrderBy(p => p.Id).Select(p => p.Name).ToList(); } }
        /// <summary>
        /// Gets the dictionary of avatar paths. The key is the username.
        /// </summary>
        public Dictionary<string, string> AvatarPaths { get; private set; }
        /// <summary>
        /// Indicates if the game has started. If true, players will no longer be allowed to join.
        /// </summary>
        public bool InProgress { get { return this.GameManager.GameState != GameState.NotStarted; } }
        /// <summary>
        /// Indicates if the game has been cancelled.
        /// </summary>
        public bool Cancelled { get; private set; }

        /// <summary>
        /// GameLobby constructor
        /// </summary>
        public GameLobby(string ownerName, string ownerAvatarPath, CreateGameViewModel model)
        {
            this.Uid = Guid.NewGuid().ToString();

            this.AvatarPaths = new Dictionary<string, string>();
            this.AvatarPaths[ownerName] = ownerAvatarPath;

            this.Name = model.DisplayName;
            this.Password = model.Password;
            this.Owner = ownerName;
            this.Cancelled = false;

            this.MaxNumberOfPlayers = model.MaxNumberOfPlayers;

            this.GameManager = new GameManager();
            this.GameManager.Settings.ScoreNeededToWin = model.WinScore;
            this.GameManager.Settings.RobberMode = model.RobberMode;
            this.GameManager.Settings.TurnTimeLimit = model.TurnTimeLimit;
            this.GameManager.Settings.CardCountLossThreshold = model.CardLossThreshold;
            this.GameManager.PlayerTurnTimeLimitExpired += GameManager_PlayerTurnTimeLimitExpired;

            this.GameManager.AddPlayer(ownerName);
        }

        private void GameManager_PlayerTurnTimeLimitExpired(object sender, TimeLimitElapsedArgs e)
        {
            // The current player's turn just expired and their turn was skipped.
            // Inform all players that the game state changed.
            GameHub.GetClientsForGame(Owner).turnTimeLimitExpired(e.PlayerId);
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
            // Remove all players from the game.
            foreach (var player in this.GameManager.Players)
            {
                this.GameManager.PlayerAbandonGame(player.Id);
            }
            this.Cancelled = true;
        }

        /// <summary>
        /// Joins a game if available.
        /// TODO: If I add more user properties, replace avatarPath with info object.
        /// </summary>
        public Jatan.Core.ActionResult JoinGame(string userName, string password, string avatarPath)
        {
            if (this.InProgress)
                return ActionResult.CreateFailed("This game is in progress.");

            if (this.Players.Count >= this.MaxNumberOfPlayers)
                return ActionResult.CreateFailed("This game is full.");

            if (this.RequiresPassword && password != this.Password)
                return ActionResult.CreateFailed("The password is incorrect.");

            this.GameManager.AddPlayer(userName);
            this.AvatarPaths[userName] = avatarPath;

            return ActionResult.CreateSuccess();
        }

        /// <summary>
        /// Makes a player abandon a game.
        /// </summary>
        public void AbandonGame(string userName)
        {
            var player = this.GameManager.GetPlayerFromName(userName);
            if (player != null)
            {
                var result = this.GameManager.PlayerAbandonGame(player.Id);
                if (result.Succeeded)
                {
                    this.AvatarPaths.Remove(userName);
                }
            }
        }

        /// <summary>
        /// Gets a GameManager data-transfer-object for this game.
        /// </summary>
        public GameManagerDTO ToGameManagerDTO(string requestingUser, bool includeBoardConstants, bool includeAvatarPaths)
        {
            var player = this.GameManager.GetPlayerFromName(requestingUser);
            var id = (player != null) ? player.Id : -1;
            var dto = new GameManagerDTO(this, id, includeBoardConstants, includeAvatarPaths);
            return dto;
        }
    }
}