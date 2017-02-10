using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.Core;
using Jatan.GameLogic;
using Jatan.Models;
using JatanWebApp.Helpers;

namespace JatanWebApp.SignalR.DTO
{
    /// <summary>
    /// GameManager data transfer object.
    /// </summary>
    public class GameManagerDTO
    {
        public int MyPlayerId { get; set; }
        public GameBoardDTO GameBoard { get; set; }
        public GameState GameState { get; set; }
        public PlayerTurnState PlayerTurnState { get; set; }
        public int ActivePlayerId { get; set; }
        public RollResult CurrentDiceRoll { get; set; }
        public List<PlayerDTO> Players { get; set; }
        public TradeOffer ActiveTradeOffer { get; set; }
        public List<TradeOffer> CounterTradeOffers { get; set; }
        // The Unix epoch time when the current player's turn will expire. 0 if none.
        public long TurnExpire { get; set; } 

        // These properties are populated only when needed.
        public List<HexEdge> ValidRoadPlacements { get; set; }
        public List<HexPoint> ValidSettlementPlacements { get; set; }
        public List<HexPoint> ValidCityPlacements { get; set; }

        public GameManagerDTO(GameLobby lobby, int requestingPlayerId, bool includeBoardConstants, bool includeAvatarPaths)
        {
            var manager = lobby.GameManager;

            this.MyPlayerId = requestingPlayerId;
            this.GameBoard = new GameBoardDTO(manager.GameBoard, includeBoardConstants);
            this.GameState = manager.GameState;
            this.PlayerTurnState = manager.PlayerTurnState;
            this.ActivePlayerId = (manager.ActivePlayer != null) ? manager.ActivePlayer.Id : -1;
            this.CurrentDiceRoll = manager.CurrentDiceRoll;
            this.Players = new List<PlayerDTO>();
            foreach (var p in manager.Players.OrderBy(p => p.Id))
            {
                var playerDto = new PlayerDTO(p, (p.Id == requestingPlayerId));
                if (includeAvatarPaths)
                {
                    playerDto.AvatarPath = lobby.AvatarPaths.ContainsKey(p.Name) ? lobby.AvatarPaths[p.Name] : null;
                }
                playerDto.AvailableToRob = manager.PlayersAvailableToRob.Select(r => r.Id).Contains(p.Id);
                if (manager.PlayersSelectingCardsToLose.Select(r => r.Id).Contains(p.Id))
                {
                    var count = p.ResourceCards.GetResourceCount();
                    var toLose = (int) Math.Floor(count/2d);
                    playerDto.CardsToLose = toLose;
                }
                // Find the ports owned by this player.
                var buildingLocations = manager.GameBoard.GetBuildingLocationsForPlayer(p.Id);
                playerDto.PortsOwned =
                    manager.GameBoard.Ports.Where(
                        t => buildingLocations.Contains(t.Key.GetPoints()[0]) ||
                             buildingLocations.Contains(t.Key.GetPoints()[1]))
                        .Select(t => t.Value.Resource).Distinct().ToList();

                this.Players.Add(playerDto);
            }

            this.ActiveTradeOffer = manager.ActivePlayerTradeOffer;
            this.CounterTradeOffers = manager.CounterTradeOffers;

            // Check if we need to send valid item placements
            if (requestingPlayerId == this.ActivePlayerId)
            {
                if (manager.PlayerTurnState == PlayerTurnState.PlacingRoad ||
                    manager.PlayerTurnState == PlayerTurnState.RoadBuildingSelectingRoads)
                {
                    ValidRoadPlacements = manager.GetLegalRoadPlacements(requestingPlayerId);
                }
                if (manager.PlayerTurnState == PlayerTurnState.PlacingSettlement)
                {
                    ValidSettlementPlacements = manager.GetLegalBuildingPlacements(requestingPlayerId, BuildingTypes.Settlement);
                }
                if (manager.PlayerTurnState == PlayerTurnState.PlacingCity)
                {
                    ValidCityPlacements = manager.GetLegalBuildingPlacements(requestingPlayerId, BuildingTypes.City);
                }
            }

            var expiration = manager.TurnTimerExpiration;
            this.TurnExpire = expiration == DateTime.MinValue ? 0 : expiration.ToUnixTimestamp();
        }
    }
}