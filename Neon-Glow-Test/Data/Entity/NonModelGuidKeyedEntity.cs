using System;
using JCS.Neon.Glow.Data.Entity;

namespace JCS.Neon.Glow.Test.Data.Entity
{
    /// <summary>
    /// Keyed entity which isn't mapped to the main test context
    /// </summary>
    public class NonModelGuidKeyedEntity : KeyedEntity<Guid>
    {
    }
}