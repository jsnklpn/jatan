using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Models
{
    /// <summary>
    /// Struct to represent a player building (settlement, city).
    /// </summary>
    public struct Building
    {
        /// <summary>
        /// The player that owns this building.
        /// </summary>
        public readonly int PlayerId;

        /// <summary>
        /// The type of building.
        /// </summary>
        public readonly BuildingTypes Type;

        /// <summary>
        /// Creates a new building
        /// </summary>
        public Building(int playerId, BuildingTypes type)
        {
            PlayerId = playerId;
            Type = type;
        }
    }
}
