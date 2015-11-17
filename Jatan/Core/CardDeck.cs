using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Core
{
    /// <summary>
    /// A class to represent a generic deck of cards.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CardDeck<T> : IEnumerable<T>
    {
        private readonly List<T> _internalList;

        /// <summary>
        /// Create a new, empty deck.
        /// </summary>
        public CardDeck()
        {
            _internalList = new List<T>();
        }

        /// <summary>
        /// Gets the number of cards in the deck.
        /// </summary>
        public int CardCount
        {
            get { return _internalList.Count; }
        }

        /// <summary>
        /// Indicates if there are cards in the deck.
        /// </summary>
        public bool HasCards
        {
            get { return _internalList.Count > 0; }
        }

        /// <summary>
        /// Empties the deck.
        /// </summary>
        public void Clear()
        {
            _internalList.Clear();
        }

        /// <summary>
        /// Draws a card from the top of the deck. The card is removed from the deck.
        /// </summary>
        public ActionResult<T> DrawCard()
        {
            if (_internalList.Count == 0)
                return new ActionResult<T>(default(T), false, "The deck is empty.");

            var cardToRemove = _internalList[0];
            _internalList.RemoveAt(0);
            return new ActionResult<T>(cardToRemove, true);
        }

        /// <summary>
        /// Draws a card from the deck at random. The card is removed from the deck.
        /// </summary>
        public ActionResult<T> DrawRandomCard()
        {
            if (_internalList.Count == 0)
                return new ActionResult<T>(default(T), false, "The deck is empty.");
            return new ActionResult<T>(_internalList.RemoveRandom(), true);
        }

        /// <summary>
        /// Shuffles the deck.
        /// </summary>
        public void Shuffle()
        {
            _internalList.Shuffle();
        }

        /// <summary>
        /// Shuffles the deck n times.
        /// </summary>
        public void Shuffle(uint n)
        {
            for (uint i = 0; i < n; i++)
                _internalList.Shuffle();
        }

        /// <summary>
        /// Adds a card to the top or bottom of the deck.
        /// </summary>
        public void AddCard(T cardToAdd, bool topOfDeck = true)
        {
            if (topOfDeck)
                _internalList.Insert(0, cardToAdd);
            else
                _internalList.Add(cardToAdd);
        }

        /// <summary>
        /// Adds a card to the top or bottom of the deck.
        /// </summary>
        public void AddCards(IEnumerable<T> cardsToAdd, bool topOfDeck = true)
        {
            if (topOfDeck)
            {
                foreach (var card in cardsToAdd.Reverse())
                    _internalList.Insert(0, card);
            }
            else
            {
                foreach (var card in cardsToAdd)
                    _internalList.Add(card);
            }
        }

        /// <summary>
        /// Gets the string representation of the deck.
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var item in _internalList)
            {
                sb.Append(item + ", ");
            }
            return sb.ToString().TrimEnd(',', ' ');
        }

        #region IEnumerable implementation

        public IEnumerator<T> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
