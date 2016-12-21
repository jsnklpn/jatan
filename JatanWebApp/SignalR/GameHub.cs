using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.GameLogic;
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

        public void GetGameManagerUpdate()
        {
            Clients.All.updateGameManager(new GameManagerDTO(_gameManager));
        }
    }
}