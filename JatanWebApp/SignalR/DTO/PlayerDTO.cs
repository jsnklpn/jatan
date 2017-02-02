using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.Models;

namespace JatanWebApp.SignalR.DTO
{
    /// <summary>
    /// Player data transfer object.
    /// </summary>
    public class PlayerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AvatarPath { get; set; }
        public PlayerColor Color { get; set; }
        public List<DevelopmentCards> DevelopmentCardsInPlay { get; set; }
        public int NumberOfResourceCards { get; set; }
        public int VictoryPointsFromCards { get; set; }
        public int ArmySize { get; set; }

        // Properties populated by game manager DTO
        public List<ResourceTypes> PortsOwned { get; set; }
        public bool AvailableToRob { get; set; } // Used when players are getting robbed
        public int CardsToLose { get; set; } // Used when a 7 is rolled and player has too many cards

        // Private data
        public ResourceCollection ResourceCards { get; set; }
        public List<DevelopmentCards> DevelopmentCards { get; set; }

        /// <summary>
        /// PlayerDTO constructor.
        /// </summary>
        /// <param name="player">The original player model.</param>
        /// <param name="includePrivateData">Include private player data, such as the cards they have in their hand.</param>
        public PlayerDTO(Player player, bool includePrivateData)
        {
            this.Id = player.Id;
            this.Name = player.Name;
            this.Color = player.Color;
            this.DevelopmentCardsInPlay = new List<DevelopmentCards>(player.DevelopmentCardsInPlay);
            this.NumberOfResourceCards = player.NumberOfResourceCards;
            this.VictoryPointsFromCards = player.VictoryPointsFromCards;
            this.ArmySize = player.ArmySize;
            if (includePrivateData)
            {
                this.ResourceCards = player.ResourceCards.Copy();
                this.DevelopmentCards = new List<DevelopmentCards>(player.DevelopmentCards);
            }
        }
    }
}