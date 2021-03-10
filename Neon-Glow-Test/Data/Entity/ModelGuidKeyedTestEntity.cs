#region

using System;
using JCS.Neon.Glow.Data.Entity;

#endregion

namespace JCS.Neon.Glow.Test.Data.Entity
{
    /// <summary>
    ///     Simple entity used for unit testing
    /// </summary>
    public class ModelGuidKeyedTestEntity : KeyedEntity<Guid>
    {
        /// <summary>
        ///     Sample string property
        /// </summary>
        public string StringProperty { get; set; }

        /// <summary>
        ///     Sample integer property
        /// </summary>
        public int IntegerProperty { get; set; }

        /// <summary>
        ///     Just overrides toString in order to dump the contents of the object
        /// </summary>
        /// <returns>String representation of the entity</returns>
        public override string ToString()
        {
            return $"[{Id},{CreationTime},{StringProperty},{IntegerProperty}]";
        }
    }
}