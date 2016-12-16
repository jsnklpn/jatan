using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jatan.Models;

namespace Jatan.GameLogic
{
    /// <summary>
    /// Helper class to track current trade offers.
    /// </summary>
    internal class TradeHelper
    {
        /// <summary>
        /// The current offer from the active player.
        /// </summary>
        public TradeOffer ActivePlayerTradeOffer { get; set; }

        /// <summary>
        /// The list of counter offers.
        /// </summary>
        public List<TradeOffer> CounterOffers { get; private set; }

        /// <summary>
        /// Returns true if the active player is currently proposing a trade.
        /// </summary>
        public bool HasActivePlayerTradeOffer
        {
            get { return ActivePlayerTradeOffer != null; }
        }

        /// <summary>
        /// Creates a trade helper instance.
        /// </summary>
        public TradeHelper()
        {
            ActivePlayerTradeOffer = null;
            CounterOffers = new List<TradeOffer>();
        }

        /// <summary>
        /// Send a counter-offer to the active player.
        /// </summary>
        public void SendCounterOffer(TradeOffer counterOffer)
        {
            int playerId = counterOffer.CreatorPlayerId;
            CancelCounterOffer(playerId);
            CounterOffers.Add(counterOffer);
        }

        /// <summary>
        /// Cancels a counter offer with the current player.
        /// </summary>
        public void CancelCounterOffer(int playerId)
        {
            CounterOffers.RemoveAll(o => o.CreatorPlayerId == playerId);
        }

        /// <summary>
        /// Gets a counter offer by the other player's Id.
        /// </summary>
        public TradeOffer GetCounterOfferByPlayerId(int playerId)
        {
            return CounterOffers.FirstOrDefault(o => o.CreatorPlayerId == playerId);
        }

        /// <summary>
        /// Clears all current trade offers.
        /// </summary>
        public void ClearAllOffers()
        {
            ActivePlayerTradeOffer = null;
            CounterOffers.Clear();
        }
    }
}
