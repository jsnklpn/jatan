using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JatanWebApp.SignalR.DTO;

namespace JatanWebApp.SignalR
{
    /// <summary>
    /// Interface for the GameHub client javascript methods.
    /// </summary>
    public interface IGameHubClient
    {
        void broadcastMessage(string userName, string message);
        void showToastMessage(string message);
        void updateGameManager(GameManagerDTO manager);
        void newPlayerJoined(string newPlayerName);
        void playerLeft(string playerName);
        void gameAborted();
        void turnTimeLimitExpired(int playerId);
    }
}
