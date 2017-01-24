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
    public class GameHub : Hub<IGameHubClient>
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

        #region Hub helper methods

        private string GetUserName()
        {
            return Context.User.Identity.Name;
        }

        public static HubUser GetUser(string userName)
        {
            HubUser user;
            _hubUsers.TryGetValue(userName, out user);
            return user;
        }

        private HubUser GetUser()
        {
            string userName = GetUserName();
            return GetUser(userName);
        }

        public static GameLobby GetGameLobby(string userName)
        {
            return GameLobbyManager.GetGameLobbyForPlayer(userName);
        }

        private GameLobby GetGameLobby()
        {
            var user = GetUser();
            if (user == null) return null;
            return GetGameLobby(user.Username);
        }

        public static int GetJatanPlayerId(string userName)
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

        public static IGameHubClient GetClientsForGame(string userName)
        {
            var connectionIds = new List<string>();
            var lobby = GetGameLobby(userName);
            if (lobby != null)
            {
                foreach (var name in HubUsers.Keys)
                {
                    if (lobby.Players.Contains(name))
                    {
                        connectionIds.AddRange(HubUsers[name].ConnectionIds);
                    }
                }
            }
            return GameHubReference.Context.Clients.Clients(connectionIds);
        }

        private IGameHubClient GetClientsForGame()
        {
            var connectionIds = new List<string>();
            var lobby = GetGameLobby();
            if (lobby != null)
            {
                foreach (var name in HubUsers.Keys)
                {
                    if (lobby.Players.Contains(name))
                    {
                        connectionIds.AddRange(HubUsers[name].ConnectionIds);
                    }
                }
            }
            return this.Clients.Clients(connectionIds);
        }

        /// <summary>
        /// Returns a list of hub users for a specific game.
        /// </summary>
        public static List<HubUser> GetHubUsersForGameLobby(string lobbyId)
        {
            var result = new List<HubUser>();
            foreach (var user in _hubUsers)
            {
                var lobby = GetGameLobby(user.Key);
                if (lobby != null && lobby.Uid == lobbyId)
                {
                    result.Add(user.Value);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a list of hub connection ids for a specific game.
        /// </summary>
        public static List<string> GetHubConnectionsForGameLobby(string lobbyId)
        {
            var result = new List<string>();
            var users = GetHubUsersForGameLobby(lobbyId);
            foreach (var user in users)
            {
                result.AddRange(user.ConnectionIds);
            }
            return result;
        }

        #endregion

        /// <summary>
        /// Broadcasts a chat message to all connected clients.
        /// </summary>
        public void SendChatMessage(string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(GetUserName(), message);
        }

        /// <summary>
        /// Sends a full game manager update to the calling client.
        /// A full update includes constants like resources tiles and port locations.
        /// </summary>
        public void GetGameManagerUpdate(bool fullUpdate)
        {
            var lobby = GetGameLobby();
            if (lobby == null) return;
            var managerDto = lobby.ToGameManagerDTO(GetUserName(), fullUpdate, fullUpdate);
            Clients.Caller.updateGameManager(managerDto);
        }

        /// <summary>
        /// Starts the game. Will only work if called by the game owner.
        /// </summary>
        public void StartGame()
        {
            var userName = GetUserName();
            var lobby = GetGameLobby();

            // Only the owned can start the game.
            if (lobby == null || lobby.InProgress || GetUserName() != lobby.Owner)
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
            if (lobby == null) return;
            var result = lobby.GameManager.PlayerEndTurn(playerId);
            if (result.Succeeded)
            {
                UpdateAllClientGameManagers();
            }
        }

        /// <summary>
        /// Causes the calling player to leave their game.
        /// </summary>
        public void LeaveGame()
        {
            var username = GetUserName();
            GameLobbyManager.AbandonCurrentGame(username);
        }

        /// <summary>
        /// The client selected a road location.
        /// </summary>
        public ActionResult SelectRoad(string strHexEdge)
        {
            var playerId = GetJatanPlayerId();
            if (playerId == -1) return ActionResult.CreateFailed();
            var lobby = GetGameLobby();
            if (lobby == null) return ActionResult.CreateFailed();

            var location = new HexEdge();
            try { location.FromString(strHexEdge); }
            catch (Exception e) { return ActionResult.CreateFailed(e.Message); }

            ActionResult result;
            if (lobby.GameManager.PlayerTurnState == PlayerTurnState.RoadBuildingSelectingRoads)
                result = lobby.GameManager.PlayerPlaceRoadForRoadBuilding(playerId, location);
            else
                result = lobby.GameManager.PlayerPlaceRoad(playerId, location);

            // If action succeeded, then something has changed and everyone needs an update.
            if (result.Succeeded) UpdateAllClientGameManagers();
            return result;
        }

        /// <summary>
        /// The client selected a building location.
        /// </summary>
        public ActionResult SelectBuilding(string strHexPoint, BuildingTypes type)
        {
            var playerId = GetJatanPlayerId();
            if (playerId == -1) return ActionResult.CreateFailed();
            var lobby = GetGameLobby();
            if (lobby == null) return ActionResult.CreateFailed();

            var location = new HexPoint();
            try { location.FromString(strHexPoint); }
            catch (Exception e) { return ActionResult.CreateFailed(e.Message); }

            ActionResult result = lobby.GameManager.PlayerPlaceBuilding(playerId, type, location);

            // If action succeeded, then something has changed and everyone needs an update.
            if (result.Succeeded) UpdateAllClientGameManagers();
            return result;
        }

        private void UpdateAllClientGameManagers(bool fullUpdate = false)
        {
            var lobby = GetGameLobby();
            if (lobby == null) return;
            foreach (var userName in HubUsers.Keys.Where(u => lobby.Players.Contains(u)))
            {
                var managerDto = lobby.ToGameManagerDTO(userName, fullUpdate, fullUpdate);
                var ids = HubUsers[userName].ConnectionIds.ToList();
                Clients.Clients(ids).updateGameManager(managerDto);
            }
        }
    }
}