namespace JatanWebAppCore.Data
{
    /// <summary>
    /// Base class for custom EF models.
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// DB primary key
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Gets the UTC date when this record was created.
        /// </summary>
        public DateTime CreatedDateUtc { get; set; }

        public EntityBase()
        {
            Id = Guid.NewGuid().ToString();
            CreatedDateUtc = DateTime.UtcNow;
        }
    }
}