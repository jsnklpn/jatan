﻿using System;
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
        /// Creates a new game with the specified owner and settings. The owner is
        /// auto-added to the list of players. Returns the new game lobby instance.
        /// </summary>
        public static GameLobby CreateNewGame(string ownerUserName, string ownerAvatarPath, CreateGameViewModel model)
        {
            // Leave any game we might be in.
            AbandonCurrentGame(ownerUserName);

            var lobby = new GameLobby(ownerUserName, ownerAvatarPath, model);
            GameLobbies[ownerUserName] = lobby;
            return lobby;
        }

        /// <summary>
        /// Cancels the game currently owned by the specified player.
        /// </summary>
        public static void CancelGame(string ownerUserName)
        {
            if (!GameLobbies.ContainsKey(ownerUserName))
                return;

            // Notify all players that the game shut down.
            var hubClients = GameHub.GetClientsForGame(ownerUserName);
            GameHub.GetClientsForGame(ownerUserName).gameAborted();

            GameLobby tmp;
            if (GameLobbies.TryRemove(ownerUserName, out tmp))
            {
                tmp.CancelGame();
            }
        }

        /// <summary>
        /// Connects to a game lobby that is owned by the specified player.
        /// </summary>
        public static Jatan.Core.ActionResult ConnectToGame(string userName, string ownerUserName, string password, string avatarPath)
        {
            // First, disconnect from any active games.
            AbandonCurrentGame(userName);

            if (GameLobbies.ContainsKey(ownerUserName))
            {
                var lobby = GameLobbies[ownerUserName];

                // We're already in the game.
                if (lobby.Players.Contains(userName))
                    return Jatan.Core.ActionResult.CreateSuccess();

                var result = lobby.JoinGame(userName, password, avatarPath);
                if (result.Succeeded)
                {
                    // Tell all hub clients in the lobby that a new player has joined.
                    GameHub.GetClientsForGame(userName).newPlayerJoined(userName);
                }
                return result;
            }
            return Jatan.Core.ActionResult.CreateFailed("This game no longer exists.");
        }

        /// <summary>
        /// Makes a player leave the game they're in.
        /// </summary>
        public static void AbandonCurrentGame(string userName)
        {
            var lobby = GetGameLobbyForPlayer(userName);
            if (lobby == null)
                return;

            if (lobby.Owner == userName)
            {
                // We're the owner. Cancel the game.
                CancelGame(userName);    
            }
            else
            {
                // Alert other users that the player left.
                GameHub.GetClientsForGame(userName).playerLeft(userName);

                lobby.AbandonGame(userName);
            }
        }
    }
}