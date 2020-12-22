using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

namespace JCS.Neon.Glow.Data.Entity{
    /// <summary>
    /// An abstract base class for keyed entities which may be stored via the
    /// EF Core framework.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class KeyedEntity<T>
        where T : IComparable<T>, IEquatable<T>{
        /// <summary>
        /// Timestamp field used as a concurrency token
        /// </summary>
        [Timestamp]
        public byte[]? Timestamp { get; set; }

        /// <summary>
        /// The primary key field of type T
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public T Id { get; set; }

        /// <summary>
        /// The creation time instant for the entity
        /// </summary>
        public Instant CreationTime { get; set; } = Instant.FromDateTimeUtc(DateTime.Now.ToUniversalTime());
    }
}