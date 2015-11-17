using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Models
{
    /// <summary>
    /// Represents a port.
    /// </summary>
    public struct Port
    {
        /// <summary>
        /// The resource which can be traded here.
        /// </summary>
        public readonly ResourceTypes Resource;

        /// <summary>
        /// Indicates if this a 3-to-1 any-resource port.
        /// </summary>
        public bool CanTradeAnyResource { get { return Resource == ResourceTypes.None; } }

        /// <summary>
        /// Creates a new port
        /// </summary>
        /// <param name="resource"></param>
        public Port(ResourceTypes resource)
        {
            Resource = resource;
        }

        /// <summary>
        /// A 3-to-1 any-resource port.
        /// </summary>
        public static Port AnyResourcePort { get { return new Port(ResourceTypes.None); } }
    }
}
