using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JatanWebApp.Helpers;
using JatanWebApp.Models.DAL;

namespace JatanWebApp.Models.ViewModels
{
    public class ApplicationUserViewModel
    {
        public string UserName { get; set; }
        public string AvatarPath { get; set; }
        public int Money { get; set; }
        public int TotalResourcesCollected { get; set; }
        public TimeSpan AverageTurnLength { get; set; }
        public TimeSpan TotalTimePlayed { get; set; }
        public int GamesWon { get; set; }
        public int GamesPlayed { get; set; }

        /// <summary>
        /// Gets the win percentage as a floating point number.
        /// </summary>
        public float WinPercentage { get; private set; }

        /// <summary>
        /// Gets the total player score for this user. This is calculated based number of games played, games won, resources collected, and total time played.
        /// </summary>
        public int PlayerScore { get; private set; }

        public ApplicationUserViewModel(ApplicationUser model)
        {
            this.UserName = model.UserName;
            this.AvatarPath = (model.UserImage != null) ? model.UserImage.ImagePath : DatabaseHelper.DefaultAvatarPath;
            this.Money = model.Money;
            this.TotalResourcesCollected = model.TotalResourcesCollected;
            this.AverageTurnLength = TimeSpan.FromSeconds(model.AverageTurnLength);
            this.TotalTimePlayed = TimeSpan.FromMinutes(model.TotalMinutesPlayed);
            this.GamesWon = model.GamesWon;
            this.GamesPlayed = model.GamesPlayed;

            this.WinPercentage = GamesPlayed == 0 ? 0f : (float)GamesWon / GamesPlayed;
            this.PlayerScore = (1000 * this.GamesPlayed) + (500 * this.GamesWon) + TotalResourcesCollected + (int)TotalTimePlayed.TotalMinutes;
        }
    }
}