/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using JCS.Neon.Glow.Data.Repository.Mongo.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    ///     Test class which acts as the base class of an inheritance heirarchy
    /// </summary>
    [BsonKnownTypes(new [] {typeof(SubClassA), typeof(SubClassB)})]
    [Index(new []{"_t"})]
    public class PolymorphicBase
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }

    /// <summary>
    ///     Subclass for testing
    /// </summary>
    [Collection(Name = "polymorphicBase")]
    public class SubClassA : PolymorphicBase
    {
        public string ClassAProperty { get; set; }
    }

    /// <summary>
    ///     Subclass for testing
    /// </summary>
    [Collection(Name = "polymorphicBase")]
    public class SubClassB : PolymorphicBase
    {
        public string ClassBProperty { get; set; }
    }
}