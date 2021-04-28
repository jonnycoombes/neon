/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using JCS.Neon.Glow.Data.Repository.EFCore;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.EFCore
{
    /// <summary>
    ///     Keyed entity which isn't mapped to the main test context
    /// </summary>
    public class NonModelGuidKeyedEntity : KeyedEntity<Guid>
    {
    
    }
}