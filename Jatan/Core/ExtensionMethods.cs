using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Core
{
    /// <summary>
    /// Extension methods
    /// </summary>
    static class ExtensionMethods
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Shuffles a list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
                list.Swap(i, _random.Next(i, list.Count));
        }

        /// <summary>
        /// Swaps two items in a list.
        /// </summary>
        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        /// <summary>
        /// Adds an item to the list n times.
        /// </summary>
        public static void Add<T>(this IList<T> list, T item, int n)
        {
            for (int i = 0; i < n; i++)
                list.Add(item);
        }

        /// <summary>
        /// Removes a random item from the list
        /// </summary>
        public static T RemoveRandom<T>(this IList<T> list)
        {
            if (list.Count == 0)
                throw new IndexOutOfRangeException("You cannot remove an item fom an empty list.");
            var itemIndex = _random.Next(list.Count);
            var item = list[itemIndex];
            list.RemoveAt(itemIndex);
            return item;
        }

        /// <summary>
        /// Gets a random item from the list
        /// </summary>
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list.Count == 0)
                throw new IndexOutOfRangeException("You cannot get an item fom an empty list.");
            var itemIndex = _random.Next(list.Count);
            var item = list[itemIndex];
            return item;
        }

        /// <summary>
        /// Removes and returns the last item from the list.
        /// </summary>
        public static T TakeLast<T>(this IList<T> list)
        {
            if (list.Count == 0)
                throw new IndexOutOfRangeException("You cannot remove an item fom an empty list.");
            var itemIndex = list.Count - 1;
            var item = list[itemIndex];
            list.RemoveAt(itemIndex);
            return item;
        }
    }
}
