using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}