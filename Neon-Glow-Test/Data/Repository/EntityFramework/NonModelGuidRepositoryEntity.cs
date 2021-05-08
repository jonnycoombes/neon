/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using JCS.Neon.Glow.Data.Repository.EntityFramework;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.EntityFramework
{
    /// <summary>
    ///     Keyed entity which isn't mapped to the main test context
    /// </summary>
    public class NonModelGuidRepositoryEntity : RepositoryEntity<Guid>
    {
    
    }
}