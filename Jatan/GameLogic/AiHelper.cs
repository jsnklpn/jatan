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

            foreach (var placement in ranks)
            {

            }

            return ranks;
        }

        private static Dictionary<HexEdge, float> ComputeRoadRanks(int playerId, GameBoard board, IEnumerable<HexEdge> placements)
        {
            var ranks = placements.ToDictionary(p => p, p => 0f);

            foreach (var placement in ranks)
            {

            }

            return ranks;
        }
    }
}
