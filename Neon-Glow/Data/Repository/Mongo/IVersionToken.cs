/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    /// Interface to be implemented by all concurrency token types used within <see cref="RepositoryObject{T}"/>
    /// </summary>
    public interface IVersionToken<T>
    {
        /// <summary>
        /// The current token value
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Increment the token and return the result
        /// </summary>
        /// <returns>A new value of type <typeparamref name="T"/></returns>
        public T Increment();

    }
}