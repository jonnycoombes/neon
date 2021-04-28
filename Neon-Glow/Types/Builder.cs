/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
namespace JCS.Neon.Glow.Types
{
    /// <summary>
    /// Simple interface which should be defined for classes implementing the builder pattern
    /// </summary>
    public interface Builder<out T>
    {
        /// <summary>
        /// Builds the target type
        /// </summary>
        /// <returns>An instance of the type which this builder knows how to build</returns>
        T Build();
    }
}