using Jatan.Core;
using Jatan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.GameLogic
{
    /// <summary>
    /// Helper class for using heuristics to make building and road placements.
    /// </summary>
    public class AiHelper
    {
        /// <summary>
        /// Get best building location based on a multi-variable heuristic.
        /// </summary>
        public static HexPoint? GetBestBuildingPlacement(int playerId, GameBoard board, bool startOfGame = true)
        {
            var placements = board.GetBuildingPlacementsForPlayer(playerId, BuildingTypes.Settlement, startOfGame);
            if (placements.Count == 0) return null;

            var ranks = ComputeBuildingRanks(playerId, board, placements);

            float bestRank = ranks.Values.Max();
            return ranks.First(r => r.Value == bestRank).Key;
        }

        /// <summary>
        /// Get best road location based on a multi-variable heuristic.
        /// </summary>
        public static HexEdge? GetBestRoadPlacement(int playerId, GameBoard board, bool startOfGame = true)
        {
            var placements = board.GetRoadPlacementsForPlayer(playerId, startOfGame);
            if (placements.Count == 0) return null;

            var ranks = ComputeRoadRanks(playerId, board, placements);

            float bestRank = ranks.Values.Max();
            return ranks.First(r => r.Value == bestRank).Key;
        }

        private static Dictionary<HexPoint, float> ComputeBuildingRanks(int playerId, GameBoard board, IEnumerable<HexPoint> placements)
        {
            var ranks = placements.ToDictionary(p => p, p => 0f);
            foreach (var loc in placements)
            {
                var playerState = new PlayerState(playerId, board);
                playerState.AddBuilding(loc, board);
                var newRank = RankPlayerState(playerState);
                ranks[loc] = newRank;
            }
            return ranks;
        }

        private static Dictionary<HexEdge, float> ComputeRoadRanks(int playerId, GameBoard board, IEnumerable<HexEdge> placements)
        {
            var ranks = placements.ToDictionary(p => p, p => 0f);
            foreach (var loc in placements)
            {
                var playerState = new PlayerState(playerId, board);
                playerState.AddRoad(loc);
                var newRank = RankPlayerState(playerState);
                ranks[loc] = newRank;
            }
            return ranks;
        }

        private static float RankPlayerState(PlayerState state)
        {
            float rank = 0f;
            foreach (var building in state.CurrentBuildings)
            {
                foreach (var tile in building.ResourcesTiles)
                {
                    if (tile.Resource == ResourceTypes.None) continue;

                    // Bonus rank for brick and wood
                    if (tile.Resource == ResourceTypes.Brick || tile.Resource == ResourceTypes.Wood)
                        rank += 2f;

                    // The roll chance is the biggest rank bonus
                    rank += GetRollPercentage(tile.RetrieveNumber) * 100;
                }
            }

            // Rank penalty for resources below average
            int allResCount = state.GetAllResourceTiles().Count(t => t.Resource != ResourceTypes.None);
            float avgResCount = allResCount / 5f; // 5 resource types
            var resCol = state.GetResourceCollection();
            rank += Math.Min(0, resCol[ResourceTypes.Brick] - avgResCount);
            rank += Math.Min(0, resCol[ResourceTypes.Wood] - avgResCount);
            rank += Math.Min(0, resCol[ResourceTypes.Ore] - avgResCount);
            rank += Math.Min(0, resCol[ResourceTypes.Wheat] - avgResCount);
            rank += Math.Min(0, resCol[ResourceTypes.Sheep] - avgResCount);

            // TODO: Rank road placements

            return rank;
        }

        private static float GetRollPercentage(int num)
        {
            switch (num)
            {
                case 2:
                case 12:
                    return 0.028f;
                case 3:
                case 11:
                    return 0.056f;
                case 4:
                case 10:
                    return 0.083f;
                case 5:
                case 9:
                    return 0.11f;
                case 6:
                case 8:
                    return 0.14f;
                default:
                    return 0f;
            }
        }

        private class PlayerState
        {
            public int PlayerId { get; }
            public List<BuildingInfo> CurrentBuildings { get; }
            public List<HexEdge> CurrentRoads { get; }
            public PlayerState(int playerId, GameBoard board)
            {
                this.PlayerId = playerId;
                this.CurrentBuildings = board.GetBuildingLocationsForPlayer(playerId)
                    .Select(loc => new BuildingInfo(loc, board))
                    .ToList();
                this.CurrentRoads = board.GetRoadLocationsForPlayer(playerId);
            }
            public void AddBuilding(HexPoint loc, GameBoard board)
            {
                this.CurrentBuildings.Add(new BuildingInfo(loc, board));
            }
            public void AddRoad(HexEdge loc)
            {
                if (!this.CurrentRoads.Contains(loc))
                    this.CurrentRoads.Add(loc);
            }
            public List<ResourceTile> GetAllResourceTiles()
            {
                return CurrentBuildings.SelectMany(b => b.ResourcesTiles).ToList();
            }
            public ResourceCollection GetResourceCollection()
            {
                var col = new ResourceCollection();
                foreach (var resTile in GetAllResourceTiles())
                {
                    col[resTile.Resource]++;
                }
                return col;
            }
        }

        private class BuildingInfo
        {
            public HexPoint Location { get; }
            public List<ResourceTile> ResourcesTiles { get; }
            public BuildingInfo(HexPoint loc, GameBoard board)
            {
                this.Location = loc;
                this.ResourcesTiles = board.ResourceTiles
                    .Where(p => p.Key.ContainsPoint(loc))
                    .Select(p => p.Value)
                    .ToList();
            }
        }
    }
}
