using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JatanWebApp.SignalR
{
    /// <summary>
    /// Class to keep track of all current games and lobbies.
    /// </summary>
    public class GameLobbyManager
    {
        public static ConcurrentBag<GameLobby> GameLobbies { get; private set; }

        static GameLobbyManager()
        {
            GameLobbies = new ConcurrentBag<GameLobby>();
        }
    }
}