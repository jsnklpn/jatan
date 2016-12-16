using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Models
{
    /// <summary>
    /// Represents a trade offer.
    /// </summary>
    public class TradeOffer
    {
        public readonly int CreatorPlayerId;

        /// <summary>
        /// The resouce to give.
        /// </summary>
        public ResourceStack ToGive { get; private set; }

        /// <summary>
        /// The resource to recieve.
        /// </summary>
        public ResourceStack ToReceive { get; private set; }

        /// <summary>
        /// Returns true if this is a valid trade.
        /// </summary>
        public bool IsValid
        {
            get { return ToGive.Count > 0 && ToReceive.Count > 0; }
        }

        /// <summary>
        /// Creates a new trade offer.
        /// </summary>
        public TradeOffer(int playerId, ResourceStack toGive, ResourceStack toReceive)
        {
            this.CreatorPlayerId = playerId;
            this.ToGive = toGive;
            this.ToReceive = toReceive;
        }
    }
}
