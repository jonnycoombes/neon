/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo.Attributes
{
    /// <summary>
    ///     Attribute that allows refinement of field contributions to indexes including the field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IndexField : Attribute
    {
        /// <summary>
        ///     Whether or not the field should have an ascending sort order
        /// </summary>
        public bool Ascending { get; set; } = true;

        /// <summary>
        ///     Whether or not the field should be treated as a text field.  Note that each collection may only have *one* text index
        /// </summary>
        public bool IsText { get; set; } = false;

        /// <summary>
        /// Whether or not a wildcard index key should be applied 
        /// </summary>
        public bool IsWildcard { get; set; } = false;
    }
}