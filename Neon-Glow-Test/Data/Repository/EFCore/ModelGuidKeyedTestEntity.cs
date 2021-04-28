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