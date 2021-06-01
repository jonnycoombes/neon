/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
using MongoDB.Bson;

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
}