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
        /// <summary>
        /// The play that created the offer.
        /// </summary>
        public int CreatorPlayerId { get; set; }

        /// <summary>
        /// The resouce to give.
        /// </summary>
        public ResourceCollection ToGive { get; set; }

        /// <summary>
        /// The resource to recieve.
        /// </summary>
        public ResourceCollection ToReceive { get; set; }

        /// <summary>
        /// Returns true if the trade is not empty.
        /// </summary>
        public bool IsValid
        {
            get { return !ToGive.IsEmpty() && !ToReceive.IsEmpty(); }
        }

        /// <summary>
        /// Returns true if each resource collection only contain 1 resource type.
        /// </summary>
        public bool IsValidBankOffer
        {
            get { return IsValid && ToGive.IsSingleResourceType && ToReceive.IsSingleResourceType; }
        }

        /// <summary>
        /// Creates a new trade offer.
        /// </summary>
        public TradeOffer(int playerId, ResourceCollection toGive, ResourceCollection toReceive)
        {
            this.CreatorPlayerId = playerId;
            this.ToGive = toGive;
            this.ToReceive = toReceive;
        }

        /// <summary>
        /// Creates a new trade offer.
        /// </summary>
        public TradeOffer(int playerId, ResourceStack toGive, ResourceStack toReceive)
        {
            this.CreatorPlayerId = playerId;
            this.ToGive = new ResourceCollection(toGive);
            this.ToReceive = new ResourceCollection(toReceive);
        }
    }
}
