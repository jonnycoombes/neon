/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using System.Runtime.Serialization;

#endregion

namespace JCS.Neon.Glow.Data.Repository.EntityFramework
{
    /// <summary>
    ///     Exception type specific to <see cref="IRepository{K,V}" /> aware contexts
    /// </summary>
    public class DbContextException : Exception
    {
        public DbContextException()
        {
        }

        protected DbContextException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DbContextException(string? message) : base(message)
        {
        }

        public DbContextException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}