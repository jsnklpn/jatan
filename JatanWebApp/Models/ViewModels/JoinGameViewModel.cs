using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JatanWebApp.SignalR;

namespace JatanWebApp.Models.ViewModels
{
    public class JoinGameViewModel
    {
        public List<GameLobby> AvailableGames { get; set; }

        public JoinGameViewModel()
        {
            AvailableGames = new List<GameLobby>(GameLobbyManager.GameLobbies.Values.Where(g => g != null));
        }
    }
}