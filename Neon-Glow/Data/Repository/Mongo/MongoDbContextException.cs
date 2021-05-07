#region

using System;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     A general exception class which may be thrown during various failure modes within the <see cref="MongoDbContext" />
    ///     class logic
    /// </summary>
    public class MongoDbContextException : Exception
    {
        /// <summary>
        ///     Overridden constructor, just calls base
        /// </summary>
        /// <param name="message">The message for the exception</param>
        public MongoDbContextException(string? message) : base(message)
        {
        }

        /// <summary>
        ///     Overridden constructor, just calls base
        /// </summary>
        /// <param name="message">An optional message for the exception</param>
        /// <param name="innerException">An optional nested exception</param>
        public MongoDbContextException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}