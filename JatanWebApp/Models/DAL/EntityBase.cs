using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JatanWebApp.Models.DAL
{
    /// <summary>
    /// Base class for EF models.
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// DB primary key
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Gets the UTC date when this record was created.
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Constructor for EntityBase
        /// </summary>
        protected EntityBase()
        {
            CreatedOnUtc = DateTime.UtcNow;
        }
    }
}