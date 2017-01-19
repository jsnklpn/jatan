using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JatanWebApp.Models.DAL;

namespace JatanWebApp.SignalR.DTO
{
    /// <summary>
    /// User account information. Used when a player joins a game and all clients are notified.
    /// </summary>
    public class UserAccountInfoDTO
    {
        /// <summary>
        /// The username of the account.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The relative url for the avatar.
        /// </summary>
        public string ImagePath { get; set; }

        public UserAccountInfoDTO(string username, string avatarPath)
        {
            UserName = username;
            ImagePath = avatarPath;
        }
    }
}
