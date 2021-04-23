#region

using System;
using JCS.Neon.Glow.Data.Repository.EFCore;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.EFCore.Entity
{
    /// <summary>
    ///     Keyed entity which isn't mapped to the main test context
    /// </summary>
    public class NonModelGuidKeyedEntity : KeyedEntity<Guid>
    {
    
    }
}