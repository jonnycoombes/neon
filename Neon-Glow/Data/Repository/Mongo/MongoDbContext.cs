#region

using System;
using JCS.Neon.Glow.Statics;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Abstract base class for Mongo DB contexts.  Takes care of all the clever stuff relating to lifecycle, session
    ///     management etc...Derived classes can add automatic support for repository-style access to collections and related
    ///     functionality
    ///     through generic virtual methods and properties.
    /// </summary>
    public abstract class MongoDbContext
    {
        /// <summary>
        ///     Static logger
        /// </summary>
        private static ILogger _log = Log.ForContext<MongoDbContext>();

        /// <summary>
        ///     Constructor that takes an instance of <see cref="MongoDbContextOptions" />
        /// </summary>
        /// <param name="options">An instance of <see cref="MongoDbContextOptions" /></param>
        protected MongoDbContext(MongoDbContextOptions options)
        {
            Logging.MethodCall(_log);
            _options = options;
        }

        /// <summary>
        ///     Constructor that takes an <see cref="Action" /> that can take a <see cref="MongoDbContextOptionsBuilder" />
        ///     instance and configure the context options through the builder
        /// </summary>
        /// <param name="configureAction"></param>
        protected MongoDbContext(Action<MongoDbContextOptionsBuilder> configureAction)
        {
            Logging.MethodCall(_log);
            var builder = new MongoDbContextOptionsBuilder();
            configureAction(builder);
            _options = builder.Build();
        }

        /// <summary>
        ///     The currently configured options for this context
        /// </summary>
        protected MongoDbContextOptions _options { get; set; }
    }

    #region Exceptions

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

    #endregion
}