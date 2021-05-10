#region

using System;
using MongoDB.Driver;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     An attribute which allows the collection associated with a given BSON model type to be specified
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class Collection : Attribute
    {
        /// <summary>
        ///     The name of the collection to be associated with the attributed class
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        ///     The maximum size in bytes for the collection
        /// </summary>
        public long MaxSize { get; set; }

        /// <summary>
        ///     The maximum number of documents within the collection
        /// </summary>
        public long MaxDocuments { get; set; }

        /// <summary>
        ///     Whether or not the collection is capped
        /// </summary>
        public bool Capped { get; set; } = false;

        /// <summary>
        ///     Whether Po2 sizes should be used
        /// </summary>
        public bool PowerOf2Sizes { get; set; } = false;

        /// <summary>
        ///     The <see cref="DocumentValidationAction" /> for the collection
        /// </summary>
        public DocumentValidationAction ValidationAction { get; set; } = DocumentValidationAction.Warn;

        /// <summary>
        ///     The <see cref="DocumentValidationLevel" /> for the collection
        /// </summary>
        public DocumentValidationLevel ValidationLevel { get; set; } = DocumentValidationLevel.Off;
    }
}