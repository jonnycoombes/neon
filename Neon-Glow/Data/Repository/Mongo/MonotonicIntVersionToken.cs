/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    /// Concurrency token based on a monotonically increasing integer value
    /// </summary>
    public class MonotonicIntVersionToken : IVersionToken<int>
    {
        /// <summary>
        /// A lock object
        /// </summary>
        private readonly object _guard = new();
        
        /// <summary>
        /// The backing field
        /// </summary>
        private int _value = 0;

        ///<inheritdoc cref="IVersionToken{T}.Value"/> 
        public int Value
        {
            get
            {
                lock (_guard)
                {
                    return _value;
                }
            }
            set
            {
                lock (_guard)
                {
                    _value = value;
                }
            }
        }
        
        ///<inheritdoc cref="IVersionToken{T}.Increment"/> 
        public int Increment()
        {
            lock (_guard)
            {
                _value++;
                return _value;
            }
        }
    }
}