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
    ///     Attribute that allows refinement of field contributions to indexes including the field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
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
    }
}