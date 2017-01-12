using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Jatan.Models;

namespace JatanWebApp.Models.ViewModels
{
    public class CreateGameViewModel
    {
        [Required]
        [Display(Name = "Lobby name")]
        public string DisplayName { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Range(2, 4, ErrorMessage = "Number of players must be between 2 and 4.")]
        [Display(Name = "Max number of players")]
        public int MaxNumberOfPlayers { get; set; }

        [Required]
        [Display(Name = "Robber mode")]
        public Jatan.Models.RobberMode RobberMode { get; set; }

        [Required]
        [Range(3, 10, ErrorMessage = "Score must be between 3 and 10.")]
        [Display(Name = "Score needed to win")]
        public int WinScore { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Time limit must be between 0 (no limit) and 3600 (1 hour).")]
        [Display(Name = "Turn time limit (seconds)")]
        public int TurnTimeLimit { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Card count must be between 0 (no cards lost) and 100.")]
        [Display(Name = "Card count loss threshold",
            Description = "This is the number of cards that will cause a player to lose half of their hand when a 7 is rolled.")]
        public int CardLossThreshold { get; set; }

        public CreateGameViewModel()
        {
            MaxNumberOfPlayers = 4;
            RobberMode = RobberMode.Normal;
            WinScore = 10;
            TurnTimeLimit = 0; // No time limit
            CardLossThreshold = 8;
        }
    }
}