using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Models
{
    /// <summary>
    /// Represents a collection of every resource.
    /// </summary>
    public class ResourceCollection
    {
        /// <summary>
        /// The wood count.
        /// </summary>
        public int Wood;

        /// <summary>
        /// The brick count.
        /// </summary>
        public int Brick;

        /// <summary>
        /// The sheep count.
        /// </summary>
        public int Sheep;

        /// <summary>
        /// The wheat count.
        /// </summary>
        public int Wheat;

        /// <summary>
        /// The ore count.
        /// </summary>
        public int Ore;

        /// <summary>
        /// Returns non-zero resources as a list.
        /// </summary>
        /// <returns></returns>
        public List<ResourceStack> ToList()
        {
            var result = new List<ResourceStack>();
            if (IsEmpty()) return result;

            if (Wood !=  0) result.Add(new ResourceStack(ResourceTypes.Wood, Wood));
            if (Brick != 0) result.Add(new ResourceStack(ResourceTypes.Brick, Brick));
            if (Sheep != 0) result.Add(new ResourceStack(ResourceTypes.Sheep, Sheep));
            if (Wheat != 0) result.Add(new ResourceStack(ResourceTypes.Wheat, Wheat));
            if (Ore != 0) result.Add(new ResourceStack(ResourceTypes.Ore, Ore));

            return result;
        }

        /// <summary>
        /// Gets the number of a certain resource.
        /// </summary>
        public int GetResourceCount(ResourceTypes type)
        {
            switch(type)
            {
                case ResourceTypes.Wood: return Wood;
                case ResourceTypes.Brick: return Brick;
                case ResourceTypes.Sheep: return Sheep;
                case ResourceTypes.Wheat: return Wheat;
                case ResourceTypes.Ore: return Ore;
                default: return 0;
            }
        }

        /// <summary>
        /// Sets the number of a certain resource.
        /// </summary>
        public void SetResourceCount(ResourceTypes type, int count)
        {
            switch (type)
            {
                case ResourceTypes.Wood: Wood = count; break;
                case ResourceTypes.Brick: Brick = count; break;
                case ResourceTypes.Sheep: Sheep = count; break;
                case ResourceTypes.Wheat: Wheat = count; break;
                case ResourceTypes.Ore: Ore = count; break;
            }
        }

        /// <summary>
        /// Increments the number of a certain resource.
        /// </summary>
        public void IncrementResourceCount(ResourceTypes type, int incrementAmount)
        {
            var current = GetResourceCount(type);
            SetResourceCount(type, current + incrementAmount);
        }

        /// <summary>
        /// The [] operator overload.
        /// </summary>
        public int this[ResourceTypes key]
        {
            get { return GetResourceCount(key); }
            set { SetResourceCount(key, value); }
        }

        /// <summary>
        /// Returns true if every resource has a count of zero.
        /// </summary>
        public bool IsEmpty()
        {
            return Wood == 0 && Brick == 0 && Sheep == 0 && Wheat == 0 && Ore == 0;
        }

        /// <summary>
        /// Returns true if there are any resources.
        /// </summary>
        public bool Any()
        {
            return !IsEmpty();
        }

        /// <summary>
        /// Returns an empty collection.
        /// </summary>
        public static ResourceCollection Empty
        {
            get { return new ResourceCollection(); }
        }
    }
}
