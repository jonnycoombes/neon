#region

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
    }
}