using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.Core;
using Jatan.GameLogic;
using Jatan.Models;
using JatanWebApp.SignalR.DTO;
using Microsoft.AspNet.SignalR;

namespace JatanWebApp.SignalR
{
    public class GameHub : Hub
    {
        private static GameManager _gameManager;

        static GameHub()
        {
            _gameManager = DoInitialPlacements();
        }

        public void SendChatMessage(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }

        /// <summary>
        /// Sends a full game manager update to the calling client.
        /// A full update includes constants like resources tiles and port locations.
        /// </summary>
        public void GetGameManagerUpdate(bool fullUpdate)
        {
            var callerPlayerId = 0; // TODO: Get real ID
            var managerDto = new GameManagerDTO(_gameManager, callerPlayerId, fullUpdate);
            Clients.Caller.updateGameManager(managerDto);
        }

        private static GameManager DoInitialPlacements()
        {
            // This setup method will create a 3-player game with the center and far-right hexagons fully surrounded.

            int PLAYER_0 = 0;
            int PLAYER_1 = 1;
            int PLAYER_2 = 2;

            var manager = new GameManager();
            manager.AddPlayer("Billy"); // PLAYER_0
            manager.AddPlayer("John"); // PLAYER_1
            manager.AddPlayer("Greg"); // PLAYER_2

            manager.StartNewGame();

            // player 0
            manager.PlayerPlaceBuilding(PLAYER_0, BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.Top));
            manager.PlayerPlaceRoad(PLAYER_0, Hexagon.Zero.GetEdge(EdgeDir.TopRight));

            // player 1
            manager.PlayerPlaceBuilding(PLAYER_1, BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.BottomRight));
            manager.PlayerPlaceRoad(PLAYER_1, Hexagon.Zero.GetEdge(EdgeDir.Right));

            // player 2
            manager.PlayerPlaceBuilding(PLAYER_2, BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.BottomLeft));
            manager.PlayerPlaceRoad(PLAYER_2, Hexagon.Zero.GetEdge(EdgeDir.Left));

            // Create around a different hexagon since the middle is filled up.
            Hexagon otherHex = new Hexagon(2, 0);

            manager.PlayerPlaceBuilding(PLAYER_2, BuildingTypes.Settlement, otherHex.GetPoint(PointDir.BottomLeft));
            manager.PlayerPlaceRoad(PLAYER_2, otherHex.GetEdge(EdgeDir.Left));

            // player 1
            manager.PlayerPlaceBuilding(PLAYER_1, BuildingTypes.Settlement, otherHex.GetPoint(PointDir.BottomRight));
            manager.PlayerPlaceRoad(PLAYER_1, otherHex.GetEdge(EdgeDir.Right));

            // player 0
            manager.PlayerPlaceBuilding(PLAYER_0, BuildingTypes.Settlement, otherHex.GetPoint(PointDir.Top));
            manager.PlayerPlaceRoad(PLAYER_0, otherHex.GetEdge(EdgeDir.TopRight));

            return manager;
        }
    }
}