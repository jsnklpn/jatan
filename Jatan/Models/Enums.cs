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
        Palace,

        /// <summary>
        /// 
        /// </summary>
        Monopoly,

        /// <summary>
        /// 
        /// </summary>
        RoadBuilding,

        /// <summary>
        /// 
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
    public enum GameStates
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
        GameStarted,

        /// <summary>
        /// The game has ended.
        /// </summary>
        EndOfGame
    }

    /// <summary>
    /// Player turn states.
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
        /// The player must decide what they want to do.
        /// </summary>
        TakeAction,

        /// <summary>
        /// The player is requesting a trade.
        /// </summary>
        RequestingTrade
    }
}
