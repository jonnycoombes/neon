/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using MongoDB.Bson.Serialization;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     An interface which can be implemented by a given type in order to get a callback from <see cref="DbContext" />
    ///     instances during
    ///     binding operations in order to customise class mapping
    /// </summary>
    /// <typeparam name="T">The type which the implementation of the interface knows how to map</typeparam>
    public interface ISupportsClassmap<T>
    {
        /// <summary>
        ///     This action will be called by instances of <see cref="DbContext" /> during bind operations
        /// </summary>
        /// <param name="cm">A classmap instance</param>
        /// <returns>Nowt, nada, dim byd</returns>
        void ConfigureClassmap(BsonClassMap<T> cm);
    }
}