using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.GameLogic;
using Jatan.Models;
using JatanWebApp.SignalR;

namespace JatanWebApp.Models.ViewModels
{
    /// <summary>
    /// ViewModel for the PostGame view.
    /// </summary>
    public class PostGameViewModel
    {
        /// <summary>
        /// Error message if something went wrong.
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// The name of the game
        /// </summary>
        public string GameName { get; set; }
        /// <summary>
        /// The UID of the game
        /// </summary>
        public string GameUid { get; set; }
        /// <summary>
        /// The game statistics
        /// </summary>
        public GameStatistics Stats { get; set; }

        public PostGameViewModel(string gameId)
        {
            this.ErrorMessage = null;
            this.GameUid = gameId;

            var lobby = GameLobbyManager.GameLobbies.Values.FirstOrDefault(g => g.Uid == gameId);
            if (lobby == null || lobby.GameManager == null)
            {
                ErrorMessage = "Game not found.";
                return;
            }
            this.GameName = lobby.Name;

            if (lobby.GameManager.GameState != GameState.EndOfGame)
            {
                ErrorMessage = "The game is not finished.";
                return;
            }

            this.Stats = lobby.GameManager.Statistics;
        }

        public static PostGameViewModel CreateTestVm()
        {
            var vm = new PostGameViewModel("test")
            {
                GameName = "Test game",
                GameUid = "test",
                ErrorMessage = null,
                Stats = GameStatistics.CreateTestStats()
            };

            return vm;
        }

    }
}