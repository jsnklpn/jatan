using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Core.Serialization
{
    /// <summary>
    /// Interface for a class that can be created from a string.
    /// </summary>
    public interface IStringSerializable
    {
        /// <summary>
        /// Populates the properties and fields of this object from a string.
        /// </summary>
        void FromString(string value);
    }
}
