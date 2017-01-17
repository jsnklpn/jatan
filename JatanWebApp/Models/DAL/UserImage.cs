using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JatanWebApp.Models.DAL
{
    /// <summary>
    /// Db model for a user image. The actual image data will stored on the file system.
    /// </summary>
    public class UserImage : EntityBase
    {
        /// <summary>
        /// The path to the image stored on the file system.
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// The name of the file uploaded by the user.
        /// </summary>
        public string UserFileName { get; set; }
    }
}