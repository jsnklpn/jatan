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

        public void DiceRoll(int turn, int playerId, RollResult roll)
        {
            if (!_playerMap.ContainsKey(playerId) || roll == null) return;
            LogItems.Add(new DiceRollLogItem(turn, _playerMap[playerId], roll.Copy()));
        }

        public void ResourceCollection(int turn, int playerId, ResourceCollection resources)
        {
            if (!_playerMap.ContainsKey(playerId) || resources == null) return;
            LogItems.Add(new ResourceCollectionLogItem(turn, _playerMap[playerId], resources.Copy()));
        }

        public void CardStolen(int turn, int thiefId, int victimId, ResourceTypes cardStolen)
        {
            if (!_playerMap.ContainsKey(thiefId) || !_playerMap.ContainsKey(victimId)) return;
            LogItems.Add(new CardStolenLogItem(turn, _playerMap[thiefId], _playerMap[victimId], cardStolen));
        }

        public void TurnStarted(int turn, int playerId)
        {
            if (!_playerMap.ContainsKey(playerId)) return;
            LogItems.Add(new PlayerTurnStateLogItem(turn, _playerMap[playerId], PlayerTurnStateLogItem.TurnStates.Start));
        }

        public void TurnEnded(int turn, int playerId)
        {
            if (!_playerMap.ContainsKey(playerId)) return;
            LogItems.Add(new PlayerTurnStateLogItem(turn, _playerMap[playerId], PlayerTurnStateLogItem.TurnStates.End));
        }

        public void AbandonGame(int turn, int playerId)
        {
            if (!_playerMap.ContainsKey(playerId)) return;
            TurnEnded(turn, playerId);
            LogItems.Add(new PlayerAbandonLogItem(turn, _playerMap[playerId]));
        }

        public void CardsLost(int turn, int playerId, ResourceCollection cardsLost)
        {
            if (!_playerMap.ContainsKey(playerId) || cardsLost == null) return;
            LogItems.Add(new CardsLostLogItem(turn, _playerMap[playerId], cardsLost.Copy()));
        }

        public void BankTrade(int turn, int playerId, TradeOffer trade)
        {
            if (!_playerMap.ContainsKey(playerId)) return;
            LogItems.Add(new BankTradeLogItem(turn, _playerMap[playerId], trade.Copy()));
        }

        public void PlayerTrade(int turn, int creatorId, int acceptorId, TradeOffer trade)
        {
            if (!_playerMap.ContainsKey(creatorId) || !_playerMap.ContainsKey(acceptorId)) return;
            LogItems.Add(new PlayerTradeLogItem(turn, _playerMap[creatorId], _playerMap[acceptorId], trade.Copy()));
        }

        #endregion
    }

    #region Helper classes

    public abstract class LogItem
    {
        public DateTime TimeStampUtc { get; protected set; }
        public int Turn { get; protected set; }
        protected LogItem(int turn)
        {
            this.TimeStampUtc = DateTime.UtcNow;
        }
    }

    public abstract class PlayerLogItem : LogItem
    {
        public Player Player { get; protected set; }
        protected PlayerLogItem(int turn, Player player) : base(turn)
        {
            this.Player = player;
        }
    }

    public class ResourceCollectionLogItem : PlayerLogItem
    {
        public ResourceCollection ResourcesCollected { get; protected set; }
        public ResourceCollectionLogItem(int turn, Player player, ResourceCollection resourcesCollected)
            : base(turn, player)
        {
            this.ResourcesCollected = resourcesCollected;
        }
    }

    public class CardsLostLogItem : PlayerLogItem
    {
        public ResourceCollection CardsLost { get; protected set; }
        public CardsLostLogItem(int turn, Player player, ResourceCollection cardsLost)
            : base(turn, player)
        {
            this.CardsLost = cardsLost;
        }
    }

    public class DiceRollLogItem : PlayerLogItem
    {
        public RollResult Roll { get; protected set; }
        public DiceRollLogItem(int turn, Player player, RollResult roll)
            : base(turn, player)
        {
            this.Roll = roll;
        }
    }

    public class CardStolenLogItem : PlayerLogItem
    {
        public Player Victim { get; protected set; }
        public ResourceTypes CardStolen { get; protected set; }
        public CardStolenLogItem(int turn, Player thief, Player victim, ResourceTypes cardStolen)
            : base(turn, thief)
        {
            this.Victim = victim;
            this.CardStolen = cardStolen;
        }
    }

    public class PlayerTurnStateLogItem : PlayerLogItem
    {
        public enum TurnStates { Start, End }
        public TurnStates TurnState { get; protected set; }
        public PlayerTurnStateLogItem(int turn, Player player, TurnStates state)
            : base(turn, player)
        {
            this.TurnState = state;
        }
    }

    public class PlayerAbandonLogItem : PlayerLogItem
    {
        public PlayerAbandonLogItem(int turn, Player player)
            : base(turn, player)
        {
        }
    }

    public class BankTradeLogItem : PlayerLogItem
    {
        public TradeOffer Trade { get; protected set; }
        public BankTradeLogItem(int turn, Player player, TradeOffer trade)
            : base(turn, player)
        {
            this.Trade = trade;
        }
    }

    public class PlayerTradeLogItem : PlayerLogItem
    {
        public Player Acceptor { get; protected set; }
        public TradeOffer Trade { get; protected set; }
        public PlayerTradeLogItem(int turn, Player tradeCreator, Player tradeAcceptor, TradeOffer trade)
            : base(turn, tradeCreator)
        {
            this.Acceptor = tradeAcceptor;
            this.Trade = trade;
        }
    }

    #endregion
}
