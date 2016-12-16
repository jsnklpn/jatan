using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Models
{
    /// <summary>
    /// Represents a certain number of a resource type.
    /// </summary>
    public struct ResourceStack
    {
        /// <summary>
        /// Gets the type of resource.
        /// </summary>
        public ResourceTypes Type;

        /// <summary>
        /// Gets the number of resources.
        /// </summary>
        public int Count;

        /// <summary>
        /// Creates a new resource stack.
        /// </summary>
        public ResourceStack(ResourceTypes type, int count)
        {
            Type = type;
            Count = count;
        }

        /// <summary>
        /// Converts to a resource collection.
        /// </summary>
        /// <returns></returns>
        public ResourceCollection ToResourceCollection()
        {
            var collection = new ResourceCollection();
            collection[this.Type] = this.Count;
            return collection;
        }
    }
}
