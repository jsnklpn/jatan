using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Models
{
    /// <summary>
    /// Types of resources
    /// </summary>
    public enum ResourceTypes
    {
        None = 0,
        Brick,
        Wood,
        Wheat,
        Sheep,
        Ore,
    }

    /// <summary>
    /// Types of player buildings
    /// </summary>
    public enum BuildingTypes
    {
        /// <summary>
        /// A settlement which gives 1 resource for each tile it touches. Each player gets 5.
        /// </summary>
        Settlement,

        /// <summary>
        /// A city which gives 2 resources for each tile it touches. Each player gets 4.
        /// </summary>
        City
    }

    /// <summary>
    /// Types of development cards
    /// </summary>
    public enum DevelopmentCards
    {
        /// <summary>
        /// Allows a player to move the robber. 14 in deck.
        /// </summary>
        Knight,

        /// <summary>
        /// 1 victory point.
        /// </summary>
        Library,

        /// <summary>
        /// 1 victory point.
        /// </summary>
        Market,

        /// <summary>
        /// 1 vistory point.
        /// </summary>
        GreatHall,

        /// <summary>
        /// 1 victory point.
        /// </summary>
        Chapel,

        /// <summary>
        /// 1 victory point.
        /// </summary>
        University,

        /// <summary>
        /// Progress card: If you play this card, you must name 1 type of resource. All the other players
        /// must give you all of the resource cards of this type that they have in their hands.
        /// </summary>
        Monopoly,

        /// <summary>
        /// Progress card: If you play this card, you may immediately place 2 free roads on the board.
        /// </summary>
        RoadBuilding,

        /// <summary>
        /// Progress card: If you play this card you may immediately take any 2 resource cards from the supply stacks.
        /// </summary>
        YearOfPlenty
    }

    /// <summary>
    /// Items that a player can purchase.
    /// </summary>
    public enum PurchasableItems
    {
        /// <summary>
        /// Costs 1 wood, 1 brick.
        /// </summary>
        Road,

        /// <summary>
        /// Costs 1 wood, 1 brick, 1 wheat, 1 sheep.
        /// </summary>
        Settlement,

        /// <summary>
        /// Costs 2 wheat, 3 ore.
        /// </summary>
        City,

        /// <summary>
        /// Costs 1 wheat, 1 sheep, 1 ore.
        /// </summary>
        DevelopmentCard
    }

    /// <summary>
    /// Game states.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// The game has not started at all.
        /// </summary>
        NotStarted,

        /// <summary>
        /// Players are placing settlements and roads on the board.
        /// </summary>
        InitialPlacement,

        /// <summary>
        /// The regular game is in play.
        /// </summary>
        GameInProgress,

        /// <summary>
        /// The game has ended.
        /// </summary>
        EndOfGame
    }

    /// <summary>
    /// Player turn states. The states are needed for actions that require further action from the player.
    /// </summary>
    public enum PlayerTurnState
    {
        /// <summary>
        /// It's not this players turn.
        /// </summary>
        None = 0,

        /// <summary>
        /// The player must roll.
        /// </summary>
        NeedToRoll,

        /// <summary>
        /// Any players are selecting which cards to lose from rolling a 7.
        /// </summary>
        AnyPlayerSelectingCardsToLose,

        /// <summary>
        /// The player is placing the robber.
        /// </summary>
        PlacingRobber,

        /// <summary>
        /// The player is selecting a player to steal from.
        /// </summary>
        SelectingPlayerToStealFrom,

        /// <summary>
        /// The player is done rolling and now must decide what they want to do.
        /// </summary>
        TakeAction,

        /// <summary>
        /// The player is placing a road.
        /// </summary>
        PlacingRoad,

        /// <summary>
        /// The player is placing a building.
        /// </summary>
        PlacingBuilding,
        
        /// <summary>
        /// The player is requesting a trade from another player.
        /// </summary>
        RequestingPlayerTrade,

        /// <summary>
        /// Progress card: Player must name 1 type of resource. Other players must give the player their cards of that type.
        /// </summary>
        MonopolySelectingResource,

        /// <summary>
        /// Progress card: Player may immediately place 2 free roads on the board.
        /// </summary>
        RoadBuildingSelectingRoads,

        /// <summary>
        /// Progress card: If you play this card you may immediately take any 2 resource cards from the supply stacks.
        /// </summary>
        YearOfPlentySelectingResources
    }

    /// <summary>
    /// Usage modes for the robber.
    /// </summary>
    public enum RobberMode
    {
        /// <summary>
        /// In normal mode, the robber will prevent a tile from generating resources.
        /// </summary>
        Normal,

        /// <summary>
        /// In boost mode, the robber will cause a tile to generate extra resources.
        /// </summary>
        ResourceBoost,

        /// <summary>
        /// The robber will do nothing.
        /// </summary>
        None
    }

    /// <summary>
    /// Possible colors for a player.
    /// </summary>
    public enum PlayerColor
    {
        Blue,
        Red,
        Green,
        Yellow,
        Pink
    }
}
