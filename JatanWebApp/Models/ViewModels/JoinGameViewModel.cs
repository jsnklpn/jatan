using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JatanWebApp.Models.ViewModels
{
    public class JoinGameViewModel
    {
        public List<string> AvailableGames { get; set; }

        public JoinGameViewModel()
        {
            AvailableGames = new List<string>();

            AvailableGames.AddRange(new string[] { "Game 1", "Game 2", "Game 3", "Game 4" });
        }
    }
}