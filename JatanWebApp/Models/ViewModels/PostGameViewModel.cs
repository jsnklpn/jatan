using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.GameLogic;
using Jatan.Models;
using JatanWebApp.SignalR;

namespace JatanWebApp.Models.ViewModels
{
    /// <summary>
    /// ViewModel for the PostGame view.
    /// </summary>
    public class PostGameViewModel
    {
        /// <summary>
        /// Error message if something went wrong.
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// The name of the game
        /// </summary>
        public string GameName { get; set; }
        /// <summary>
        /// The total number of turns in the game.
        /// </summary>
        public int TotalTurnCount { get; set; }
        /// <summary>
        /// A map of players to a map of resources to the list of amount collected.
        /// </summary>
        public Dictionary<Player, Dictionary<ResourceTypes, int[]>> CardsCollected;
        /// <summary>
        /// A map of numbers to the number of times that number was rolled. (From 2 to 12)
        /// </summary>
        public Dictionary<int, int> DiceRolls { get; set; }
        /// <summary>
        /// A map of players to their average turn length.
        /// </summary>
        public Dictionary<Player, TimeSpan> AverageTurnLengths { get; set; }

        public PostGameViewModel(string gameId)
        {
            this.ErrorMessage = null;

            var lobby = GameLobbyManager.GameLobbies.Values.FirstOrDefault(g => g.Uid == gameId);
            if (lobby == null)
            {
                ErrorMessage = "Game not found.";
                return;
            }
            if (lobby.GameManager.GameState != GameState.EndOfGame)
            {
                ErrorMessage = "The game is not finished.";
                return;
            }
            
            // Generate data points for charts
            var history = lobby.GameManager.HistoryLog;
            if (history == null || history.LogItems == null || !history.LogItems.Any())
            {
                ErrorMessage = "Stats unavailable.";
                return;
            }
            var log = history.LogItems;

            this.GameName = lobby.Name;

            // Populate the total turn count.
            this.TotalTurnCount = log.Last().Turn + 1;

            var playerLogItems = log.OfType<PlayerLogItem>().ToList();
            var resCollectItems = playerLogItems.OfType<ResourceCollectionLogItem>().ToList();
            var diceRolls = playerLogItems.OfType<DiceRollLogItem>().ToList();
            var turnChanges = playerLogItems.OfType<PlayerTurnStateLogItem>();
            var players = playerLogItems.Select(i => i.Player).Distinct().ToList();

            // Find the resource collection totals and create data sets for all turns.
            this.CardsCollected = new Dictionary<Player, Dictionary<ResourceTypes, int[]>>();
            foreach (var p in players)
            {
                var id = p.Id;
                var playerCardsCollected = new Dictionary<ResourceTypes, int[]>();
                playerCardsCollected[ResourceTypes.None] = new int[this.TotalTurnCount];
                playerCardsCollected[ResourceTypes.Wood] = new int[this.TotalTurnCount];
                playerCardsCollected[ResourceTypes.Brick] = new int[this.TotalTurnCount];
                playerCardsCollected[ResourceTypes.Wheat] = new int[this.TotalTurnCount];
                playerCardsCollected[ResourceTypes.Sheep] = new int[this.TotalTurnCount];
                playerCardsCollected[ResourceTypes.Ore] = new int[this.TotalTurnCount];
                var totals = new Dictionary<ResourceTypes, int>();
                totals[ResourceTypes.None] = 0;
                totals[ResourceTypes.Brick] = 0;
                totals[ResourceTypes.Wood] = 0;
                totals[ResourceTypes.Wheat] = 0;
                totals[ResourceTypes.Sheep] = 0;
                totals[ResourceTypes.Ore] = 0;
                foreach (var resCollect in resCollectItems.Where(r => r.Player.Id == id))
                {
                    foreach (var resStack in resCollect.ResourcesCollected.ToList())
                    {
                        totals[resStack.Type] += resStack.Count;
                        totals[ResourceTypes.None] += resStack.Count;
                        playerCardsCollected[resStack.Type][resCollect.Turn] = totals[resStack.Type];
                        playerCardsCollected[ResourceTypes.None][resCollect.Turn] = totals[resStack.Type];
                    }
                }
                this.CardsCollected[p] = playerCardsCollected;
            }

            // Populate dice roll stats.
            this.DiceRolls = new Dictionary<int, int>();
            for (int i = 2; i <= 12; i++)
            {
                var rollCount = diceRolls.Count(r => r.Roll.Total == i);
                this.DiceRolls[i] = rollCount;
            }

            // Populate average turn times
            this.AverageTurnLengths = new Dictionary<Player, TimeSpan>();
            var allTurnLengths = new Dictionary<int, List<TimeSpan>>();
            DateTime startTimeStamp = DateTime.MinValue;
            int turnPlayerId = -1;
            foreach (var turnLog in turnChanges)
            {
                if (turnLog.TurnState == PlayerTurnStateLogItem.TurnStates.Start)
                {
                    turnPlayerId = turnLog.Player.Id;
                    startTimeStamp = turnLog.TimeStampUtc;
                }
                else if (turnLog.TurnState == PlayerTurnStateLogItem.TurnStates.End)
                {
                    if (turnPlayerId == turnLog.Player.Id && startTimeStamp != DateTime.MinValue)
                    {
                        var duration = turnLog.TimeStampUtc - startTimeStamp;
                        if (allTurnLengths.ContainsKey(turnPlayerId))
                            allTurnLengths[turnPlayerId].Add(duration);
                        else
                            allTurnLengths.Add(turnPlayerId, new List<TimeSpan> {duration});
                    }
                    turnPlayerId = -1;
                    startTimeStamp = DateTime.MinValue;
                }
            }
            foreach (var p in players)
            {
                if (allTurnLengths.ContainsKey(p.Id))
                {
                    var averageTicks = allTurnLengths[p.Id].Average(s => s.Ticks);
                    this.AverageTurnLengths[p] = new TimeSpan(Convert.ToInt64(averageTicks));
                }
            }

        }
    }
}