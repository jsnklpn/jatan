using System;
using System.Linq;
using Jatan.Core;
using Jatan.GameLogic;
using Jatan.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jatan.UnitTest
{
    [TestClass]
    public class GameManagerTest
    {
        const int PLAYER_0 = 0;
        const int PLAYER_1 = 1;
        const int PLAYER_2 = 2;

        [TestMethod]
        public void TestInitialPlacementPhase()
        {
            var manager = new GameManager();
            manager.AddPlayer("Billy"); // PLAYER_0
            manager.AddPlayer("John"); // PLAYER_1
            manager.AddPlayer("Greg"); // PLAYER_2
            
            manager.StartNewGame();

            Assert.AreEqual(GameStates.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
            Assert.AreEqual(PlayerTurnState.PlacingBuilding, manager.PlayerTurnState, "The player state should be in building placement mode.");

            // First, player 0 must place 1 settlement

            Assert.AreEqual(PLAYER_0, manager.ActivePlayer.Id, "It should be player 0's turn.");

            ActionResult ar;
            ar = manager.PlayerPlaceRoad(PLAYER_0, Hexagon.Zero.GetEdge(EdgeDir.Right));
            Assert.IsTrue(ar.Failed, "The player can't build a road yet.");

            ar = manager.PlayerPlaceBuilding(PLAYER_0, BuildingTypes.City, Hexagon.Zero.GetPoint(PointDir.Top));
            Assert.IsTrue(ar.Failed, "The player can't build a city during the initial placement phase.");

            ar = manager.PlayerPlaceBuilding(PLAYER_1, BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.Top));
            Assert.IsTrue(ar.Failed, "It is not this player's turn.");

            ar = manager.PlayerPlaceBuilding(PLAYER_0, BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.Top));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a settlement.");
            Assert.AreEqual(GameStates.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
            Assert.AreEqual(PlayerTurnState.PlacingRoad, manager.PlayerTurnState, "The player state should be in road placement mode.");

            // Now, player 0 is expected to place a road.

            ar = manager.PlayerPlaceBuilding(PLAYER_0, BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.Bottom));
            Assert.IsTrue(ar.Failed, "The player can't build a settlement during the road placement phase.");

            ar = manager.PlayerPlaceRoad(PLAYER_0, Hexagon.Zero.GetEdge(EdgeDir.TopRight));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a road.");

            // Now, player 1 must place a settlement.

            Assert.AreEqual(PLAYER_1, manager.ActivePlayer.Id, "It should be player 1's turn.");

            ar = manager.PlayerPlaceBuilding(PLAYER_1, BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.BottomRight));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a settlement.");
            Assert.AreEqual(GameStates.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
            Assert.AreEqual(PlayerTurnState.PlacingRoad, manager.PlayerTurnState, "The player state should be in road placement mode.");

            // Now, player 1 is expected to place a road.

            ar = manager.PlayerPlaceRoad(PLAYER_1, Hexagon.Zero.GetEdge(EdgeDir.Right));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a road.");

            // Player 2's turn to place a building, a road, another buliding, then another road.

            Assert.AreEqual(PLAYER_2, manager.ActivePlayer.Id, "It should be player 2's turn.");

            ar = manager.PlayerPlaceBuilding(PLAYER_2, BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.BottomLeft));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a settlement.");
            Assert.AreEqual(GameStates.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
            Assert.AreEqual(PlayerTurnState.PlacingRoad, manager.PlayerTurnState, "The player state should be in road placement mode.");

            ar = manager.PlayerPlaceRoad(PLAYER_2, Hexagon.Zero.GetEdge(EdgeDir.Left));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a road.");

            Assert.AreEqual(PLAYER_2, manager.ActivePlayer.Id, "It should still be player 2's turn.");
            Assert.AreEqual(PlayerTurnState.PlacingBuilding, manager.PlayerTurnState, "The player state should be in road placement mode.");

            // Create around a different hexagon since the middle is filled up.
            Hexagon otherHex = new Hexagon(2, 0);

            ar = manager.PlayerPlaceBuilding(PLAYER_2, BuildingTypes.Settlement, otherHex.GetPoint(PointDir.BottomLeft));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a settlement.");
            Assert.AreEqual(GameStates.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
            Assert.AreEqual(PlayerTurnState.PlacingRoad, manager.PlayerTurnState, "The player state should be in road placement mode.");

            ar = manager.PlayerPlaceRoad(PLAYER_2, otherHex.GetEdge(EdgeDir.Left));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a road.");

            // Now we are counting down to player 0. It should be player 1's turn now.

            Assert.AreEqual(PLAYER_1, manager.ActivePlayer.Id, "It should be player 1's turn.");

            ar = manager.PlayerPlaceBuilding(PLAYER_1, BuildingTypes.Settlement, otherHex.GetPoint(PointDir.BottomRight));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a settlement.");
            Assert.AreEqual(GameStates.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
            Assert.AreEqual(PlayerTurnState.PlacingRoad, manager.PlayerTurnState, "The player state should be in road placement mode.");

            ar = manager.PlayerPlaceRoad(PLAYER_1, otherHex.GetEdge(EdgeDir.Right));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a road.");

            // It should be player 0's turn now. After this turn, the game should begin.

            Assert.AreEqual(PLAYER_0, manager.ActivePlayer.Id, "It should be player 0's turn.");

            ar = manager.PlayerPlaceBuilding(PLAYER_0, BuildingTypes.Settlement, otherHex.GetPoint(PointDir.Top));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a settlement.");
            Assert.AreEqual(GameStates.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
            Assert.AreEqual(PlayerTurnState.PlacingRoad, manager.PlayerTurnState, "The player state should be in road placement mode.");

            ar = manager.PlayerPlaceRoad(PLAYER_0, otherHex.GetEdge(EdgeDir.TopRight));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a road.");

            //
            // All done. The game should be started now.
            //
            Assert.AreEqual(PLAYER_0, manager.ActivePlayer.Id, "It should be player 0's turn.");
            Assert.AreEqual(GameStates.GameInProgress, manager.GameState, "The game state should be in the main game phase.");
            Assert.AreEqual(PlayerTurnState.NeedToRoll, manager.PlayerTurnState, "The player state should 'NeedToRoll'.");
        }

        [TestMethod]
        public void TestDiceRoll()
        {
            var manager = DoInitialPlacements();

            // Get a copy of the players' current resources
            var player0 = manager.Players.First(p => p.Id == PLAYER_0);
            var player1 = manager.Players.First(p => p.Id == PLAYER_1);
            var player2 = manager.Players.First(p => p.Id == PLAYER_2);
            var resourcesCopy0 = player0.ResourceCards.Copy();
            var resourcesCopy1 = player1.ResourceCards.Copy();
            var resourcesCopy2 = player2.ResourceCards.Copy();

            // Roll the dice
            var rollResult = manager.PlayerRollDice(PLAYER_0);
            Assert.IsTrue(rollResult.Succeeded, "The roll should not fail.");
            int roll = rollResult.Data;
            var resources = manager.GameBoard.GetResourcesForDiceRoll(roll);

            // Add the resources generated from the roll to the copies.
            var rollResources0 = resources[PLAYER_0];
            var rollResources1 = resources[PLAYER_1];
            var rollResources2 = resources[PLAYER_2];
            resourcesCopy0.Add(rollResources0);
            resourcesCopy1.Add(rollResources1);
            resourcesCopy2.Add(rollResources2);

            Assert.IsTrue(player0.ResourceCards.Equals(resourcesCopy0), "The player's resource counts are not correct");
            Assert.IsTrue(player1.ResourceCards.Equals(resourcesCopy1), "The player's resource counts are not correct");
            Assert.IsTrue(player2.ResourceCards.Equals(resourcesCopy2), "The player's resource counts are not correct");
        }

        [TestMethod]
        public void TestAcceptTradeOffer()
        {
            // TODO
            Assert.Fail();
        }

        [TestMethod]
        public void TestAcceptTradeCounterOffer()
        {
            // TODO
            Assert.Fail();
        }

        [TestMethod]
        public void TestTradeWithBank()
        {
            var manager = DoInitialPlacementsAndRoll();
            var player = manager.ActivePlayer;

            // Trade 4 ore for 1 brick.
            var fourOre = new ResourceCollection(ore: 4);
            var oneBrick = new ResourceCollection(brick: 1);
            player.RemoveAllResources();
            player.AddResources(fourOre);
            Assert.IsTrue(player.ResourceCards.IsSingleResourceType && player.GetNumberResourceCount(ResourceTypes.Ore) == 4);
            var offer = new TradeOffer(player.Id, fourOre, oneBrick);
            var tradeResult = manager.PlayerTradeWithBank(player.Id, offer);
            Assert.IsTrue(tradeResult.Succeeded, "The bank trade should succeed.");
            Assert.IsTrue(player.ResourceCards.IsSingleResourceType && player.GetNumberResourceCount(ResourceTypes.Brick) == 1);

            // Trade 8 wheat for 2 wood.
            var eightWheat = new ResourceCollection(wheat: 8);
            var twoWood = new ResourceCollection(wood: 2);
            player.RemoveAllResources();
            player.AddResources(eightWheat);
            Assert.IsTrue(player.ResourceCards.IsSingleResourceType && player.GetNumberResourceCount(ResourceTypes.Wheat) == 8);
            offer = new TradeOffer(player.Id, eightWheat, twoWood);
            tradeResult = manager.PlayerTradeWithBank(player.Id, offer);
            Assert.IsTrue(tradeResult.Succeeded, "The bank trade should succeed.");
            Assert.IsTrue(player.ResourceCards.IsSingleResourceType && player.GetNumberResourceCount(ResourceTypes.Wood) == 2);

            // Trade 2 wheat for 2 wood. Should fail.
            var twoWheat = new ResourceCollection(wheat: 2);
            player.RemoveAllResources();
            player.AddResources(twoWheat);
            Assert.IsTrue(player.ResourceCards.IsSingleResourceType && player.GetNumberResourceCount(ResourceTypes.Wheat) == 2);
            offer = new TradeOffer(player.Id, twoWheat, twoWood);
            tradeResult = manager.PlayerTradeWithBank(player.Id, offer);
            Assert.IsTrue(tradeResult.Failed, "The bank trade should fail.");
            Assert.IsTrue(player.ResourceCards.IsSingleResourceType && player.GetNumberResourceCount(ResourceTypes.Wheat) == 2);
        }

        private GameManager DoInitialPlacementsAndRoll()
        {
            var manager = DoInitialPlacements();
            var player = manager.ActivePlayer;
            manager.PlayerRollDice(player.Id);
            Assert.AreEqual(PlayerTurnState.TakeAction, manager.PlayerTurnState, "The player state should 'TakeAction'.");
            return manager;
        }

        private GameManager DoInitialPlacements()
        {
            // This setup method will create a 3-player game with the center and far-right hexagons fully surrounded.

            var manager = new GameManager();
            manager.AddPlayer("Billy"); // PLAYER_0
            manager.AddPlayer("John"); // PLAYER_1
            manager.AddPlayer("Greg"); // PLAYER_2
            manager.StartNewGame();

            // player 0
            manager.PlayerPlaceBuilding(PLAYER_0, BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.Top));
            manager.PlayerPlaceRoad(PLAYER_0, Hexagon.Zero.GetEdge(EdgeDir.TopRight));

            // player 1
            manager.PlayerPlaceBuilding(PLAYER_1, BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.BottomRight));
            manager.PlayerPlaceRoad(PLAYER_1, Hexagon.Zero.GetEdge(EdgeDir.Right));

            // player 2
            manager.PlayerPlaceBuilding(PLAYER_2, BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.BottomLeft));
            manager.PlayerPlaceRoad(PLAYER_2, Hexagon.Zero.GetEdge(EdgeDir.Left));

            // Create around a different hexagon since the middle is filled up.
            Hexagon otherHex = new Hexagon(2, 0);

            manager.PlayerPlaceBuilding(PLAYER_2, BuildingTypes.Settlement, otherHex.GetPoint(PointDir.BottomLeft));
            manager.PlayerPlaceRoad(PLAYER_2, otherHex.GetEdge(EdgeDir.Left));

            // player 1
            manager.PlayerPlaceBuilding(PLAYER_1, BuildingTypes.Settlement, otherHex.GetPoint(PointDir.BottomRight));
            manager.PlayerPlaceRoad(PLAYER_1, otherHex.GetEdge(EdgeDir.Right));

            // player 0
            manager.PlayerPlaceBuilding(PLAYER_0, BuildingTypes.Settlement, otherHex.GetPoint(PointDir.Top));
            manager.PlayerPlaceRoad(PLAYER_0, otherHex.GetEdge(EdgeDir.TopRight));

            Assert.AreEqual(PLAYER_0, manager.ActivePlayer.Id, "It should be player 0's turn.");
            Assert.AreEqual(GameStates.GameInProgress, manager.GameState, "The game state should be in the main game phase.");
            Assert.AreEqual(PlayerTurnState.NeedToRoll, manager.PlayerTurnState, "The player state should 'NeedToRoll'.");

            return manager;
        }
    }
}
