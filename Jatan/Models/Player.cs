using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Jatan.Core;

namespace Jatan.Models
{
    /// <summary>
    /// A class to represent a player.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique Id of the player. The server should manage these.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The color of the player pieces.
        /// </summary>
        public PlayerColor Color { get; set; }

        /// <summary>
        /// Gets the remaining roads this player has.
        /// </summary>
        public int RoadsAvailable { get; set; }

        /// <summary>
        /// Gets the remaining settlements this player has.
        /// </summary>
        public int SettlementsAvailable { get; set; }

        /// <summary>
        /// Gets the remaining cities this player has.
        /// </summary>
        public int CitiesAvailable { get; set; }

        /// <summary>
        /// Gets the resource cards this player has in their hand.
        /// </summary>
        public ResourceCollection ResourceCards { get; set; }

        /// <summary>
        /// Gets the development cards this player has in their hand.
        /// </summary>
        public List<DevelopmentCards> DevelopmentCards { get; set; }

        /// <summary>
        /// Gets the development cards this player has played.
        /// </summary>
        public List<DevelopmentCards> DevelopmentCardsInPlay { get; set; }

        /// <summary>
        /// Gets the number of resource cards.
        /// </summary>
        public int NumberOfResourceCards { get { return ResourceCards.GetResourceCount(); } }

        /// <summary>
        /// Gets the number of victory points this player has from cards alone.
        /// </summary>
        public int VictoryPointsFromCards
        {
            get
            {
                return DevelopmentCardsInPlay.Count(
                        c => c == Models.DevelopmentCards.Library ||
                            c == Models.DevelopmentCards.Chapel ||
                            c == Models.DevelopmentCards.Market ||
                            c == Models.DevelopmentCards.University ||
                            c == Models.DevelopmentCards.GreatHall);
            }
        }

        /// <summary>
        /// Gets the number of knights this player has played.
        /// </summary>
        public int ArmySize
        {
            get { return DevelopmentCardsInPlay.Count(c => c == Models.DevelopmentCards.Knight); }
        }

        /// <summary>
        /// Creates a new player.
        /// </summary>
        public Player(int id, string name, PlayerColor color)
        {
            Name = name;
            Id = id;
            Color = color;
            RoadsAvailable = 15;
            SettlementsAvailable = 5;
            CitiesAvailable = 4;
            ResourceCards = new ResourceCollection();
            DevelopmentCards = new List<DevelopmentCards>();
            DevelopmentCardsInPlay = new List<DevelopmentCards>();
        }

        /// <summary>
        /// Gets the number of resource cards this player has for a certain type.
        /// </summary>
        public int GetNumberResourceCount(ResourceTypes resource)
        {
            return ResourceCards[resource];
        }

        /// <summary>
        /// Returns true if the player has at least n of the indicated resource.
        /// </summary>
        public bool HasAtLeast(ResourceTypes resource, int n)
        {
            return ResourceCards[resource] >= n;
        }

        /// <summary>
        /// Returns true if the player has at least the resouces contained in the stack.
        /// </summary>
        public bool HasAtLeast(ResourceStack stack)
        {
            return ResourceCards[stack.Type] >= stack.Count;
        }

        /// <summary>
        /// Returns true if the player has at least the resouces contained in the collection.
        /// </summary>
        public bool HasAtLeast(ResourceCollection collection)
        {
            foreach (var stack in collection.ToList())
            {
                if (!HasAtLeast(stack)) return false;
            }
            return true;
        }

        /// <summary>
        /// Adds a collection of resources into the player's hand.
        /// </summary>
        public void AddResources(ResourceCollection collection)
        {
            ResourceCards.Add(collection);
        }

        /// <summary>
        /// Removes a number of resource cards of a certain type. Returns false if the player doesn't have enough.
        /// </summary>
        public bool RemoveResources(ResourceTypes resource, int count)
        {
            if (!HasAtLeast(resource, count))
                return false;
            ResourceCards[resource] -= count;
            return true;
        }

        /// <summary>
        /// Removes a number of resource cards of a certain type. Returns false if the player doesn't have enough.
        /// </summary>
        public bool RemoveResources(ResourceStack stack)
        {
            return RemoveResources(stack.Type, stack.Count);
        }

        /// <summary>
        /// Removes a number of resource cards of a certain type. Returns false if the player doesn't have enough.
        /// </summary>
        public bool RemoveResources(ResourceCollection collection)
        {
            if (!HasAtLeast(collection))
                return false;
            foreach (var stack in collection.ToList())
                RemoveResources(stack);
            return true;
        }

        /// <summary>
        /// Removes all resource cards of a certain type. Returns the number of cards removed.
        /// </summary>
        public int RemoveAllResources(ResourceTypes resource)
        {
            var count = ResourceCards[resource];
            ResourceCards[resource] = 0;
            return count;
        }

        /// <summary>
        /// Removes all resource cards. Returns the number of cards removed.
        /// </summary>
        public int RemoveAllResources()
        {
            var count = ResourceCards.GetResourceCount();
            ResourceCards.Clear();
            return count;
        }

        /// <summary>
        /// Removes the specified number of random cards from the player.
        /// </summary>
        public void RemoveRandomResources(int count)
        {
            // Get a copy of the resources list
            var resourceList = ResourceCards.ToResourceTypeList();
            for (int i = 0; i < count && resourceList.Any(); i++)
            {
                var typeToRemove = resourceList.RemoveRandom();
                ResourceCards[typeToRemove]--;
            }
        }

        /// <summary>
        /// Returns true if the player can afford a certain item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CanAfford(PurchasableItems item)
        {
            switch (item)
            {
                case PurchasableItems.Road: 
                    return HasAtLeast(ResourceTypes.Wood, 1) &&
                           HasAtLeast(ResourceTypes.Brick, 1);
                case PurchasableItems.Settlement:
                    return HasAtLeast(ResourceTypes.Wood, 1) &&
                           HasAtLeast(ResourceTypes.Brick, 1) &&
                           HasAtLeast(ResourceTypes.Wheat, 1) &&
                           HasAtLeast(ResourceTypes.Sheep, 1);
                case PurchasableItems.City:
                    return HasAtLeast(ResourceTypes.Wheat, 2) &&
                           HasAtLeast(ResourceTypes.Ore, 3);
                case PurchasableItems.DevelopmentCard:
                    return HasAtLeast(ResourceTypes.Wheat, 1) &&
                           HasAtLeast(ResourceTypes.Sheep, 1) &&
                           HasAtLeast(ResourceTypes.Ore, 1);
            }
            return false;
        }

        /// <summary>
        /// Removes resource from a player so they can purchase the specified item. Returns false if they can't afford it.
        /// </summary>
        public ActionResult Purchase(PurchasableItems item, bool isItemFree = false)
        {
            if (!isItemFree && !CanAfford(item))
                return ActionResult.CreateFailed("Cannot afford this.");

            switch (item)
            {
                case PurchasableItems.Road:
                    if (RoadsAvailable <= 0)
                        return ActionResult.CreateFailed("No more roads available.");
                    if (!isItemFree)
                    {
                        RemoveResources(ResourceTypes.Wood, 1);
                        RemoveResources(ResourceTypes.Brick, 1);
                    }
                    RoadsAvailable--;
                    break;
                case PurchasableItems.Settlement:
                    if (SettlementsAvailable <= 0)
                        return ActionResult.CreateFailed("No more settlements available.");
                    if (!isItemFree)
                    {
                        RemoveResources(ResourceTypes.Wood, 1);
                        RemoveResources(ResourceTypes.Brick, 1);
                        RemoveResources(ResourceTypes.Wheat, 1);
                        RemoveResources(ResourceTypes.Sheep, 1);
                    }
                    SettlementsAvailable--;
                    break;
                case PurchasableItems.City:
                    if (CitiesAvailable <= 0)
                        return ActionResult.CreateFailed("No more cities available.");
                    if (!isItemFree)
                    {
                        RemoveResources(ResourceTypes.Wheat, 2);
                        RemoveResources(ResourceTypes.Ore, 3);
                    }
                    SettlementsAvailable++;
                    CitiesAvailable--;
                    break;
                case PurchasableItems.DevelopmentCard:
                    if (!isItemFree)
                    {
                        RemoveResources(ResourceTypes.Wheat, 1);
                        RemoveResources(ResourceTypes.Sheep, 1);
                        RemoveResources(ResourceTypes.Ore, 1);
                    }
                    break;
            }
            return ActionResult.CreateSuccess();
        }

        /// <summary>
        /// Returns true if this player can afford to do the specified trade offered by a different player.
        /// </summary>
        public bool CanAffordTradeOffer(TradeOffer otherPlayerOffer)
        {
            return HasAtLeast(otherPlayerOffer.ToReceive);
        }

        /// <summary>
        /// Accepts a trade offer from another player and does the resouces exchange.
        /// </summary>
        public ActionResult AcceptTradeOffer(Player otherPlayer, TradeOffer offer)
        {
            if (!CanAffordTradeOffer(offer))
            {
                return ActionResult.CreateFailed("Accepting player cannot afford the trade.");
            }
            if (!otherPlayer.HasAtLeast(offer.ToGive))
            {
                return ActionResult.CreateFailed("Offering player cannot afford the trade.");
            }

            this.RemoveResources(offer.ToReceive);
            otherPlayer.RemoveResources(offer.ToGive);

            this.AddResources(offer.ToGive);
            otherPlayer.AddResources(offer.ToReceive);

            return ActionResult.CreateSuccess();
        }

        /// <summary>
        /// Does a resource exchange with the bank.
        /// </summary>
        public ActionResult DoTradeWithBank(TradeOffer offer, IList<Port> portsAvailable)
        {
            if (!offer.IsValidBankOffer) return ActionResult.CreateFailed("Invalid trade. You can only trade a single resource type.");

            var toReceive = offer.ToReceive.GetLargestStack();
            var toGive = offer.ToGive.GetLargestStack();

            if (toGive.Type == toReceive.Type)
                return ActionResult.CreateFailed("Invalid trade. Both resource types are identical.");

            bool threeToOnePort = portsAvailable.Any(p => p.Resource == ResourceTypes.None);
            bool twoToOnePort = portsAvailable.Any(p => p.Resource == toGive.Type);
            bool doTrade = false;

            doTrade = ((toReceive.Count*4) == toGive.Count) ||
                      (threeToOnePort && ((toReceive.Count*3) == toGive.Count)) ||
                      (twoToOnePort && ((toReceive.Count*2) == toGive.Count));

            if (!doTrade)
                return ActionResult.CreateFailed("Invalid trade. Resource counts are invalid.");

            if (!this.RemoveResources(offer.ToGive))
                return ActionResult.CreateFailed("Invalid trade. Cannot afford to give these resources.");

            this.AddResources(offer.ToReceive);
            return ActionResult.CreateSuccess();
        }

        /// <summary>
        /// Removes a development card from the players hand and puts it into their "CardsInPlay" list.
        /// </summary>
        public ActionResult PlayDevelopmentCard(DevelopmentCards card)
        {
            var foundIndex = DevelopmentCards.FindIndex(c => c == card);
            if (foundIndex == -1)
                return ActionResult.CreateFailed("Cannot play this card because it is not in the player's hand.");

            DevelopmentCards.RemoveAt(foundIndex);
            DevelopmentCardsInPlay.Add(card);

            return ActionResult.CreateSuccess();
        }
    }
}
