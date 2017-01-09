using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Models
{
    /// <summary>
    /// A struct to represent a road. Each player gets 15.
    /// </summary>
    public struct Road
    {
        /// <summary>
        /// The player that owns this road.
        /// </summary>
        public readonly int PlayerId;

        /// <summary>
        /// Creates a new road.
        /// </summary>
        public Road(int playerId)
        {
            PlayerId = playerId;
        }
    }
}
