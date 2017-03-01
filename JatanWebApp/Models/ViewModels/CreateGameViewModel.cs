using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jatan.Models;

namespace JatanWebApp.Models.ViewModels
{
    public class CreateGameViewModel
    {
        [Required]
        [Display(Name = "Game name")]
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
        [Range(5, 10, ErrorMessage = "Score must be between 5 and 10.")]
        [Display(Name = "Score needed to win")]
        public int WinScore { get; set; }

        [Required]
        [Range(0, 180, ErrorMessage = "Time limit must be between 0 (no limit) and 180 (3 minutes).")]
        [Display(Name = "Turn time limit (seconds)")]
        public int TurnTimeLimit { get; set; }

        [Required]
        [Range(0, 20, ErrorMessage = "Card count must be between 0 (no cards lost) and 20.")]
        [Display(Name = "Card count loss threshold",
            Description = "This is the number of cards that will cause a player to lose half of their hand when a 7 is rolled.")]
        public int CardLossThreshold { get; set; }

        [Required]
        [Display(Name= "Player trading")]
        public RuleBooleanState AllowPlayerTrading { get; set; }

        /// <summary>
        /// List of valid values for the max number of players.
        /// </summary>
        public SelectList MaxNumberOfPlayersList { get; set; }

        /// <summary>
        /// List of valid values for the win score.
        /// </summary>
        public SelectList WinScoreList { get; set; }

        /// <summary>
        /// List of valid values for the turn time limit.
        /// </summary>
        public SelectList TurnTimeLimitList { get; set; }

        /// <summary>
        /// List of valid values for card loss threshold.
        /// </summary>
        public SelectList CardLossThresholdList { get; set; }

        public CreateGameViewModel()
        {
            MaxNumberOfPlayers = 4;
            RobberMode = RobberMode.Normal;
            WinScore = 10;
            TurnTimeLimit = 0; // No time limit
            CardLossThreshold = 8;
            AllowPlayerTrading = RuleBooleanState.Enabled;

            // Create the value select lists for the UI
            MaxNumberOfPlayersList = new SelectList(new List<int>() { 2, 3, 4 });
            WinScoreList = new SelectList(new List<int>() { 5, 6, 7, 8, 9, 10 });
            TurnTimeLimitList = new SelectList(new List<int>() { 0, 10, 20, 30, 45, 60, 90, 120, 180 });
            CardLossThresholdList = new SelectList(new List<int>() { 0, 6, 8, 10, 12, 20 });
        }
    }

    public enum RuleBooleanState
    {
        Enabled,
        Disabled
    }
}