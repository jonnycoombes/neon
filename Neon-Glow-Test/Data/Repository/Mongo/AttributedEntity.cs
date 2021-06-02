/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using JCS.Neon.Glow.Data.Repository.Mongo;
using JCS.Neon.Glow.Data.Repository.Mongo.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    ///     A test entity which has been attributed using <see cref="Collection" /> and
    ///     <see cref="Glow.Data.Repository.Mongo.Attributes.Index" /> custom attributes
    /// </summary>
    [Collection(Name = "AttributedEntity", Capped = false, ValidationAction = DocumentValidationAction.Warn)]
    [Glow.Data.Repository.Mongo.Attributes.Index(new[] {"StringProperty"}, Unique = false)]
    [Glow.Data.Repository.Mongo.Attributes.Index(new[] {"IntProperty", "DateTimeProperty"}, Unique = false)]
    [Glow.Data.Repository.Mongo.Attributes.Index(new[] {"FloatProperty"}, Unique = false)]
    [Glow.Data.Repository.Mongo.Attributes.Index(new[] {"VersionToken.Value"})]
    public class AttributedEntity :  ISupportsClassmap<AttributedEntity>
    {

        /// <summary>
        /// The id
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

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

        public void ConfigureClassmap(BsonClassMap<AttributedEntity> cm)
        {
            cm.AutoMap();
        }
    }
}