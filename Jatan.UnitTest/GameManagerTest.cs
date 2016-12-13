using System;
using Jatan.Core;
using Jatan.GameLogic;
using Jatan.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jatan.UnitTest
{
    [TestClass]
    public class GameManagerTest
    {
        [TestMethod]
        public void TestInitialPlacementPhase()
        {
            var manager = new GameManager();
            manager.AddPlayer("Billy"); // player 0
            manager.AddPlayer("John"); // player 1
            manager.AddPlayer("Greg"); // player 2
            manager.StartNewGame();

            Assert.AreEqual(GameStates.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
            Assert.AreEqual(PlayerTurnState.PlacingBuilding, manager.PlayerTurnState, "The player state should be in building placement mode.");

            // First, player 0 must place 1 settlement

            ActionResult ar;
            ar = manager.PlayerPlaceRoad(0, Hexagon.Zero.GetEdge(EdgeDir.Right));
            Assert.IsTrue(ar.Failed, "The player can't build a road yet.");

            ar = manager.PlayerPlaceBuilding(0, BuildingTypes.City, Hexagon.Zero.GetPoint(PointDir.Top));
            Assert.IsTrue(ar.Failed, "The player can't build a city during the initial placement phase.");

            ar = manager.PlayerPlaceBuilding(1, BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.Top));
            Assert.IsTrue(ar.Failed, "It is not this player's turn.");

            ar = manager.PlayerPlaceBuilding(0, BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.Top));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a settlement.");
            Assert.AreEqual(GameStates.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
            Assert.AreEqual(PlayerTurnState.PlacingRoad, manager.PlayerTurnState, "The player state should be in road placement mode.");

            // Now, player 0 is expected to place two roads.

            ar = manager.PlayerPlaceBuilding(0, BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.Bottom));
            Assert.IsTrue(ar.Failed, "The player can't build a settlement during the road placement phase.");

            ar = manager.PlayerPlaceRoad(0, Hexagon.Zero.GetEdge(EdgeDir.TopRight));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a road.");
            Assert.AreEqual(PlayerTurnState.PlacingRoad, manager.PlayerTurnState, "The player state should still be in road placement mode.");

            ar = manager.PlayerPlaceRoad(0, Hexagon.Zero.GetEdge(EdgeDir.TopLeft));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a second road.");

            // Now, player 1 must place a settlement.
            // TODO: finish test
        }
    }
}
