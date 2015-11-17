using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Models
{
    /// <summary>
    /// Struct to represent a resource tile
    /// </summary>
    public struct ResourceTile
    {
        /// <summary>
        /// The type of resource this tile gives.
        /// </summary>
        public readonly ResourceTypes Resource;

        /// <summary>
        /// The dice roll which will cause this tile to give a resource.
        /// </summary>
        public readonly int RetrieveNumber;

        /// <summary>
        /// Creates a resource tile.
        /// </summary>
        /// <param name="resource">The type of resource.</param>
        /// <param name="retrieveNumber">The dice roll which will cause this tile to give a resource.</param>
        public ResourceTile(ResourceTypes resource, int retrieveNumber)
        {
            Resource = resource;
            RetrieveNumber = retrieveNumber;
        }

        /// <summary>
        /// Create a desert tile which gives no resources.
        /// </summary>
        public static ResourceTile DesertTile 
        {
            get { return new ResourceTile(ResourceTypes.None, 0); }
        }

        /// <summary>
        /// Gets the string representation of the resource tile.
        /// </summary>
        public override string ToString()
        {
            return string.Format("[{0} - {1}]", RetrieveNumber, Resource);
        }
    }
}
