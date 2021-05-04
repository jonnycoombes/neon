#region

using System;
using System.Runtime.Serialization;

#endregion

namespace JCS.Neon.Glow.Data.Repository.EFCore
{
    /// <summary>
    ///     Exception type specific to <see cref="IAsyncRepository{K,V}" /> aware contexts
    /// </summary>
    public class AsyncRepositoryAwareDbContextException : Exception
    {
        public AsyncRepositoryAwareDbContextException()
        {
        }

        protected AsyncRepositoryAwareDbContextException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AsyncRepositoryAwareDbContextException(string? message) : base(message)
        {
        }

        public AsyncRepositoryAwareDbContextException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}