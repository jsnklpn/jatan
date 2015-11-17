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
    }
}
