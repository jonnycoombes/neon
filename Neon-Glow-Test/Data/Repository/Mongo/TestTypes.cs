/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region
using JCS.Neon.Glow.Data.Repository.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    ///     A test entity which is not attributed in any way. Has a number of properties of differing types
    /// </summary>
    public class NonAttributedEntity
    {
        /// <summary>
        ///     The identifier field for the class
        /// </summary>
        public ObjectId Id { get; set; }

        /// <summary>
        ///     Test string property
        /// </summary>
        public string StringProperty { get; set; }

        /// <summary>
        ///     Test integer property
        /// </summary>
        public int IntProperty { get; set; }

        /// <summary>
        ///     Test float property
        /// </summary>
        public float FloatProperty { get; set; }

        /// <summary>
        ///     Test date time property
        /// </summary>
        public System.DateTime DateTimeProperty { get; set; }

        /// <summary>
        ///     Byte array property
        /// </summary>
        public byte[] ByteArrayProperty { get; set; }
    }

    /// <summary>
    ///     A test entity which has been attributed using <see cref="Collection" /> and
    ///     <see cref="Glow.Data.Repository.Mongo.Index" /> custom attributes
    /// </summary>
    [Collection(Name = "TestCollection", Capped = false, ValidationAction = DocumentValidationAction.Warn)]
    [Index(new string[]{"StringProperty", "IntProperty"}, Name= "TestIdx", Unique = true)]
    [Index(new string[]{"FloatProperty"}, Unique = false)]
    public class AttributedEntity
    {
        /// <summary>
        ///     The identifier field for the class
        /// </summary>
        public ObjectId Id { get; set; }

        /// <summary>
        ///     Test string property
        /// </summary>
        [IndexField(Ascending = true)]
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
        public System.DateTime DateTimeProperty { get; set; }

        /// <summary>
        ///     Byte array property
        /// </summary>
        public byte[] ByteArrayProperty { get; set; }
    }
}