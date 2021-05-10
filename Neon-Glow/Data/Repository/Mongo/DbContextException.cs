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
    ///     A general exception class which may be thrown during various failure modes within the <see cref="DbContext" />
    ///     class logic
    /// </summary>
    public class DbContextException : Exception
    {
        /// <summary>
        ///     Overridden constructor, just calls base
        /// </summary>
        /// <param name="message">The message for the exception</param>
        public DbContextException(string? message) : base(message)
        {
        }

        /// <summary>
        ///     Overridden constructor, just calls base
        /// </summary>
        /// <param name="message">An optional message for the exception</param>
        /// <param name="innerException">An optional nested exception</param>
        public DbContextException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}