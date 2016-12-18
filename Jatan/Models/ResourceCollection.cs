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
        public int Wood { get; set; }

        /// <summary>
        /// The brick count.
        /// </summary>
        public int Brick { get; set; }

        /// <summary>
        /// The sheep count.
        /// </summary>
        public int Sheep { get; set; }

        /// <summary>
        /// The wheat count.
        /// </summary>
        public int Wheat { get; set; }

        /// <summary>
        /// The ore count.
        /// </summary>
        public int Ore { get; set; }

        /// <summary>
        /// Returns true if this collection only contains one resource type.
        /// </summary>
        public bool IsSingleResourceType
        {
            get
            {
                var totalCount = GetResourceCount();
                foreach (var stack in this.ToList())
                {
                    if (stack.Count == totalCount)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Creates a new resource collection.
        /// </summary>
        public ResourceCollection()
        {
        }

        /// <summary>
        /// Creates new a resource collection from a stack.
        /// </summary>
        public ResourceCollection(ResourceStack resStack)
        {
            this.SetResourceCount(resStack.Type, resStack.Count);
        }

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
        /// Sets all resources to zero.
        /// </summary>
        public void Clear()
        {
            Wood = 0;
            Brick = 0;
            Sheep = 0;
            Wheat = 0;
            Ore = 0;
        }

        /// <summary>
        /// Adds a collection to another collection.
        /// </summary>
        public void Add(ResourceCollection collection)
        {
            Wood += collection.Wood;
            Brick += collection.Brick;
            Sheep += collection.Sheep;
            Wheat += collection.Wheat;
            Ore += collection.Ore;
        }

        /// <summary>
        /// Gets the total number of resources in the collection.
        /// </summary>
        /// <returns></returns>
        public int GetResourceCount()
        {
            return Wood + Brick + Sheep + Wheat + Ore;
        }

        /// <summary>
        /// Returns the largest resource stack in the collection.
        /// </summary>
        public ResourceStack GetLargestStack()
        {
            return ToList().OrderByDescending(s => s.Count).First();
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
                default: break;
            }
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
        /// Returns a copy of the collection.
        /// </summary>
        /// <returns></returns>
        public ResourceCollection Copy()
        {
            return new ResourceCollection()
            {
                Brick = this.Brick,
                Ore = this.Ore,
                Sheep = this.Sheep,
                Wheat = this.Wheat,
                Wood = this.Wood
            };
        }

        /// <summary>
        /// Returns true if both collections contains the same number of resources.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ResourceCollection other)
        {
            return (Brick == other.Brick &&
                    Ore   == other.Ore &&
                    Sheep == other.Sheep &&
                    Wheat == other.Wheat &&
                    Wood  == other.Wood);
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
