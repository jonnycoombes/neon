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

namespace JCS.Neon.Glow.Data.Repository.EFCore
{
    public class AsyncRepositoryException : Exception
    {
        public AsyncRepositoryException()
        {
        }

        protected AsyncRepositoryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AsyncRepositoryException(string? message) : base(message)
        {
        }

        public AsyncRepositoryException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}