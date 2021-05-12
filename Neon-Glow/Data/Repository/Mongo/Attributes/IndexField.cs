#region

using System;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Attribute that allows refinement of field contributions to indexes including the field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class IndexField : Attribute
    {
        /// <summary>
        ///     Whether or not the field should have an ascending sort order
        /// </summary>
        public bool Ascending { get; set; } = true;

    }
}