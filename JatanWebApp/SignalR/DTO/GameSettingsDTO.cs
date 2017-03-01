using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.GameLogic;
using Jatan.Models;

namespace JatanWebApp.SignalR.DTO
{
    public class GameSettingsDTO
    {
        public int WinScore { get; set; }
        public RobberMode RobberMode { get; set; }
        public int CardLoss { get; set; }
        public int LongestRoad { get; set; }
        public int TurnTimeLimit { get; set; }
        public bool PlayerTrading { get; set; }

        public GameSettingsDTO(GameSettings settings)
        {
            this.WinScore = settings.ScoreNeededToWin;
            this.RobberMode = settings.RobberMode;
            this.CardLoss = settings.CardCountLossThreshold;
            this.LongestRoad = settings.MinimumLongestRoad;
            this.TurnTimeLimit = settings.TurnTimeLimit;
            this.PlayerTrading = settings.AllowPlayerTrading;
        }
    }
}