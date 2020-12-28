using System;
using JCS.Neon.Glow.Data.Entity;

namespace JCS.Neon.Glow.Test.Data.Entity
{
    /// <summary>
    /// Simple entity used for unit testing
    /// </summary>
    public class ModelGuidKeyedTestEntity : KeyedEntity<Guid>
    {

        /// <summary>
        /// Sample string property
        /// </summary>
        public string StringProperty { get; set; }
        
        public int IntegerProperty { get; set; }
        public override string ToString()
        {
            return $"[{Id},{CreationTime},{StringProperty},{IntegerProperty}]";
        }
    }
}