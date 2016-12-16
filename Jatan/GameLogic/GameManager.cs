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
        private GameSettings _gameSettings;
        private List<Player> _players;
        private int _playerTurnIndex;
        private CardDeck<DevelopmentCards> _developmentCardDeck;
        private GameStates _gameState;
        private PlayerTurnState _playerTurnState;
        private Dice _dice;

        // <playerId, roadLength>
        private Tuple<int, int> _longestRoad; 
        // <playerId, armySize>
        private Tuple<int, int> _largestArmy;

        #region public properties

        /// <summary>
        /// Gets the game board.
        /// </summary>
        public GameBoard GameBoard
        {
            get { return _gameBoard; }
        }

        /// <summary>
        /// Gets the game settings.
        /// </summary>
        public GameSettings Settings
        {
            get {  return _gameSettings; }
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
        public GameStates GameState
        {
            get { return _gameState; }
        }

        /// <summary>
        /// Gets the state of the active player's turn.
        /// </summary>
        public PlayerTurnState PlayerTurnState
        {
            get { return _playerTurnState; }
        }

        /// <summary>
        /// Gets the player whose turn it currently is.
        /// </summary>
        public Player ActivePlayer
        {
            get { return _players[_playerTurnIndex]; }
        }

        /// <summary>
        /// Returns true if the last player has placed a building during the initial placement phase.
        /// </summary>
        public bool LastPlayerHasPlaced
        {
            get { return _gameBoard.GetBuildingCountForPlayer(_players.Last().Id) > 0; }
        }

        /// <summary>
        /// Returns true if the "last" player is currently active.
        /// </summary>
        public bool LastPlayerIsActive
        {
            get { return _playerTurnIndex == _players.Count - 1; }
        }

        /// <summary>
        /// Gets a copy of the player list.
        /// </summary>
        public List<Player> Players
        {
            get { return new List<Player>(_players); }
        }

        #endregion

        /// <summary>
        /// Creates a new game instance.
        /// </summary>
        public GameManager()
        {
            _gameSettings = new GameSettings();
            _gameState = GameStates.NotStarted;
            _playerTurnState = PlayerTurnState.None;
            _gameBoard = new GameBoard();
            _players = new List<Player>();
            _developmentCardDeck = new CardDeck<DevelopmentCards>();
            _dice = new Dice();
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
            _playerTurnIndex = 0; // The first person in the list goes first.
            _longestRoad = Tuple.Create(-1, -1);
            _largestArmy = Tuple.Create(-1, -1);
            SetupDevelopmentCards();
            _gameBoard.Setup();
            _gameBoard.RobberMode = _gameSettings.RobberMode;
            _dice.ClearLog();
            _gameState = GameStates.InitialPlacement;
            _playerTurnState = PlayerTurnState.PlacingBuilding; // TODO: Possibly wait for something to trigger the game start.
        }

        private int _idCounter;
        public void AddPlayer(string name)
        {
            _players.Add(new Player(_idCounter++, name, 0));
        }

        #region public player methods

        public ActionResult<int> PlayerRollDice(int playerId)
        {
            var validation = ValidatePlayerAction(PlayerTurnState.NeedToRoll, playerId);
            if (validation.Failed) return validation.ToGeneric<int>();

            var diceRoll = _dice.Roll();

            // If the roll is a 7, then we do special logic to make players
            // lose cards and we let the active player place the robber.
            if (diceRoll == 7)
            {
                // Possibly make players lose cards
                // TODO: Check which players should lose cards and let them decide which

                // Set the PlacingRobber player state.
                if (_gameSettings.RobberMode != RobberMode.None)
                {
                    // TODO: This will need to work even if some players have to lose cards.
                    //_playerTurnState = PlayerTurnState.PlacingRobber;
                }

                // TODO: Remove this. Temporarily skip the 7 logic.
                _playerTurnState = PlayerTurnState.TakeAction;
            }
            else
            {
                // Give resources
                var allResources = _gameBoard.GetResourcesForDiceRoll(diceRoll);
                foreach (var resources in allResources)
                {
                    var playerResult = GetPlayerFromId(resources.Key);
                    if (playerResult.Succeeded && playerResult.Data != null)
                    {
                        var player = playerResult.Data;
                        var newResourcesForPlayer = resources.Value;
                        player.AddResourceCollection(newResourcesForPlayer);
                    }
                }

                // Nothing special to do, just advance to next turn state.
                _playerTurnState = PlayerTurnState.TakeAction;
            }

            return ActionResult<int>.CreateSuccess(diceRoll);
        }

        public ActionResult PlayerOfferTrade(int playerId, ResourceStack toGive, ResourceStack toReceive)
        {
            var validation = ValidatePlayerAction(PlayerTurnState.TakeAction, playerId);
            if (validation.Failed) return validation;

            // TODO
            return ActionResult.CreateFailed("Not implemented");
        }

        public ActionResult PlayerAcceptCurrentTrade(int playerId)
        {
            var validation = ValidatePlayerAction(PlayerTurnState.RequestingTrade, playerId);
            if (validation.Failed) return validation;

            // TODO
            return ActionResult.CreateFailed("Not implemented");
        }

        public ActionResult PlayerTradeWithBank(int playerId, ResourceStack toGive, ResourceStack toReceive)
        {
            var validation = ValidatePlayerAction(PlayerTurnState.TakeAction, playerId);
            if (validation.Failed) return validation;

            // TODO
            return ActionResult.CreateFailed("Not implemented");
        }

        /// <summary>
        /// Makes a player buy a development card and places it into their hand. Returns the bought card.
        /// </summary>
        public ActionResult<DevelopmentCards> PlayerBuyDevelopmentCard(int playerId)
        {
            var validation = ValidatePlayerAction(PlayerTurnState.TakeAction, playerId);
            if (validation.Failed) return validation.ToGeneric<DevelopmentCards>();

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
        /// If it's the initial placement phase of the game, the road is free.
        /// Otherwise, resources will be removed from the player.
        /// </summary>
        public ActionResult PlayerPlaceRoad(int playerId, HexEdge location)
        {
            var validation = ValidatePlayerAction(PlayerTurnState.PlacingRoad, playerId);
            if (validation.Failed) return validation;

            var pr = GetPlayerFromId(playerId);
            if (pr.Failed) return pr;
            var player = pr.Data;

            bool startOfGame = (_gameState == GameStates.InitialPlacement);

            // Make sure the player doesn't place too many roads in the intial placement phase
            if (startOfGame)
            {
                var buildingCount = _gameBoard.GetBuildingCountForPlayer(playerId);
                var roadCount = _gameBoard.GetRoadCountForPlayer(playerId);
                var maxRoads = buildingCount;
                if (roadCount >= maxRoads)
                    return ActionResult.CreateFailed("Cannot place more than 1 road per settlement during the initial placement phase.");
            }

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
                // Must be at least 5 (by default) roads to have the longest road.
                if (newRoadLength >= _gameSettings.MinimumLongestRoad && newRoadLength > _longestRoad.Item2)
                    _longestRoad = Tuple.Create(playerId, newRoadLength);
            }

            // Update game and player states.
            if (_gameState == GameStates.InitialPlacement)
            {
                var buildingCount = _gameBoard.GetBuildingCountForPlayer(playerId);
                if (LastPlayerIsActive && buildingCount == 1)
                {
                    // The "last" player gets to place twice.
                    _playerTurnState = PlayerTurnState.PlacingBuilding;
                }
                else
                {
                    // Go to the next players turn.
                    AdvanceToNextPlayerTurn();
                }
            }
            else if (_gameState == GameStates.GameInProgress)
            {
                _playerTurnState = PlayerTurnState.TakeAction;
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
            var validation = ValidatePlayerAction(PlayerTurnState.PlacingBuilding, playerId);
            if (validation.Failed) return validation;

            var pr = GetPlayerFromId(playerId);
            if (pr.Failed) return pr;
            var player = pr.Data;

            bool startOfGame = (_gameState == GameStates.InitialPlacement);

            // Make sure the player doesn't place too many buildings in the intial placement phase
            if (startOfGame)
            {
                if (type != BuildingTypes.Settlement)
                    return ActionResult.CreateFailed("Can only place settlements during the initial placement phase.");

                var buildingCount = _gameBoard.GetBuildingCountForPlayer(playerId);
                var maxBuildingCount = (LastPlayerHasPlaced) ? 2 : 1;
                if (buildingCount >= maxBuildingCount)
                    return ActionResult.CreateFailed("Cannot place any more settlements during the initial placement phase.");
            }

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

            // Update game and player states.
            if (_gameState == GameStates.InitialPlacement)
            {
                var buildingCount = _gameBoard.GetBuildingCountForPlayer(playerId);
                if (buildingCount == 2)
                {
                    // We've played the second building during the setup phase. Collect resources.
                    var resources = _gameBoard.GetResourcesForBuilding(location, BuildingTypes.Settlement);
                    player.ResourceCards.Add(resources);
                }

                _playerTurnState = PlayerTurnState.PlacingRoad;
            }
            else if (_gameState == GameStates.GameInProgress)
            {
                _playerTurnState = PlayerTurnState.TakeAction;
            }

            return ActionResult.CreateSuccess();
        }

        /// <summary>
        /// Plays a development card for a player and performs any actions due to the card.
        /// </summary>
        public ActionResult PlayerPlayDevelopmentCard(int playerId, DevelopmentCards cardToPlay)
        {
            var validation = ValidatePlayerAction(PlayerTurnState.TakeAction, playerId);
            if (validation.Failed) return validation;

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

        /// <summary>
        /// Ends a players turn during the normal game phase.
        /// </summary>
        public ActionResult PlayerEndTurn(int playerId)
        {
            var validation = ValidatePlayerAction(PlayerTurnState.TakeAction, playerId);
            if (validation.Failed) return validation;

            AdvanceToNextPlayerTurn();

            return ActionResult.CreateSuccess();
        }

        #endregion

        #region private setup methods

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

        #region private helper methods

        private ActionResult ValidatePlayerAction(PlayerTurnState requiredState, int playerId)
        {
            if (ActivePlayer.Id != playerId)
                return ActionResult.CreateFailed("Not allowed to play out of turn.");

            return ValidatePlayerAction(requiredState);
        }

        private ActionResult ValidatePlayerAction(PlayerTurnState requiredState)
        {
            // Validates that the game and player are in the correct states to take the action.

            if (requiredState != _playerTurnState)
                return ActionResult.CreateFailed("Not allowed to take this action at this time.");

            bool validAction = true;
            switch (requiredState)
            {
                // The following actions can be taken during the initial placement phase or during the normal phase.
                case PlayerTurnState.PlacingRoad:
                case PlayerTurnState.PlacingBuilding:
                    validAction = (_gameState == GameStates.GameInProgress || _gameState == GameStates.InitialPlacement);
                    break;
                // The following actions require the normal game phase to be in progress.
                case PlayerTurnState.NeedToRoll:
                case PlayerTurnState.PlacingRobber:
                case PlayerTurnState.RequestingTrade:
                case PlayerTurnState.SelectingCardsToLose:
                case PlayerTurnState.StealingCards:
                case PlayerTurnState.TakeAction:
                    validAction = (_gameState == GameStates.GameInProgress);
                    break;
            }
            return validAction ? ActionResult.CreateSuccess() :
                                 ActionResult.CreateFailed("Not allowed to take this action at this time.");
        }

        private ActionResult<Player> GetPlayerFromId(int playerId)
        {
            var player = _players.FirstOrDefault(p => p.Id == playerId);
            if (player == null)
                return new ActionResult<Player>(null, false, "Player \"" + playerId + "\" does not exist.");
            return new ActionResult<Player>(player, true);
        }

        private void AdvanceToNextPlayerTurn()
        {
            // Advances to the next player's turn.
            if (_gameState == GameStates.GameInProgress)
            {
                _playerTurnIndex++;
                _playerTurnIndex %= _players.Count;
                _playerTurnState = PlayerTurnState.NeedToRoll;
            }
            else if (_gameState == GameStates.InitialPlacement)
            {
                // During the initial placement phase, the turn index increases until the last player
                // makes his placements. Then, the turn index counts down to the first player.
                if (!LastPlayerHasPlaced)
                {
                    // The last player has not placed yet. Count up.
                    _playerTurnIndex++;
                    _playerTurnState = PlayerTurnState.PlacingBuilding;
                }
                else
                {
                    // The last player has gone. Count down.
                    _playerTurnIndex--;
                    
                    // If everyone has placed, start the game.
                    if (_playerTurnIndex < 0)
                    {
                        _playerTurnIndex = 0;
                        _gameState = GameStates.GameInProgress;
                        _playerTurnState = PlayerTurnState.NeedToRoll;
                    }
                    else
                    {
                        _playerTurnState = PlayerTurnState.PlacingBuilding;    
                    }
                }
                
            }
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

            var settlementCount = _gameBoard.GetBuildingCountForPlayer(playerId, BuildingTypes.Settlement);
            var cityCount = _gameBoard.GetBuildingCountForPlayer(playerId, BuildingTypes.City);

            var totalPoints = 0;
            totalPoints += settlementCount;
            totalPoints += (cityCount * 2);
            totalPoints += player.VictoryPointsFromCards;
            if (_largestArmy.Item1 == playerId) totalPoints += 2;
            if (_longestRoad.Item1 == playerId) totalPoints += 2;

            return ActionResult<int>.CreateSuccess(totalPoints);
        }

        #endregion
    }
}
