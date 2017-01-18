using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Jatan.Core;
using Jatan.GameLogic;
using Jatan.Models;
using JatanWebApp.Helpers;
using JatanWebApp.SignalR.DTO;
using Microsoft.AspNet.SignalR;

namespace JatanWebApp.SignalR
{
    /// <summary>
    /// Signal-R hub used for real-time updates between the clients and the server.
    /// </summary>
    [Authorize]
    public class GameHub : Hub
    {
        private static readonly ConcurrentDictionary<string, HubUser> _hubUsers;

        /// <summary>
        /// Gets the users currently connected to this hub.
        /// </summary>
        public static ConcurrentDictionary<string, HubUser> HubUsers
        {
            get { return _hubUsers; }
        }

        static GameHub()
        {
            _hubUsers = new ConcurrentDictionary<string, HubUser>();
        }

        #region Hub helper methods

        #region Hub overrides

        public override Task OnConnected()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            var user = _hubUsers.GetOrAdd(userName, new HubUser
            {
                Username = userName,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {
                user.ConnectionIds.Add(connectionId);
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            var user = GetUser(userName);

            if (user != null)
            {
                lock (user.ConnectionIds)
                {
                    user.ConnectionIds.RemoveWhere(i => i.Equals(connectionId));

                    // If there are no connections left, remove the hub-user completely
                    if (!user.ConnectionIds.Any())
                    {
                        HubUser removedUser;
                        _hubUsers.TryRemove(userName, out removedUser);
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        #endregion

        private static HubUser GetUser(string userName)
        {
            HubUser user;
            _hubUsers.TryGetValue(userName, out user);
            return user;
        }

        private HubUser GetUser()
        {
            string userName = Context.User.Identity.Name;
            return GetUser(userName);
        }

        private static GameLobby GetGameLobby(string userName)
        {
            return GameLobbyManager.GetGameLobbyForPlayer(userName);
        }

        private GameLobby GetGameLobby()
        {
            var user = GetUser();
            if (user == null) return null;
            return GetGameLobby(user.Username);
        }

        private static int GetJatanPlayerId(string userName)
        {
            var lobby = GameLobbyManager.GetGameLobbyForPlayer(userName);
            if (lobby == null)
                return -1;
            var jp = lobby.GameManager.GetPlayerFromName(userName);
            if (jp == null)
                return -1;
            return jp.Id;
        }

        private int GetJatanPlayerId()
        {
            string userName = Context.User.Identity.Name;
            return GetJatanPlayerId(userName);
        }

        #endregion

        /// <summary>
        /// Broadcasts a chat message to all connected clients.
        /// </summary>
        public void SendChatMessage(string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(GetUser().Username, message);
        }

        /// <summary>
        /// Sends a full game manager update to the calling client.
        /// A full update includes constants like resources tiles and port locations.
        /// </summary>
        public void GetGameManagerUpdate(bool fullUpdate)
        {
            var callerPlayerId = GetJatanPlayerId();
            var manager = GetGameLobby().GameManager;
            var managerDto = new GameManagerDTO(manager, callerPlayerId, fullUpdate);
            Clients.Caller.updateGameManager(managerDto);
        }

        /// <summary>
        /// Starts the game. Will only work if called by the game owner.
        /// </summary>
        public void StartGame()
        {
            var user = GetUser();
            var lobby = GetGameLobby();

            // Only the owned can start the game.
            if (lobby.InProgress || user.Username != lobby.Owner)
                return;

            lobby.GameManager.StartNewGame();
            UpdateAllClientGameManagers(true);
        }

        /// <summary>
        /// Ends the turn of the current player.
        /// </summary>
        public void EndTurn()
        {
            var playerId = GetJatanPlayerId();
            var lobby = GetGameLobby();
            var result = lobby.GameManager.PlayerEndTurn(playerId);
            if (result.Succeeded)
            {
                UpdateAllClientGameManagers();
            }
        }

        private void UpdateAllClientGameManagers(bool fullUpdate = false)
        {
            foreach (var userName in HubUsers.Keys)
            {
                var lobby = GetGameLobby();
                var userId = GetJatanPlayerId(userName);
                var managerDto = new GameManagerDTO(lobby.GameManager, userId, fullUpdate);
                foreach (var connectionId in HubUsers[userName].ConnectionIds)
                {
                    Clients.Client(connectionId).updateGameManager(managerDto);
                }
            }
        }
    }
}