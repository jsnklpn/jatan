using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Jatan.Models;
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

        /// <summary>
        /// Gets the avatar path for a user identity.
        /// </summary>
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
#if BETA
            return @"/beta/Content/Images/avatars/default.jpg";
#else
            return @"/Content/Images/avatars/default.jpg";
#endif
        }

        /// <summary>
        /// Converts a DateTime to a Unix timestamp
        /// </summary>
        public static long ToUnixTimestamp(this DateTime date)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);
            TimeSpan unixTimeSpan = date - unixEpoch;

            return (long)unixTimeSpan.TotalSeconds;
        }

        /// <summary>
        /// Encodes a string for JSON
        /// </summary>
        public static string JsonEscape(this object o)
        {
            return System.Web.Helpers.Json.Encode(o);
        }

        /// <summary>
        /// Encodes a string for JSON with option to trim quotation marks
        /// </summary>
        public static string JsonEscape(this object o, bool removeQuotes)
        {
            var encoded = System.Web.Helpers.Json.Encode(o);
            if (removeQuotes) return encoded.Trim('"');
            return encoded;
        }

        /// <summary>
        /// Returns a css color from a player color.
        /// </summary>
        public static string ToCssColor(this PlayerColor playerColor, float alpha)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            switch (playerColor)
            {
                case PlayerColor.Blue:
                    b = 255;
                    break;
                case PlayerColor.Green:
                    g = 255;
                    break;
                case PlayerColor.Red:
                    r = 255;
                    break;
                case PlayerColor.Yellow:
                    r = 255;
                    g = 255;
                    break;
            }
            if (alpha > 1) alpha = 1;
            else if (alpha < 0) alpha = 0;
            return string.Format("rgba({0}, {1}, {2}, {3})", r, g, b, alpha);
        }

        /// <summary>
        /// Returns a random guid string.
        /// </summary>
        public static string GetGuid(this HtmlHelper helper)
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}