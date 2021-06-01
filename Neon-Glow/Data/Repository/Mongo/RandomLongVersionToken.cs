/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
using System;
using JCS.Neon.Glow.Statics.Crypto;

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    /// <see cref="IVersionToken{T}"/> based on a changing random long value
    /// </summary>
    public class RandomLongVersionToken : IVersionToken<long>
    {
        /// <summary>
        /// Guard/synchronisation object
        /// </summary>
        private readonly object _guard = new();

        /// <summary>
        /// The backing value
        /// </summary>
        private long _value = Math.Abs(Rng.RandomLong());

        /// <inheritdoc cref="IVersionToken{T}.Value"/> 
        public long Value
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

        /// <inheritdoc cref="IVersionToken{T}.Increment"/>
        public long Increment()
        {
            lock (_guard)
            {
                _value = Math.Abs(Rng.RandomLong());
                return _value;
            }
        }
    }
}