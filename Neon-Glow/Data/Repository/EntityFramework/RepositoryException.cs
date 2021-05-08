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
    public class RepositoryException : Exception
    {
        public RepositoryException()
        {
        }

        protected RepositoryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public RepositoryException(string? message) : base(message)
        {
        }

        public RepositoryException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}