/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     An attribute which allows the collection associated with a given BSON model type to be specified
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MongoCollection : Attribute
    {
        /// <summary>
        ///     The name of the collection to be associated with the attributed class
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        ///     The default constructor takes the name of a collection to be associated with the attributed class
        /// </summary>
        /// <param name="collectionName">The name of the target collection for the attributed class</param>
        public MongoCollection(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}