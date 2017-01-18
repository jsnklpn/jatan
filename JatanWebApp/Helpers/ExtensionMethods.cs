using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using JatanWebApp.Models.DAL;

namespace JatanWebApp.Helpers
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the Jatan Player object from their username. Returns null if not found.
        /// </summary>
        public static Jatan.Models.Player GetPlayerFromName(this Jatan.GameLogic.GameManager manager, string userName)
        {
            return manager.Players.FirstOrDefault(p => p.Name == userName);
        }

        public static string GetAvatarPath(this IIdentity identity)
        {
            var userName = identity.Name;
            using (var db = new JatanDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.UserName == userName);
                if (user != null)
                {
                    var userImage = user.UserImage;
                    if (userImage != null)
                    {
                        return userImage.ImagePath;
                    }
                }
            }
            return "";
        }
    }
}