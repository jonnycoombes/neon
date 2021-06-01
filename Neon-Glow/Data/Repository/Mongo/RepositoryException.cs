/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
using System;

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    /// Exception type for <see cref="IRepository{T}"/> implementations 
    /// </summary>
    public class RepositoryException : Exception
    {
        public RepositoryException(string? message) : base(message)
        {
        }

        public RepositoryException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}