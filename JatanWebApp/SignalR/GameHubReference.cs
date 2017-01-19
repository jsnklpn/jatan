using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace JatanWebApp.SignalR
{
    /// <summary>
    /// Class used by outside objects to call hub methods
    /// </summary>
    public class GameHubReference
    {
        private IHubContext<IGameHubClient> _context;

        private static readonly Lazy<GameHubReference> _instance = new Lazy<GameHubReference>(
            () => new GameHubReference(GlobalHost.ConnectionManager.GetHubContext<GameHub, IGameHubClient>()));

        /// <summary>
        /// Gets the hub context.
        /// </summary>
        public static IHubContext<IGameHubClient> Context { get { return _instance.Value._context; } }

        /// <summary>
        /// Private constructor
        /// </summary>
        private GameHubReference(IHubContext<IGameHubClient> context)
        {
            _context = context;
        }
    }
}