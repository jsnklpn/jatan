using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using JatanWebApp.Models.DAL;

namespace JatanWebApp.Helpers
{
    /// <summary>
    /// Static class to hold common database operations.
    /// </summary>
    public static class DatabaseHelper
    {
        static DatabaseHelper()
        {
            string contentRootPath = WebConfigurationManager.AppSettings["ContentRootPath"];
            AvatarPath = contentRootPath + "/Content/Images/avatars";
        }

        public static string AvatarPath { get; private set; }

        /// <summary>
        /// Returns the path to the default avatar
        /// </summary>
        public static string DefaultAvatarPath
        {
            get { return AvatarPath + @"/default.jpg"; }
        }

        /// <summary>
        /// Returns the path to the avatar for a specific username.
        /// </summary>
        public static string GetAvatarPathFromUsername(string userName)
        {
            try
            {
                if (!string.IsNullOrEmpty(userName))
                {
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
                }
            }
            catch { }
            return DefaultAvatarPath;
        }
    }
}