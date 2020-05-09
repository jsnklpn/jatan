using Jatan.Core;
using Jatan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.GameLogic
{
    public class AiHelper
    {
        /// <summary>
        /// Get building location rankings based on a multi-variable heuristic.
        /// </summary>
        public static Dictionary<HexPoint, float> GetBuildingRanks(int playerId, GameBoard board, bool startOfGame = true)
        {
            var placements = board.GetBuildingPlacementsForPlayer(playerId, BuildingTypes.Settlement, startOfGame);

            var ranks = placements.ToDictionary(p => p, p => 1.0f);

            return ranks;
        }
    }
}
