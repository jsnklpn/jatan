using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JatanWebApp.Models.DAL
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

    /// <summary>
    /// User database model.
    /// </summary>
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

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}