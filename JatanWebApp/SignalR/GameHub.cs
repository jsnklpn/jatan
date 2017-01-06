﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.GameLogic;
using JatanWebApp.SignalR.DTO;
using Microsoft.AspNet.SignalR;

namespace JatanWebApp.SignalR
{
    public class GameHub : Hub
    {
        private static GameManager _gameManager;

        static GameHub()
        {
            _gameManager = new GameManager();
            _gameManager.AddPlayer("Jason");
            _gameManager.AddPlayer("Simon");
            _gameManager.AddPlayer("Eric");
            _gameManager.StartNewGame();
        }

        public void SendChatMessage(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }

        /// <summary>
        /// Sends a full game manager update to the calling client.
        /// A full update includes constants like resources tiles and port locations.
        /// </summary>
        public void GetGameManagerUpdate(bool fullUpdate)
        {
            var managerDto = new GameManagerDTO(_gameManager, 0, fullUpdate);
            Clients.Caller.updateGameManager(managerDto);
        }
    }
}