using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jatan.GameLogic;
using JatanWebApp.Models.ViewModels;
using JatanWebApp.SignalR.DTO;
using ActionResult = Jatan.Core.ActionResult;

namespace JatanWebApp.SignalR
{
    /// <summary>
    /// Static class to keep track of all current games and lobbies.
    /// </summary>
    public static class GameLobbyManager
    {
        /// <summary>
        /// A collection containing all current games. This inclues game that are in progress.
        /// </summary>
        public static ConcurrentDictionary<string, GameLobby> GameLobbies { get; private set; }
        
        static GameLobbyManager()
        {
            GameLobbies = new ConcurrentDictionary<string, GameLobby>();
        }

        /// <summary>
        /// Creates a new game with the specified owner and settings. The owner is
        /// auto-added to the list of players. Returns the new game lobby instance.
        /// </summary>
        public static GameLobby CreateNewGame(string ownerUserName, CreateGameViewModel model)
        {
            // Remove any game that may be in progress from the current user.
            CancelGame(ownerUserName);

            var lobby = new GameLobby(ownerUserName, model);
            GameLobbies[ownerUserName] = lobby;
            return lobby;
        }

        /// <summary>
        /// Cancels the game currently owned by the specified player.
        /// </summary>
        public static void CancelGame(string ownerUserName)
        {
            GameLobby tmp;
            if (GameLobbies.TryRemove(ownerUserName, out tmp))
            {
                tmp.CancelGame();
            }
        }

        /// <summary>
        /// Returns the game lobby with the unique id.
        /// </summary>
        public static GameLobby GetGameLobbyFromUid(string uid)
        {
            var game = GameLobbies.Values.FirstOrDefault(g => g.Uid.Equals(uid));
            return game;
        }

        /// <summary>
        /// Returns the game lobby that the specified player is connected to. Returns null if none.
        /// </summary>
        public static GameLobby GetGameLobbyForPlayer(string userName)
        {
            var game = GameLobbies.Values.FirstOrDefault(g => g.Players.Contains(userName));
            return game;
        }
        
        /// <summary>
        /// Connects to a game lobby that is owned by the specified player.
        /// </summary>
        public static Jatan.Core.ActionResult ConnectToGame(string userName, string ownerUserName, string password, string avatarPath)
        {
            if (GameLobbies.ContainsKey(ownerUserName))
            {
                var lobby = GameLobbies[ownerUserName];

                // We're already in the game.
                if (lobby.Players.Contains(userName))
                    return Jatan.Core.ActionResult.CreateSuccess();

                var result =  lobby.JoinGame(userName, password);
                if (result.Succeeded)
                {
                    // Tell the hub that a new player joined.
                    GameHubReference.Context.Clients.All.newPlayerJoined(new UserAccountInfoDTO(userName, avatarPath));
                }
                return result;
            }
            return Jatan.Core.ActionResult.CreateFailed("This game no longer exists.");
        }
    }
}