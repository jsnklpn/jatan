using System;
using System.Collections.Generic;
using System.Linq;
using Jatan.Core;
using Jatan.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jatan.UnitTest
{
    [TestClass]
    public class GameBoardTest
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        public void TestDiceRoll()
        {
            var board = new GameBoard();
            board.Setup();

            // Randomize the board again if the middle tile (the tile to test) is a desert tile.
            while (board.ResourceTiles[Hexagon.Zero].Resource == ResourceTypes.None)
            {
                board.Setup();
            }

            var middleResourceTile = board.ResourceTiles[Hexagon.Zero];
            board.PlaceBuilding(1, BuildingTypes.Settlement, new HexPoint(0, 0, 1, 1, 1, 0), true);
            board.PlaceBuilding(2, BuildingTypes.City, new HexPoint(0, 0, -1, 0, -1, -1), true);

            var rollResult = board.GetResourcesForDiceRoll(middleResourceTile.RetrieveNumber);

            Assert.AreEqual(1, rollResult[1][middleResourceTile.Resource], "Settlement needs to collect 1 resource.");
            Assert.AreEqual(2, rollResult[2][middleResourceTile.Resource], "City needs to collect 2 resources.");
        }

        [TestMethod]
        public void TestRobber()
        {
            var board = new GameBoard();
            board.Setup();

            var middleResourceTile = board.ResourceTiles[Hexagon.Zero];
            board.PlaceBuilding(1, BuildingTypes.Settlement, new HexPoint(0, 0, 1, 1, 1, 0), true);
            board.PlaceBuilding(2, BuildingTypes.City, new HexPoint(0, 0, -1, 0, -1, -1), true);

            // Put the robber in the middle.
            board.MoveRobber(1, Hexagon.Zero);

            var rollResult = board.GetResourcesForDiceRoll(middleResourceTile.RetrieveNumber);

            Assert.IsTrue(rollResult[1].IsEmpty(), "Settlement should not collect on a robber tile.");
            Assert.IsTrue(rollResult[2].IsEmpty(), "City should not collect on a robber tile.");
        }

        [TestMethod]
        public void TestLongestRoad()
        {
            var board = new GameBoard();
            board.Setup();
            board.PlaceBuilding(1, BuildingTypes.Settlement, new HexPoint(0, 0, 1, 1, 1, 0), true);

            Assert.AreEqual(0, board.GetRoadLengthForPlayer(1));
            board.PlaceRoad(1, new HexEdge(0, 0, 1, 1), false);
            Assert.AreEqual(1, board.GetRoadLengthForPlayer(1));
            board.PlaceRoad(1, new HexEdge(0, 0, 1, 0), false);
            Assert.AreEqual(2, board.GetRoadLengthForPlayer(1));
            board.PlaceRoad(1, new HexEdge(0, 0, 0, -1), false);
            Assert.AreEqual(3, board.GetRoadLengthForPlayer(1));
            board.PlaceRoad(1, new HexEdge(0, 0, -1, -1), false);
            Assert.AreEqual(4, board.GetRoadLengthForPlayer(1));
            board.PlaceRoad(1, new HexEdge(0, 0, -1, 0), false);
            Assert.AreEqual(5, board.GetRoadLengthForPlayer(1));
            board.PlaceRoad(1, new HexEdge(0, 0, 0, 1), false);
            Assert.AreEqual(6, board.GetRoadLengthForPlayer(1));
            board.PlaceRoad(1, new HexEdge(0, 1, 1, 1), false);
            Assert.AreEqual(7, board.GetRoadLengthForPlayer(1));
            board.PlaceRoad(1, new HexEdge(1, 2, 1, 1), false);
            Assert.AreEqual(8, board.GetRoadLengthForPlayer(1));
            board.PlaceRoad(1, new HexEdge(2, 2, 1, 1), false);
            Assert.AreEqual(9, board.GetRoadLengthForPlayer(1));
            board.PlaceRoad(1, new HexEdge(2, 1, 1, 1), false);
            Assert.AreEqual(10, board.GetRoadLengthForPlayer(1));
            board.PlaceRoad(1, new HexEdge(1, 0, 1, 1), false);
            Assert.AreEqual(11, board.GetRoadLengthForPlayer(1));

            // Block player1's road with a player2 settlement
            board.PlaceBuilding(2, BuildingTypes.Settlement, new HexPoint(0, 0, -1, -1, 0, -1), true);
            board.PlaceBuilding(2, BuildingTypes.Settlement, new HexPoint(0, 0, -1, 0, 0, 1), true);
            Assert.AreEqual(8, board.GetRoadLengthForPlayer(1));

            // Create roads for player2
            board.PlaceBuilding(2, BuildingTypes.Settlement, new HexPoint(-2, 0, -1, 0, -2, -1), true);
            board.PlaceBuilding(2, BuildingTypes.Settlement, new HexPoint(1, 0, 2, 0, 1, -1), true);

            Assert.AreEqual(0, board.GetRoadLengthForPlayer(2));
            
            board.PlaceRoad(2, new HexEdge(-2, 0, -2, -1), false);
            board.PlaceRoad(2, new HexEdge(-1, 0, -2, -1), false);

            Assert.AreEqual(2, board.GetRoadLengthForPlayer(2));

            board.PlaceRoad(2, new HexEdge(1, 0, 1, -1), false);
            board.PlaceRoad(2, new HexEdge(2, 0, 1, -1), false);
            board.PlaceRoad(2, new HexEdge(2, -1, 1, -1), false);

            Assert.AreEqual(3, board.GetRoadLengthForPlayer(2));
        }
    }
}
