using Microsoft.AspNetCore.Identity;

namespace JatanWebAppCore.Data
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// The current money this player has for buying things. For future use.
        /// </summary>
        public int Money { get; set; }

        /// <summary>
        /// The average turn length, in seconds.
        /// </summary>
        public float AverageTurnLength { get; set; }

        /// <summary>
        /// The total number of resources this user has collected.
        /// </summary>
        public int TotalResourcesCollected { get; set; }

        /// <summary>
        /// The total number of minutes this user has played
        /// </summary>
        public int TotalMinutesPlayed { get; set; }

        /// <summary>
        /// The number of games this user has won
        /// </summary>
        public int GamesWon { get; set; }

        /// <summary>
        /// Number of games this user has played
        /// </summary>
        public int GamesPlayed { get; set; }

        /// <summary>
        /// DB foreign key for the avatar image
        /// </summary>
        public long? UserImageId { get; set; }

        /// <summary>
        /// The image for this user
        /// </summary>
        public virtual UserImage UserImage { get; set; }
    }
}
