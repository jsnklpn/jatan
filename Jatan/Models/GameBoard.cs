using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jatan.Core;

namespace Jatan.Models
{
    // Guide to hexagons
    // http://www.redblobgames.com/grids/hexagons/

    /* 
     *            0, 2      1, 2      2, 2
     * 
     *      -1, 1      0, 1      1, 1      2, 1
     *      
     * -2, 0     -1, 0      0, 0      1, 0      2, 0
     * 
     *      -2,-1     -1,-1      0,-1      1,-1
     * 
     *           -2,-2     -1,-2      0,-2
     * 
     * =================================================
     * 
     *                  + \  Y axis
     *                     \
     *                      \
     *          _____________\_____________ X axis
     *          -             \           +
     *                         \
     *                        - \
     * 
     */

    /// <summary>
    /// A class to represent the game board
    /// </summary>
    public class GameBoard
    {
        // Serialize a dictionary
        // http://theburningmonk.com/2010/05/net-tips-xml-serialize-or-deserialize-dictionary-in-csharp/

        private Dictionary<Hexagon, ResourceTile> _resourceTiles;
        private Dictionary<HexEdge, Road> _roads;
        private Dictionary<HexPoint, Building> _buildings;
        private Dictionary<HexEdge, Port> _ports;
        private Hexagon _robberLocation;

        // Some useful lists for validation and stuff
        private readonly List<Hexagon> _validBoardHexagons;
        private readonly List<HexEdge> _validBoardEdges;
        private readonly List<HexPoint> _validBoardPoints;
        private readonly List<Hexagon> _waterHexagons;
        private readonly List<HexEdge> _borderEdges;

        private static readonly Random _random = new Random();

        #region Constants
        private static readonly int[] NumberTokens = { 5, 2, 6, 3, 8, 10, 9, 12, 11, 4, 8, 10, 9, 4, 5, 6, 3, 11 };
        private static readonly Hexagon[] OuterHexagons = 
        {
            new Hexagon(0, 2), new Hexagon(-1, 1),
            new Hexagon(-2, 0), new Hexagon(-2, -1),
            new Hexagon(-2, -2), new Hexagon(-1, -2),
            new Hexagon(0, -2), new Hexagon(1, -1),
            new Hexagon(2, 0), new Hexagon(2, 1),
            new Hexagon(2, 2), new Hexagon(1, 2)
        };
        private static readonly Hexagon[] InnerHexagons =
        {
            new Hexagon(0, 1), new Hexagon(-1, 0), new Hexagon(-1, -1), new Hexagon(0, -1), new Hexagon(1, 0), new Hexagon(1, 1)
        };
        #endregion

        #region Public properties

        /// <summary>
        /// Gets the robber location.
        /// </summary>
        public Hexagon RobberLocation
        {
            get { return _robberLocation; }
        }

        /// <summary>
        /// Gets the collection of resource tiles.
        /// </summary>
        public ReadOnlyDictionary<Hexagon, ResourceTile> ResourceTiles
        {
            get { return new ReadOnlyDictionary<Hexagon, ResourceTile>(_resourceTiles); }
        }

        /// <summary>
        /// Gets the collection of roads.
        /// </summary>
        public ReadOnlyDictionary<HexEdge, Road> Roads
        {
            get { return new ReadOnlyDictionary<HexEdge, Road>(_roads); }
        }

        /// <summary>
        /// Gets the collection of buildings.
        /// </summary>
        public ReadOnlyDictionary<HexPoint, Building> Buildings
        {
            get { return new ReadOnlyDictionary<HexPoint, Building>(_buildings); }
        }

        /// <summary>
        /// Gets the collection of ports.
        /// </summary>
        public ReadOnlyDictionary<HexEdge, Port> Ports
        {
            get { return new ReadOnlyDictionary<HexEdge, Port>(_ports); }
        }

        #endregion

        /// <summary>
        /// Creates a new game board.
        /// </summary>
        public GameBoard()
        {
            _resourceTiles = new Dictionary<Hexagon, ResourceTile>();
            _roads = new Dictionary<HexEdge, Road>();
            _buildings = new Dictionary<HexPoint, Building>();
            _ports = new Dictionary<HexEdge, Port>();
            _robberLocation = Hexagon.Zero;

            // Create lists of valid hexagons and points
            _validBoardHexagons = new List<Hexagon>();
            _validBoardHexagons.AddRange(OuterHexagons);
            _validBoardHexagons.AddRange(InnerHexagons);
            _validBoardHexagons.Add(Hexagon.Zero);

            var tmpEdges = new List<HexEdge>();
            foreach (var hex in _validBoardHexagons)
                tmpEdges.AddRange(hex.GetEdges());
            _validBoardEdges = tmpEdges.Distinct().ToList();

            var tmpPoints = new List<HexPoint>();
            foreach (var hex in _validBoardHexagons)
                tmpPoints.AddRange(hex.GetPoints());
            _validBoardPoints = tmpPoints.Distinct().ToList();

            // Gets the list of border water tiles
            _waterHexagons = new List<Hexagon>();
            foreach (var outerHexNeighbor in OuterHexagons)
            {
                // Get the out-of-bounds neighbors of all the outer ring tiles.
                var outerBorderTiles = outerHexNeighbor.GetNeighbors().Where(h => !_validBoardHexagons.Contains(h));
                _waterHexagons.AddRange(outerBorderTiles);
            }
            _waterHexagons = _waterHexagons.Distinct().ToList();

            // Get the edges that border the board.
            _borderEdges = _validBoardEdges.Where(e => _waterHexagons.Contains(e.Hex1) ||
                                                       _waterHexagons.Contains(e.Hex2)).Distinct().ToList();
        }

        /// <summary>
        /// Restores a specific board layout.
        /// </summary>
        public void RestoreBoard(IDictionary<Hexagon, ResourceTile> resourceTiles,
                                 IDictionary<HexEdge, Road> roads,
                                 IDictionary<HexPoint, Building> buildings,
                                 IDictionary<HexEdge, Port> ports,
                                 Hexagon robberLocation)
        {
            _resourceTiles = new Dictionary<Hexagon, ResourceTile>(resourceTiles);
            _roads = new Dictionary<HexEdge, Road>(roads);
            _buildings = new Dictionary<HexPoint, Building>(buildings);
            _ports = new Dictionary<HexEdge, Port>(ports);
            _robberLocation = robberLocation;
        }

        /// <summary>
        /// Does a random initial setup of the board.
        /// </summary>
        public void Setup()
        {
            _roads.Clear();
            _buildings.Clear();

            SetupResourceTiles();
            SetupRobber();
            SetupPorts();
        }

        /// <summary>
        /// Gets a list of player IDs which have buildings on the board.
        /// </summary>
        public IList<int> GetPlayersOnBoard()
        {
            var players = _buildings.Values.Select(b => b.Player).Distinct().ToList();
            players.Sort();
            return players;
        }

        /// <summary>
        /// Returns the resources that all players collect on a given roll.
        /// </summary>
        public Dictionary<int, ResourceCollection> GetResourcesForDiceRoll(int roll)
        {
            // result is Dictionary of <PlayerID, ResourceCollection>
            var result = new Dictionary<int, ResourceCollection>();
            foreach (var playerId in GetPlayersOnBoard())
                result[playerId] = ResourceCollection.Empty;

            var activatedResourceTiles = _resourceTiles.Where(p => p.Value.RetrieveNumber == roll && p.Key != _robberLocation).ToList();
            foreach (var activatedTile in activatedResourceTiles)
            {
                foreach (var point in activatedTile.Key.GetPoints())
                {
                    if (_buildings.ContainsKey(point))
                    {
                        var building = _buildings[point];
                        int resourcesCollected = (building.Type == BuildingTypes.Settlement) ? 1 : 2;
                        var playerCollectedResources = result[building.Player];
                        playerCollectedResources[activatedTile.Value.Resource] += resourcesCollected;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Moves the robber to the given hex location and returns a list of players touching that hex (excluding the player moving it.)
        /// </summary>
        public ActionResult<IList<int>> MoveRobber(int player, Hexagon location)
        {
            if (!IsHexagonInBoard(location))
            {
                return new ActionResult<IList<int>>(null, false, string.Format("The hex location {0} is out of bounds.", location));
            }

            if (_robberLocation == location)
            {
                return new ActionResult<IList<int>>(null, false, string.Format("The hex location {0} already contains the robber.", location));
            }

            _robberLocation = location;

            var points = location.GetPoints();
            var buildingsTouching = _buildings.Where(b => points.Contains(b.Key));
            var players = buildingsTouching.Select(b => b.Value.Player).Distinct().ToList();
            players.Sort();
            return new ActionResult<IList<int>>(players, true);
        }

        /// <summary>
        /// Returns a success result if this is a valid placement for a road.
        /// </summary>
        public ActionResult ValidateRoadPlacement(int player, HexEdge edge, bool startOfGame)
        {
            if (!IsEdgeInBoard(edge))
            {
                return new ActionResult(false, string.Format("The road location {0} is out of bounds.", edge));
            }

            // Do not allow removing a road.
            if (_roads.ContainsKey(edge))
            {
                return new ActionResult(false, string.Format("The road location {0} is already used.", edge));
            }

            if (startOfGame)
            {
                // Make sure this is touching a settlement owned by this player. It's the start of the game.
                if (!IsPlayerBuildingHere(player, edge))
                {
                    return new ActionResult(false, String.Format("The road location {0} is not near a settlement.", edge));
                }
            }
            else
            {
                // We must be near a settlement, city, or another road.
                if (!IsPlayerBuildingHere(player, edge))
                {
                    bool isTouchingRoad = false;
                    foreach (var surroundingRoad in edge.GetNeighborEdges())
                    {
                        if (IsPlayerRoadHere(player, surroundingRoad))
                        {
                            isTouchingRoad = true;
                            break;
                        }
                    }
                    if (!isTouchingRoad)
                    {
                        return new ActionResult(false, string.Format("The road location {0} is not near a city, settlement, or road.", edge));
                    }
                }
            }
            return ActionResult.CreateSuccess();
        }

        /// <summary>
        /// Places a new road onto the board.
        /// </summary>
        public ActionResult PlaceRoad(int player, HexEdge edge, bool startOfGame)
        {
            var validationResult = ValidateRoadPlacement(player, edge, startOfGame);
            if (validationResult.Failed) return validationResult;
            _roads[edge] = new Road(player);
            return ActionResult.CreateSuccess();
        }

        /// <summary>
        /// Returns a success result if this is a valid placement for a building.
        /// </summary>
        public ActionResult ValidateBuildingPlacement(int player, BuildingTypes buildingType, HexPoint point, bool startOfGame)
        {
            if (!IsPointInBoard(point))
            {
                return new ActionResult(false, string.Format("The location {0} is out of bounds.", point));
            }

            if (_buildings.ContainsKey(point))
            {
                if (_buildings[point].Player != player)
                    return new ActionResult(false, string.Format("The location {0} is already used by another player.", point));

                // If this play has something here, make sure that it's not a city, which can't be upgraded.
                if (_buildings[point].Type == BuildingTypes.City)
                    return new ActionResult(false, string.Format("The location {0} already contains a city.", point));

                // If the existing building is a settlement, make sure the building being placed is not a settlement.
                if (buildingType == BuildingTypes.Settlement)
                    return new ActionResult(false, string.Format("The location {0} already contains a settlement.", point));
            }

            // Check that there isn't a neighboring building.
            var buildingNearby = point.GetNeighborPoints().Any(b => _buildings.ContainsKey(b));
            if (buildingNearby)
            {
                return new ActionResult(false, string.Format("The location {0} is too close to another settlement or city.", point));
            }

            if (!startOfGame)
            {
                // Make sure the player has a road nearby.
                bool roadNearby = point.GetNeighborEdges().Any(r => IsPlayerRoadHere(player, r));
                if (!roadNearby)
                {
                    return new ActionResult(false, string.Format("The location {0} is not touching a road.", point));
                }
            }
            return ActionResult.CreateSuccess();
        }

        /// <summary>
        /// Places a new building onto the board. A player is allowed to upgrade a settlement to a city.
        /// </summary>
        public ActionResult PlaceBuilding(int player, BuildingTypes buildingType, HexPoint point, bool startOfGame)
        {
            var validationResult = ValidateBuildingPlacement(player, buildingType, point, startOfGame);
            if (validationResult.Failed) return validationResult;
            _buildings[point] = new Building(player, buildingType);
            return ActionResult.CreateSuccess();
        }

        /// <summary>
        /// Gets a list of all ports for a player.
        /// </summary>
        public IList<Port> GetPortsForPlayer(int player)
        {
            var playerPorts = _ports.Where(e => IsPlayerBuildingHere(player, e.Key))
                                    .Select(e => e.Value)
                                    .ToList();
            return playerPorts;
        }

        /// <summary>
        /// Gets the string representation of the game board.
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var tileEntry in _resourceTiles)
            {
                sb.AppendFormat("{{{0}: {1}}}\r\n", tileEntry.Key, tileEntry.Value);
            }
            return sb.ToString();
        }

        #region Board setup

        private void SetupResourceTiles()
        {
            _resourceTiles.Clear();

            var tiles = new List<ResourceTypes>();
            tiles.Add(ResourceTypes.Sheep, 4);
            tiles.Add(ResourceTypes.Wood, 4);
            tiles.Add(ResourceTypes.Wheat, 4);
            tiles.Add(ResourceTypes.Ore, 3);
            tiles.Add(ResourceTypes.Brick, 3);
            tiles.Add(ResourceTypes.None, 1);

            int numberTokenIndex = 0;
            int innerIndex = _random.Next(5);
            int outerIndex = innerIndex * 2;

            // Populate outer ring
            for (int i = 0; i < OuterHexagons.Length; i++)
            {
                var tile = tiles.RemoveRandom();
                var resourceTile = (tile == ResourceTypes.None)
                    ? ResourceTile.DesertTile
                    : new ResourceTile(tile, NumberTokens[numberTokenIndex++]);
                _resourceTiles[OuterHexagons[outerIndex]] = resourceTile;

                // Wrap back to start of array if needed
                outerIndex++;
                if (outerIndex == OuterHexagons.Length)
                    outerIndex = 0;
            }

            // Populate inner ring
            for (int i = 0; i < InnerHexagons.Length; i++)
            {
                var tile = tiles.RemoveRandom();
                var resourceTile = (tile == ResourceTypes.None)
                    ? ResourceTile.DesertTile
                    : new ResourceTile(tile, NumberTokens[numberTokenIndex++]);
                _resourceTiles[InnerHexagons[innerIndex]] = resourceTile;

                // Wrap back to start of array if needed
                innerIndex++;
                if (innerIndex == InnerHexagons.Length)
                    innerIndex = 0;
            }

            // Populate center
            var lastTile = tiles[0];
            var lastResourceTile = (lastTile == ResourceTypes.None)
                ? ResourceTile.DesertTile
                : new ResourceTile(lastTile, NumberTokens[numberTokenIndex]);
            _resourceTiles[Hexagon.Zero] = lastResourceTile;
        }

        private void SetupRobber()
        {
            foreach (var tuple in _resourceTiles)
            {
                if (tuple.Value.Resource == ResourceTypes.None)
                {
                    _robberLocation = tuple.Key;
                    return;
                }
            }
            _robberLocation = Hexagon.Zero;
        }

        private void SetupPorts()
        {
            _ports.Clear();

            // There are 4 three-to-one ports, and one two-to-one for each resource type.

            // Sort the water edges into a ring, so we can walk around the outside
            var waterRing = new List<HexEdge>();
            var remainingWaterEdges = new List<HexEdge>(_borderEdges);
            waterRing.Add(remainingWaterEdges.RemoveRandom());
            while (remainingWaterEdges.Count > 0)
            {
                var currentEdge = waterRing.Last();
                var nextEdge = remainingWaterEdges.First(e => e.IsTouching(currentEdge));
                remainingWaterEdges.Remove(nextEdge);
                waterRing.Add(nextEdge);
            }

            var portsToAdd = new List<ResourceTypes>();
            portsToAdd.Add(ResourceTypes.None, 4);
            portsToAdd.Add(ResourceTypes.Brick);
            portsToAdd.Add(ResourceTypes.Ore);
            portsToAdd.Add(ResourceTypes.Sheep);
            portsToAdd.Add(ResourceTypes.Wheat);
            portsToAdd.Add(ResourceTypes.Wood);

            int ringIndex = 0;
            for (int i = 0; portsToAdd.Any(); i++)
            {
                var resource = portsToAdd.RemoveRandom();
                _ports[waterRing[ringIndex]] = new Port(resource);

                // Every third add, skip 4 edges instead of 3.
                ringIndex += (i % 3 == 0) ? 4 : 3;
            }
        }

        #endregion

        #region Helper methods

        private bool IsPlayerBuildingHere(int player, HexEdge edge)
        {
            foreach (var point in edge.GetPoints())
            {
                if (IsPlayerBuildingHere(player, point))
                    return true;
            }
            return false;
        }

        private bool IsPlayerBuildingHere(int player, HexPoint point)
        {
            if (_buildings.ContainsKey(point))
            {
                var b = _buildings[point];
                return (b.Player == player);
            }
            return false;
        }

        private bool IsPlayerRoadHere(int player, HexEdge edge)
        {
            if (_roads.ContainsKey(edge))
            {
                return (_roads[edge].Player == player);
            }
            return false;
        }

        private bool IsHexagonInBoard(Hexagon hex)
        {
            return _validBoardHexagons.Contains(hex);
        }

        private bool IsEdgeInBoard(HexEdge edge)
        {
            return _validBoardEdges.Contains(edge);
        }

        private bool IsPointInBoard(HexPoint point)
        {
            return _validBoardPoints.Contains(point);
        }

        #endregion
    }
}
