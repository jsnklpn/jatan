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
    /// An instance of a game of Jatan. This class should contain every bit of information needed for the game.
    /// </summary>
    public class GameManager
    {
        private GameBoard _gameBoard;
        private List<Player> _players;
        private int _playerTurnIndex;
        private CardDeck<DevelopmentCards> _developmentCardDeck;
        private GameStates _gameState;

        // <playerId, roadLength>
        private Tuple<int, int> _longestRoad; 
        // <playerId, armySize>
        private Tuple<int, int> _largestArmy;

        private static Random _random = new Random();

        /// <summary>
        /// Gets the game board.
        /// </summary>
        public GameBoard GameBoard
        {
            get { return _gameBoard; }
        }

        /// <summary>
        /// Gets the number of players.
        /// </summary>
        public int NumberOfPlayers
        {
            get { return _players.Count; }
        }

        /// <summary>
        /// Gets the current state of the game.
        /// </summary>
        public GameStates GameState { get { return _gameState; } }

        /// <summary>
        /// Creates a new game instance.
        /// </summary>
        public GameManager()
        {
            _gameState = GameStates.NotStarted;
            _gameBoard = new GameBoard();
            _players = new List<Player>();
            _developmentCardDeck = new CardDeck<DevelopmentCards>();
        }

        /// <summary>
        /// Starts a brand new game.
        /// </summary>
        public void StartNewGame(IEnumerable<Player> players)
        {
            _players.Clear();
            _players.AddRange(players);
            StartNewGame();
        }

        /// <summary>
        /// Starts a brand new game with the current players.
        /// </summary>
        public void StartNewGame()
        {
            _playerTurnIndex = _random.Next(NumberOfPlayers); // A random player will go first.
            _longestRoad = Tuple.Create(-1, -1);
            _largestArmy = Tuple.Create(-1, -1);
            SetupDevelopmentCards();
            _gameBoard.Setup();
            _gameState = GameStates.InitialPlacement;
        }

        private static int _idCounter;
        public void AddPlayer(string name)
        {
            _players.Add(new Player(_idCounter++, name, 0));
        }

        public ActionResult PlayerOfferTrade(int playerId, ResourceStack toGive, ResourceStack toReceive)
        {
            // TODO
            return ActionResult.CreateFailed("Not implemented");
        }

        public ActionResult PlayerAcceptCurrentTrade(int playerId)
        {
            // TODO
            return ActionResult.CreateFailed("Not implemented");
        }

        public ActionResult PlayerTradeWithBank(int playerId, ResourceStack toGive, ResourceStack toReceive)
        {
            // TODO
            return ActionResult.CreateFailed("Not implemented");
        }

        /// <summary>
        /// Makes a player buy a development card and places it into their hand. Returns the bought card.
        /// </summary>
        public ActionResult<DevelopmentCards> PlayerBuyDevelopmentCard(int playerId)
        {
            var pr = GetPlayerFromId(playerId);
            if (pr.Failed) pr.ToGeneric<DevelopmentCards>();
            var player = pr.Data;

            if (!player.CanAfford(PurchasableItems.DevelopmentCard))
                return ActionResult<DevelopmentCards>.CreateFailed("Cannot afford this.");

            var drawResult = _developmentCardDeck.DrawCard();
            if (drawResult.Failed) return drawResult;
            var card = drawResult.Data;

            var buyResult = player.Purchase(PurchasableItems.DevelopmentCard); // This should work, because we know the player can afford it.
            System.Diagnostics.Debug.Assert(buyResult.Succeeded);

            // Everything worked. Give the card to the player.
            player.DevelopmentCards.Add(card);

            return ActionResult<DevelopmentCards>.CreateSuccess(card);
        }

        /// <summary>
        /// Places a road for the given player at the given location.
        /// If it's the start of the game, the road is free.
        /// Otherwise, resources will be removed from the player.
        /// </summary>
        public ActionResult PlayerPlaceRoad(int playerId, HexEdge location)
        {
            var pr = GetPlayerFromId(playerId);
            if (pr.Failed) return pr;
            var player = pr.Data;

            bool startOfGame = (_gameState == GameStates.InitialPlacement);

            var placementValidation = _gameBoard.ValidateRoadPlacement(playerId, location, startOfGame);
            if (placementValidation.Failed) return placementValidation;

            var purchaseResult = player.Purchase(PurchasableItems.Road, startOfGame);
            if (purchaseResult.Failed) return purchaseResult;

            // We'll assume this succeeds because we already validated it.
            var placement = _gameBoard.PlaceRoad(player.Id, location, startOfGame);
            System.Diagnostics.Debug.Assert(placement.Succeeded);

            // When we place a road, do a check to see if we deserve the longest road.
            if (_longestRoad.Item1 != playerId)
            {
                var newRoadLength = _gameBoard.GetRoadLengthForPlayer(playerId);
                // Must be at least 5 roads to have the longest road.
                if (newRoadLength >= 5 && newRoadLength > _longestRoad.Item2)
                    _longestRoad = Tuple.Create(playerId, newRoadLength);
            }

            return ActionResult.CreateSuccess();
        }

        /// <summary>
        /// Places a building for the given player at the given location.
        /// If it's the start of the game, the building is free.
        /// Otherwise, resources will be removed from the player.
        /// </summary>
        public ActionResult PlayerPlaceBuilding(int playerId, BuildingTypes type, HexPoint location)
        {
            var pr = GetPlayerFromId(playerId);
            if (pr.Failed) return pr;
            var player = pr.Data;

            bool startOfGame = (_gameState == GameStates.InitialPlacement);

            var placementValidation = _gameBoard.ValidateBuildingPlacement(playerId, type, location, startOfGame);
            if (placementValidation.Failed) return placementValidation;

            PurchasableItems itemToBuy = (type == BuildingTypes.City)
                ? PurchasableItems.City
                : PurchasableItems.Settlement;

            var purchaseResult = player.Purchase(itemToBuy, startOfGame);
            if (purchaseResult.Failed) return purchaseResult;

            // We'll assume this succeeds because we already validated it.
            var placement = _gameBoard.PlaceBuilding(player.Id, type, location, startOfGame);
            System.Diagnostics.Debug.Assert(placement.Succeeded);

            return ActionResult.CreateSuccess();
        }

        /// <summary>
        /// Plays a development card for a player and performs any actions due to the card.
        /// </summary>
        public ActionResult PlayerPlayDevelopmentCard(int playerId, DevelopmentCards cardToPlay)
        {
            // TODO: PlayerPlayDevelopmentCard method

            var pr = GetPlayerFromId(playerId);
            if (pr.Failed) return pr;
            var player = pr.Data;

            // Check if the player now has the largest army.
            if (_largestArmy.Item1 != playerId)
            {
                var newArmySize = player.ArmySize;
                // Must be at least 3 knights to have the largest army.
                if (newArmySize >= 3 && newArmySize > _largestArmy.Item2)
                    _largestArmy = Tuple.Create(playerId, newArmySize);
            }

            return ActionResult.CreateSuccess();
        }

        #region Private setup methods

        private void SetupDevelopmentCards()
        {
            _developmentCardDeck.Clear();
            var cardsToAdd = new List<DevelopmentCards>();
            // TODO: I need to double check these card counts. I got them from yahoo answers :)
            cardsToAdd.Add(DevelopmentCards.Knight, 14);
            cardsToAdd.Add(DevelopmentCards.Library, 3);
            cardsToAdd.Add(DevelopmentCards.Palace, 2);
            cardsToAdd.Add(DevelopmentCards.YearOfPlenty, 2);
            cardsToAdd.Add(DevelopmentCards.RoadBuilding, 2);
            cardsToAdd.Add(DevelopmentCards.Monopoly, 2);
            _developmentCardDeck.AddCards(cardsToAdd);
            _developmentCardDeck.Shuffle(5);
        }

        #endregion

        private ActionResult<Player> GetPlayerFromId(int playerId)
        {
            if (_players.All(p => p.Id != playerId))
                return new ActionResult<Player>(null, false, "Player \"" + playerId + "\" does not exist.");
            var player = _players.First(p => p.Id == playerId);
            return new ActionResult<Player>(player, true);
        }

        private ActionResult<int> GetTotalPointsForPlayer(int playerId)
        {
            var pr = GetPlayerFromId(playerId);
            if (pr.Failed) return pr.ToGeneric<int>();
            var player = pr.Data;

            // Ways to get points:
            // * 1 point per settlement.
            // * 2 points per city.
            // * From certain development cards.
            // * Longest Road (2 points) - Must be at least 5 segments.
            // * Largest Army (2 points) - Must be at least 3 knights.

            var totalPoints = 0;
            var settlementCount = _gameBoard.GetBuildingCountForPlayer(playerId, BuildingTypes.Settlement);
            var cityCount = _gameBoard.GetBuildingCountForPlayer(playerId, BuildingTypes.City);

            totalPoints += settlementCount;
            totalPoints += (cityCount * 2);
            totalPoints += player.VictoryPointsFromCards;
            if (_largestArmy.Item1 == playerId) totalPoints += 2;
            if (_longestRoad.Item1 == playerId) totalPoints += 2;

            return ActionResult<int>.CreateSuccess(totalPoints);
        }
    }
}
