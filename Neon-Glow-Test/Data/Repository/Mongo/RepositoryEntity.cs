/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using System.Collections.Generic;
using JCS.Neon.Glow.Data.Repository.Mongo;
using JCS.Neon.Glow.Data.Repository.Mongo.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    ///     A test entity which has been attributed using <see cref="Collection" /> and
    ///     <see cref="Glow.Data.Repository.Mongo.Attributes.Index" /> custom attributes
    /// </summary>
    [Collection(Name = "Repository", Capped = false, ValidationAction = DocumentValidationAction.Warn)]
    [BsonKnownTypes(new []{typeof(PolymorphicEntity)})]
    [Glow.Data.Repository.Mongo.Attributes.Index(new[] {"Deleted"}, Unique = false)]
    [Glow.Data.Repository.Mongo.Attributes.Index(new[] {"VersionToken.Value"})]
    public class RepositoryEntity : RepositoryObject, ISupportsClassmap<RepositoryEntity>
    {
        /// <summary>
        ///     Test string property
        /// </summary>
        [IndexField(IsText = true)]
        public string StringProperty { get; set; }

        /// <summary>
        ///     Test integer property
        /// </summary>
        [IndexField(Ascending = false)]
        public int IntProperty { get; set; }

        /// <summary>
        ///     Test float property
        /// </summary>
        public float FloatProperty { get; set; }

        /// <summary>
        ///     Test date time property
        /// </summary>
        [IndexField(Ascending = false)]
        public DateTime DateTimeProperty { get; set; }

        /// <summary>
        ///     Byte array property
        /// </summary>
        public byte[] ByteArrayProperty { get; set; }

        public void ConfigureClassmap(BsonClassMap<RepositoryEntity> cm)
        {
            cm.AutoMap();
        }
    }

    [Collection(Name = "Repository")]
    public class PolymorphicEntity : RepositoryEntity
    {
        public string SubclassStringProperty { get; set; }

        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public Dictionary<string, string> TestPropertyBag { get; set; } = new Dictionary<string, string>()
        {
            {"test", "test"}
        };
    }
}