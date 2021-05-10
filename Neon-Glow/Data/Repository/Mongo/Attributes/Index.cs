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
    /// An attribute which allows for the specification of indexes to be created against a specific collection document type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class Index : Attribute
    {
        /// <summary>
        ///     The name for the index
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// A list of the fields to be included within the index
        /// </summary>
        public string[]? Fields { get; set; }

    }
}