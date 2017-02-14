using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jatan.Core;
using Jatan.Models;

namespace Jatan.GameLogic
{
    /// <summary>
    /// Class to log game actions.
    /// </summary>
    public class HistoryLog
    {
        /// <summary>
        /// The list of all actions logged during the game.
        /// </summary>
        public List<LogItem> LogItems { get; private set; }

        // Save a map of players to make it easy to lookup a player by Id
        private Dictionary<int, Player> _playerMap;

        /// <summary>
        /// HistoryLog constructor
        /// </summary>
        public HistoryLog(IEnumerable<Player> players)
        {
            _playerMap = new Dictionary<int, Player>();
            foreach (var player in players)
                _playerMap[player.Id] = player;

            LogItems = new List<LogItem>();
        }

        #region Log methods

        public void DiceRoll(int playerId, RollResult roll)
        {
            if (!_playerMap.ContainsKey(playerId) || roll == null) return;
            LogItems.Add(new DiceRollLogItem(_playerMap[playerId], roll.Copy()));
        }

        public void ResourceCollection(int playerId, ResourceCollection resources)
        {
            if (!_playerMap.ContainsKey(playerId) || resources == null) return;
            LogItems.Add(new ResourceCollectionLogItem(_playerMap[playerId], resources.Copy()));
        }

        public void CardStolen(int thiefId, int victimId, ResourceTypes cardStolen)
        {
            if (!_playerMap.ContainsKey(thiefId) || !_playerMap.ContainsKey(victimId)) return;
            LogItems.Add(new CardStolenLogItem(_playerMap[thiefId], _playerMap[victimId], cardStolen));
        }

        public void TurnStarted(int playerId)
        {
            if (!_playerMap.ContainsKey(playerId)) return;
            LogItems.Add(new PlayerTurnStateLogItem(_playerMap[playerId], PlayerTurnStateLogItem.TurnStates.Start));
        }

        public void TurnEnded(int playerId)
        {
            if (!_playerMap.ContainsKey(playerId)) return;
            LogItems.Add(new PlayerTurnStateLogItem(_playerMap[playerId], PlayerTurnStateLogItem.TurnStates.End));
        }

        public void AbandonGame(int playerId)
        {
            if (!_playerMap.ContainsKey(playerId)) return;
            TurnEnded(playerId);
            LogItems.Add(new PlayerAbandonLogItem(_playerMap[playerId]));
        }

        public void CardsLost(int playerId, ResourceCollection cardsLost)
        {
            if (!_playerMap.ContainsKey(playerId) || cardsLost == null) return;
            LogItems.Add(new CardsLostLogItem(_playerMap[playerId], cardsLost.Copy()));
        }

        public void BankTrade(int playerId, TradeOffer trade)
        {
            if (!_playerMap.ContainsKey(playerId)) return;
            LogItems.Add(new BankTradeLogItem(_playerMap[playerId], trade.Copy()));
        }

        public void PlayerTrade(int creatorId, int acceptorId, TradeOffer trade)
        {
            if (!_playerMap.ContainsKey(creatorId) || !_playerMap.ContainsKey(acceptorId)) return;
            LogItems.Add(new PlayerTradeLogItem(_playerMap[creatorId], _playerMap[acceptorId], trade.Copy()));
        }

        #endregion
    }

    #region Helper classes

    public abstract class LogItem
    {
        public DateTime TimeStampUtc { get; protected set; }
        protected LogItem()
        {
            this.TimeStampUtc = DateTime.UtcNow;
        }
    }

    public abstract class PlayerLogItem : LogItem
    {
        public Player Player { get; protected set; }
        protected PlayerLogItem(Player player)
        {
            this.Player = player;
        }
    }

    public class ResourceCollectionLogItem : PlayerLogItem
    {
        public ResourceCollection ResourcesCollected { get; protected set; }
        public ResourceCollectionLogItem(Player player, ResourceCollection resourcesCollected)
            : base(player)
        {
            this.ResourcesCollected = resourcesCollected;
        }
    }

    public class CardsLostLogItem : PlayerLogItem
    {
        public ResourceCollection CardsLost { get; protected set; }
        public CardsLostLogItem(Player player, ResourceCollection cardsLost)
            : base(player)
        {
            this.CardsLost = cardsLost;
        }
    }

    public class DiceRollLogItem : PlayerLogItem
    {
        public RollResult Roll { get; protected set; }
        public DiceRollLogItem(Player player, RollResult roll)
            : base(player)
        {
            this.Roll = roll;
        }
    }

    public class CardStolenLogItem : PlayerLogItem
    {
        public Player Victim { get; protected set; }
        public ResourceTypes CardStolen { get; protected set; }
        public CardStolenLogItem(Player thief, Player victim, ResourceTypes cardStolen)
            : base(thief)
        {
            this.Victim = victim;
            this.CardStolen = cardStolen;
        }
    }

    public class PlayerTurnStateLogItem : PlayerLogItem
    {
        public enum TurnStates { Start, End }
        public TurnStates TurnState { get; protected set; }
        public PlayerTurnStateLogItem(Player player, TurnStates state)
            : base(player)
        {
            this.TurnState = state;
        }
    }

    public class PlayerAbandonLogItem : PlayerLogItem
    {
        public PlayerAbandonLogItem(Player player) : base(player)
        {
        }
    }

    public class BankTradeLogItem : PlayerLogItem
    {
        public TradeOffer Trade { get; protected set; }
        public BankTradeLogItem(Player player, TradeOffer trade) : base(player)
        {
            this.Trade = trade;
        }
    }

    public class PlayerTradeLogItem : PlayerLogItem
    {
        public Player Acceptor { get; protected set; }
        public TradeOffer Trade { get; protected set; }
        public PlayerTradeLogItem(Player tradeCreator, Player tradeAcceptor, TradeOffer trade)
            : base(tradeCreator)
        {
            this.Acceptor = tradeAcceptor;
            this.Trade = trade;
        }
    }

    #endregion
}
