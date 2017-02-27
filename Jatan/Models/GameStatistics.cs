using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jatan.GameLogic;

namespace Jatan.Models
{
    /// <summary>
    /// Object to calculate and store the stats for a completed game.
    /// </summary>
    public class GameStatistics
    {
        /// <summary>
        /// The name of the winner.
        /// </summary>
        public string WinnerName { get; set; }
        /// <summary>
        /// The total number of turns in the game.
        /// </summary>
        public int TotalTurnCount { get; set; }
        /// <summary>
        /// The total length of the game.
        /// </summary>
        public TimeSpan GameLength { get; set; }
        /// <summary>
        /// A map of players to a map of resources to the list of amount collected.
        /// </summary>
        public Dictionary<Player, Dictionary<ResourceTypes, int[]>> CardsCollected { get; set; }
        /// <summary>
        /// A map of numbers to the number of times that number was rolled. (From 2 to 12)
        /// </summary>
        public Dictionary<int, int> DiceRolls { get; set; }
        /// <summary>
        /// A map of players to their average turn length.
        /// </summary>
        public Dictionary<Player, TimeSpan> AverageTurnLengths { get; set; }

        /// <summary>
        /// Creates a new GameStatistics object from a game instance.
        /// </summary>
        public GameStatistics(GameManager manager)
        {
            this.DiceRolls = new Dictionary<int, int>();
            this.CardsCollected = new Dictionary<Player, Dictionary<ResourceTypes, int[]>>();
            this.AverageTurnLengths = new Dictionary<Player, TimeSpan>();

            if (manager == null) return;

            // Generate data points for charts
            var history = manager.HistoryLog;
            if (history == null || history.LogItems == null || !history.LogItems.Any())
            {
                return;
            }
            var log = history.LogItems;

            // Populate the winner name
            var winnerId = manager.WinnerPlayerId;
            var winnerLog = log.OfType<PlayerLogItem>().FirstOrDefault(i => i.Player.Id == winnerId);
            if (winnerLog != null)
            {
                this.WinnerName = winnerLog.Player.Name;
            }

            // Populate the total game length
            if (log.Any())
            {
                var first = log.First().TimeStampUtc;
                var last = log.Last().TimeStampUtc;
                this.GameLength = last - first;
            }
            else
            {
                this.GameLength = TimeSpan.Zero;
            }

            // Populate the total turn count.
            this.TotalTurnCount = log.Last().Turn + 1;

            var playerLogItems = log.OfType<PlayerLogItem>().ToList();
            var resCollectItems = playerLogItems.OfType<ResourceCollectionLogItem>().ToList();
            var diceRolls = playerLogItems.OfType<DiceRollLogItem>().ToList();
            var turnChanges = playerLogItems.OfType<PlayerTurnStateLogItem>();
            var players = playerLogItems.Select(i => i.Player).Distinct().ToList();

            // Find the resource collection totals and create data sets for all turns.
            foreach (var p in players)
            {
                var id = p.Id;
                var playerCardsCollected = new Dictionary<ResourceTypes, int[]>();
                var totals = new Dictionary<ResourceTypes, int>();
                foreach (var res in Enum.GetValues(typeof(ResourceTypes)).Cast<ResourceTypes>())
                {
                    playerCardsCollected[res] = new int[this.TotalTurnCount];
                    totals[res] = 0;
                }
                foreach (var resCollect in resCollectItems.Where(r => r.Player.Id == id))
                {
                    foreach (var resStack in resCollect.ResourcesCollected.ToList())
                    {
                        totals[resStack.Type] += resStack.Count;
                        totals[ResourceTypes.None] += resStack.Count;
                        playerCardsCollected[resStack.Type][resCollect.Turn] = totals[resStack.Type];
                        playerCardsCollected[ResourceTypes.None][resCollect.Turn] = totals[ResourceTypes.None];
                    }
                }
                foreach (var res in Enum.GetValues(typeof(ResourceTypes)).Cast<ResourceTypes>())
                {
                    // Fix all the '0' data points
                    for (int i = 1; i < this.TotalTurnCount; i++)
                    {
                        if (playerCardsCollected[res][i] == 0)
                        {
                            playerCardsCollected[res][i] = playerCardsCollected[res][i - 1];
                        }
                    }
                }
                this.CardsCollected[p] = playerCardsCollected;
            }

            // Populate dice roll stats.
            for (int i = 2; i <= 12; i++)
            {
                var rollCount = diceRolls.Count(r => r.Roll.Total == i);
                this.DiceRolls[i] = rollCount;
            }

            // Populate average turn times
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

        /// <summary>
        /// Returns fake game statistics. Used for testing.
        /// </summary>
        public static GameStatistics CreateTestStats()
        {
            var stats = new GameStatistics(null);
            var rand = new Random();

            var players = new List<Player>()
            {
                new Player(0, "billy", PlayerColor.Blue),
                new Player(1, "john", PlayerColor.Red),
                new Player(2, "robert", PlayerColor.Green),
                new Player(3, "jim", PlayerColor.Yellow)
            };

            stats.WinnerName = "john";
            stats.GameLength = TimeSpan.FromMinutes(rand.Next(30, 300));
            stats.TotalTurnCount = rand.Next(50, 100);

            for (int i = 2; i <= 12; i++)
            {
                stats.DiceRolls[i] = rand.Next(20);
            }

            foreach (var p in players)
            {
                stats.AverageTurnLengths[p] = TimeSpan.FromSeconds(rand.Next(10, 100));
            }

            foreach (var p in players)
            {
                stats.CardsCollected[p] = new Dictionary<ResourceTypes, int[]>();

                foreach (var res in Enum.GetValues(typeof(ResourceTypes)).Cast<ResourceTypes>())
                {
                    stats.CardsCollected[p][res] = new int[stats.TotalTurnCount];
                }

                foreach (var res in Enum.GetValues(typeof(ResourceTypes)).Cast<ResourceTypes>())
                {
                    if (res == ResourceTypes.None) continue;
                    
                    var runningTotal = 0;
                    for (var i = 0; i < stats.TotalTurnCount; i++)
                    {
                        var numCollected = rand.Next(0, 5) == 0 ? rand.Next(1, 3) : 0;
                        runningTotal += numCollected;
                        stats.CardsCollected[p][res][i] = runningTotal;
                        stats.CardsCollected[p][ResourceTypes.None][i] += runningTotal;
                    }
                }
            }
            return stats;
        }
    }
}
