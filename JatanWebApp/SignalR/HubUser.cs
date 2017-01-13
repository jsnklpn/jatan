using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JatanWebApp.SignalR
{
    /// <summary>
    /// Class to represent a user that talks to a Signal-R hub.
    /// </summary>
    public class HubUser
    {
        /// <summary>
        /// The ASP.net Identity username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The Signal-R connection IDs that belong to this user.
        /// </summary>
        public HashSet<string> ConnectionIds { get; set; }

        public HubUser()
        {
            
        }
    }
}