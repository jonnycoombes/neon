/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
using JCS.Neon.Glow.Data.Repository.Mongo;

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    /// A test entity which is not attributed in any way. Has a number of properties of differing types
    /// </summary>
    public class NonAttributedEntity
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// A test entity which has been attributed using <see cref="Collection"/> and <see cref="Index"/> custom attributes
    /// </summary>
    [Collection(Name="TestCollection")]
    public class AttributedEntity
    {
        
    }
}