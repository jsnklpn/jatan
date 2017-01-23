using System;
using System.Linq;
using System.Reflection;
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

            Assert.AreEqual(GameState.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
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
            Assert.AreEqual(GameState.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
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
            Assert.AreEqual(GameState.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
            Assert.AreEqual(PlayerTurnState.PlacingRoad, manager.PlayerTurnState, "The player state should be in road placement mode.");

            // Now, player 1 is expected to place a road.

            ar = manager.PlayerPlaceRoad(PLAYER_1, Hexagon.Zero.GetEdge(EdgeDir.Right));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a road.");

            // Player 2's turn to place a building, a road, another buliding, then another road.

            Assert.AreEqual(PLAYER_2, manager.ActivePlayer.Id, "It should be player 2's turn.");

            ar = manager.PlayerPlaceBuilding(PLAYER_2, BuildingTypes.Settlement, Hexagon.Zero.GetPoint(PointDir.BottomLeft));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a settlement.");
            Assert.AreEqual(GameState.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
            Assert.AreEqual(PlayerTurnState.PlacingRoad, manager.PlayerTurnState, "The player state should be in road placement mode.");

            ar = manager.PlayerPlaceRoad(PLAYER_2, Hexagon.Zero.GetEdge(EdgeDir.Left));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a road.");

            Assert.AreEqual(PLAYER_2, manager.ActivePlayer.Id, "It should still be player 2's turn.");
            Assert.AreEqual(PlayerTurnState.PlacingBuilding, manager.PlayerTurnState, "The player state should be in road placement mode.");

            // Create around a different hexagon since the middle is filled up.
            Hexagon otherHex = new Hexagon(2, 0);

            ar = manager.PlayerPlaceBuilding(PLAYER_2, BuildingTypes.Settlement, otherHex.GetPoint(PointDir.BottomLeft));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a settlement.");
            Assert.AreEqual(GameState.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
            Assert.AreEqual(PlayerTurnState.PlacingRoad, manager.PlayerTurnState, "The player state should be in road placement mode.");

            ar = manager.PlayerPlaceRoad(PLAYER_2, otherHex.GetEdge(EdgeDir.Left));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a road.");

            // Now we are counting down to player 0. It should be player 1's turn now.

            Assert.AreEqual(PLAYER_1, manager.ActivePlayer.Id, "It should be player 1's turn.");

            ar = manager.PlayerPlaceBuilding(PLAYER_1, BuildingTypes.Settlement, otherHex.GetPoint(PointDir.BottomRight));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a settlement.");
            Assert.AreEqual(GameState.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
            Assert.AreEqual(PlayerTurnState.PlacingRoad, manager.PlayerTurnState, "The player state should be in road placement mode.");

            ar = manager.PlayerPlaceRoad(PLAYER_1, otherHex.GetEdge(EdgeDir.Right));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a road.");

            // It should be player 0's turn now. After this turn, the game should begin.

            Assert.AreEqual(PLAYER_0, manager.ActivePlayer.Id, "It should be player 0's turn.");

            ar = manager.PlayerPlaceBuilding(PLAYER_0, BuildingTypes.Settlement, otherHex.GetPoint(PointDir.Top));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a settlement.");
            Assert.AreEqual(GameState.InitialPlacement, manager.GameState, "The game state should be in the initial placement phase.");
            Assert.AreEqual(PlayerTurnState.PlacingRoad, manager.PlayerTurnState, "The player state should be in road placement mode.");

            ar = manager.PlayerPlaceRoad(PLAYER_0, otherHex.GetEdge(EdgeDir.TopRight));
            Assert.IsTrue(ar.Succeeded, "The player should be able to place a road.");

            //
            // All done. The game should be started now.
            //
            Assert.AreEqual(PLAYER_0, manager.ActivePlayer.Id, "It should be player 0's turn.");
            Assert.AreEqual(GameState.GameInProgress, manager.GameState, "The game state should be in the main game phase.");
            Assert.AreEqual(PlayerTurnState.NeedToRoll, manager.PlayerTurnState, "The player state should 'NeedToRoll'.");
        }

        [TestMethod]
        public void TestDiceRoll()
        {
            var manager = DoInitialPlacements(false);

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
            int roll = rollResult.Data.Total;
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
            var manager = DoInitialPlacementsAndRoll(false);
            var activePlayer = manager.ActivePlayer;
            var player1 = manager.Players.FirstOrDefault(p => p.Id == PLAYER_1);
            var player2 = manager.Players.FirstOrDefault(p => p.Id == PLAYER_2);
            Assert.IsNotNull(player1);
            Assert.IsNotNull(player2);

            // Active player offer trade of 1 ore and 2 wheat for 3 wood and 4 sheep.
            activePlayer.RemoveAllResources();
            player1.RemoveAllResources();
            player2.RemoveAllResources();
            var toGive = new ResourceCollection(ore: 1, wheat: 2);
            var toGet = new ResourceCollection(wood: 3, sheep: 4);
            activePlayer.AddResources(toGive);
            player1.AddResources(toGet);
            manager.PlayerOfferTrade(activePlayer.Id, new TradeOffer(activePlayer.Id, toGive, toGet));
            Assert.AreEqual(PlayerTurnState.RequestingPlayerTrade, manager.PlayerTurnState, "Player should be in the 'RequestingTrade' state.");

            // Player 2 will try to accept the trade, yet he can't afford to do it.
            var r = manager.AcceptTradeFromActivePlayer(player2.Id);
            Assert.IsTrue(r.Failed, "This player can't afford to do the trade.");

            // Player 1 will accept the trade and it should work.
            r = manager.AcceptTradeFromActivePlayer(player1.Id);
            Assert.IsTrue(r.Succeeded, "The trade should be successful.");
            Assert.IsTrue(activePlayer.ResourceCards.Equals(toGet), "Active player did not get his resources from the trade.");
            Assert.IsTrue(player1.ResourceCards.Equals(toGive), "Player 1 did not get his resources from the trade.");
            Assert.AreEqual(PlayerTurnState.TakeAction, manager.PlayerTurnState, "The trade is complete. Player should be in the 'TakeAction' state.");
        }

        [TestMethod]
        public void TestAcceptTradeCounterOffer()
        {
            var manager = DoInitialPlacementsAndRoll(false);
            var activePlayer = manager.ActivePlayer;
            var player1 = manager.Players.FirstOrDefault(p => p.Id == PLAYER_1);
            Assert.IsNotNull(player1);

            // Active player offer trade of 1 ore and 2 wheat for 3 wood and 4 sheep.
            activePlayer.RemoveAllResources();
            player1.RemoveAllResources();
            var toOtherPlayer = new ResourceCollection(ore: 1, wheat: 2);
            var toActivePlayer = new ResourceCollection(wood: 3, sheep: 4);
            var toActivePlayerCounter = new ResourceCollection(brick: 5, sheep: 1);
            var originalOffer = new TradeOffer(activePlayer.Id, toOtherPlayer, toActivePlayer);
            var counterOffer = new TradeOffer(player1.Id, toActivePlayerCounter, toOtherPlayer);

            activePlayer.AddResources(toOtherPlayer);
            player1.AddResources(toActivePlayerCounter);
            manager.PlayerOfferTrade(activePlayer.Id, originalOffer);
            Assert.AreEqual(PlayerTurnState.RequestingPlayerTrade, manager.PlayerTurnState, "Player should be in the 'RequestingTrade' state.");

            // Player 1 can't afford it and will send a counter-offer.
            var r = manager.AcceptTradeFromActivePlayer(player1.Id);
            Assert.IsTrue(r.Failed, "Player 1 can't afford the current trade.");
            r = manager.SendCounterTradeOffer(player1.Id, counterOffer);
            Assert.IsTrue(r.Succeeded, "The counter offer send action should work.");
            r = manager.PlayerAcceptCounterTradeOffer(activePlayer.Id, player1.Id);
            Assert.IsTrue(r.Succeeded, "The counter offer should be accepted.");

            Assert.IsTrue(activePlayer.ResourceCards.Equals(toActivePlayerCounter), "Active player did not get his resources from the trade.");
            Assert.IsTrue(player1.ResourceCards.Equals(toOtherPlayer), "Player 1 did not get his resources from the trade.");
            Assert.AreEqual(PlayerTurnState.TakeAction, manager.PlayerTurnState, "The trade is complete. Player should be in the 'TakeAction' state.");
        }

        [TestMethod]
        public void TestTradeWithBank()
        {
            var manager = DoInitialPlacementsAndRoll(false);
            var player = manager.ActivePlayer;

            // Trade 4 ore for 1 brick.
            var toGive = new ResourceCollection(ore: 4);
            var toGet = new ResourceCollection(brick: 1);
            player.RemoveAllResources();
            player.AddResources(toGive);
            Assert.IsTrue(player.ResourceCards.IsSingleResourceType && player.ResourceCards.Equals(toGive));
            var offer = new TradeOffer(player.Id, toGive, toGet);
            var tradeResult = manager.PlayerTradeWithBank(player.Id, offer);
            Assert.IsTrue(tradeResult.Succeeded, "The bank trade should succeed.");
            Assert.IsTrue(player.ResourceCards.IsSingleResourceType && player.ResourceCards.Equals(toGet));
            Assert.AreEqual(PlayerTurnState.TakeAction, manager.PlayerTurnState, "Player should be in the 'TakeAction' state.");

            // Trade 8 wheat for 2 wood.
            toGive = new ResourceCollection(wheat: 8);
            toGet = new ResourceCollection(wood: 2);
            player.RemoveAllResources();
            player.AddResources(toGive);
            Assert.IsTrue(player.ResourceCards.IsSingleResourceType && player.ResourceCards.Equals(toGive));
            offer = new TradeOffer(player.Id, toGive, toGet);
            tradeResult = manager.PlayerTradeWithBank(player.Id, offer);
            Assert.IsTrue(tradeResult.Succeeded, "The bank trade should succeed.");
            Assert.IsTrue(player.ResourceCards.IsSingleResourceType && player.ResourceCards.Equals(toGet));
            Assert.AreEqual(PlayerTurnState.TakeAction, manager.PlayerTurnState, "Player should be in the 'TakeAction' state.");

            // Trade 2 wheat for 2 wood. Should fail.
            toGive = new ResourceCollection(wheat: 2);
            toGet = new ResourceCollection(wood: 2);
            player.RemoveAllResources();
            player.AddResources(toGive);
            Assert.IsTrue(player.ResourceCards.IsSingleResourceType && player.ResourceCards.Equals(toGive));
            offer = new TradeOffer(player.Id, toGive, toGet);
            tradeResult = manager.PlayerTradeWithBank(player.Id, offer);
            Assert.IsTrue(tradeResult.Failed, "The bank trade should fail.");
            Assert.IsTrue(player.ResourceCards.IsSingleResourceType && player.ResourceCards.Equals(toGive));
            Assert.AreEqual(PlayerTurnState.TakeAction, manager.PlayerTurnState, "Player should be in the 'TakeAction' state.");
        }

        [TestMethod]
        public void TestPlayMonopolyCard()
        {
            var manager = DoInitialPlacementsAndRoll(false);
            var activePlayer = manager.ActivePlayer;
            var player1 = manager.Players.FirstOrDefault(p => p.Id == PLAYER_1);
            var player2 = manager.Players.FirstOrDefault(p => p.Id == PLAYER_2);
            Assert.IsNotNull(activePlayer);
            Assert.IsNotNull(player1);
            Assert.IsNotNull(player2);

            activePlayer.RemoveAllResources();
            player1.RemoveAllResources();
            player2.RemoveAllResources();

            activePlayer.AddResources(new ResourceCollection(wood: 1));
            player1.AddResources(new ResourceCollection(wood: 5, ore: 2, sheep: 1));
            player2.AddResources(new ResourceCollection(wheat: 6, sheep: 3, brick: 1));

            var pr = manager.PlayerPlayDevelopmentCard(activePlayer.Id, DevelopmentCards.Monopoly);
            Assert.IsTrue(pr.Failed, "Card play should fail. The player does not have the card yet.");

            activePlayer.DevelopmentCards.Add(DevelopmentCards.Monopoly);

            pr = manager.PlayerPlayDevelopmentCard(activePlayer.Id, DevelopmentCards.Monopoly);
            Assert.IsTrue(pr.Succeeded, "Card play should succeed.");
            Assert.AreEqual(PlayerTurnState.MonopolySelectingResource, manager.PlayerTurnState, "Player should be in the 'MonopolySelectingResource' state.");

            // Now the player must choose a card to monopolize

            pr = manager.PlayerSelectResourceForMonopoly(activePlayer.Id, ResourceTypes.Sheep);
            Assert.IsTrue(pr.Succeeded, "Resource selection should succeed.");
            Assert.AreEqual(PlayerTurnState.TakeAction, manager.PlayerTurnState, "Player should be in the 'TakeAction' state.");

            Assert.AreEqual(4, activePlayer.ResourceCards[ResourceTypes.Sheep]);
            Assert.AreEqual(0, player1.ResourceCards[ResourceTypes.Sheep]);
            Assert.AreEqual(0, player2.ResourceCards[ResourceTypes.Sheep]);
        }

        [TestMethod]
        public void TestPlayRoadBuildingCard()
        {
            var manager = DoInitialPlacementsAndRoll(false);
            var activePlayer = manager.ActivePlayer;
            Assert.IsNotNull(activePlayer);

            var pr = manager.PlayerPlayDevelopmentCard(activePlayer.Id, DevelopmentCards.RoadBuilding);
            Assert.IsTrue(pr.Failed, "Card play should fail. The player does not have the card yet.");

            activePlayer.DevelopmentCards.Add(DevelopmentCards.RoadBuilding);

            pr = manager.PlayerPlayDevelopmentCard(activePlayer.Id, DevelopmentCards.RoadBuilding);
            Assert.IsTrue(pr.Succeeded, "Card play should succeed.");
            Assert.AreEqual(PlayerTurnState.RoadBuildingSelectingRoads, manager.PlayerTurnState, "Player should be in the 'RoadBuildingSelectingRoads' state.");

            // Now the player gets to place 2 roads.

            var roadCount = manager.GameBoard.GetRoadCountForPlayer(activePlayer.Id);
            Assert.AreEqual(2, roadCount);

            pr = manager.PlayerPlaceRoadForRoadBuilding(activePlayer.Id, new Hexagon(0, 1).GetEdge(EdgeDir.Right));
            roadCount = manager.GameBoard.GetRoadCountForPlayer(activePlayer.Id);
            Assert.IsTrue(pr.Succeeded, "Road building should succeed.");
            Assert.AreEqual(PlayerTurnState.RoadBuildingSelectingRoads, manager.PlayerTurnState, "Player should still be in the 'RoadBuildingSelectingRoads' state.");
            Assert.AreEqual(3, roadCount);

            pr = manager.PlayerPlaceRoadForRoadBuilding(activePlayer.Id, new Hexagon(0, 1).GetEdge(EdgeDir.TopRight));
            roadCount = manager.GameBoard.GetRoadCountForPlayer(activePlayer.Id);
            Assert.IsTrue(pr.Succeeded, "Road building should succeed.");
            Assert.AreEqual(PlayerTurnState.TakeAction, manager.PlayerTurnState, "Player should still be in the 'TakeAction' state.");
            Assert.AreEqual(4, roadCount);
        }

        [TestMethod]
        public void TestPlayYearOfPlentyCard()
        {
            var manager = DoInitialPlacementsAndRoll(false);
            var activePlayer = manager.ActivePlayer;
            Assert.IsNotNull(activePlayer);

            activePlayer.RemoveAllResources();

            var pr = manager.PlayerPlayDevelopmentCard(activePlayer.Id, DevelopmentCards.YearOfPlenty);
            Assert.IsTrue(pr.Failed, "Card play should fail. The player does not have the card yet.");

            activePlayer.DevelopmentCards.Add(DevelopmentCards.YearOfPlenty);

            pr = manager.PlayerPlayDevelopmentCard(activePlayer.Id, DevelopmentCards.YearOfPlenty);
            Assert.IsTrue(pr.Succeeded, "Card play should succeed.");
            Assert.AreEqual(PlayerTurnState.YearOfPlentySelectingResources, manager.PlayerTurnState, "Player should be in the 'YearOfPlentySelectingResources' state.");

            // Now the player must choose 2 resources to collect

            Assert.AreEqual(0, activePlayer.ResourceCards[ResourceTypes.Wood]);
            Assert.AreEqual(0, activePlayer.ResourceCards[ResourceTypes.Brick]);

            pr = manager.PlayerSelectResourcesForYearOfPlenty(activePlayer.Id, ResourceTypes.Wood, ResourceTypes.Brick);
            Assert.IsTrue(pr.Succeeded, "Resource selection should succeed.");
            Assert.AreEqual(PlayerTurnState.TakeAction, manager.PlayerTurnState, "Player should be in the 'TakeAction' state.");

            Assert.AreEqual(1, activePlayer.ResourceCards[ResourceTypes.Wood]);
            Assert.AreEqual(1, activePlayer.ResourceCards[ResourceTypes.Brick]);
        }

        private GameManager DoInitialPlacementsAndRoll(bool allowSevenRoll)
        {
            var manager = DoInitialPlacements(allowSevenRoll);
            var player = manager.ActivePlayer;
            manager.PlayerRollDice(player.Id);
            Assert.AreEqual(PlayerTurnState.TakeAction, manager.PlayerTurnState, "The player state should 'TakeAction'.");
            return manager;
        }

        private GameManager DoInitialPlacements(bool allowSevenRoll)
        {
            // This setup method will create a 3-player game with the center and far-right hexagons fully surrounded.

            var manager = new GameManager();
            manager.AddPlayer("Billy"); // PLAYER_0
            manager.AddPlayer("John"); // PLAYER_1
            manager.AddPlayer("Greg"); // PLAYER_2

            if (!allowSevenRoll)
            {
                var diceFieldInfo = typeof(GameManager).GetField("_dice", BindingFlags.NonPublic | BindingFlags.Instance);
                var dice = (diceFieldInfo != null) ? diceFieldInfo.GetValue(manager) as Dice : null;
                if (dice != null)
                {
                    dice.ExcludeSet.Add(7);
                }
            }

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
            Assert.AreEqual(GameState.GameInProgress, manager.GameState, "The game state should be in the main game phase.");
            Assert.AreEqual(PlayerTurnState.NeedToRoll, manager.PlayerTurnState, "The player state should 'NeedToRoll'.");

            return manager;
        }
    }
}
