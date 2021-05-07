/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using JCS.Neon.Glow.Types;

#endregion

namespace JCS.Neon.Glow.Statics.Crypto
{
    /// <summary>
    ///     Class used to specify options for certificate generation
    /// </summary>
    public class X509CertificateGenerationOptions
    {
    }

    /// <summary>
    ///     Builder class for <see cref="X509CertificateGenerationOptions" />
    /// </summary>
    public class X509CertificateGenerationOptionsBuilder : IBuilder<X509CertificateGenerationOptions>
    {
        /// <summary>
        ///     Builds a new <see cref="X509CertificateGenerationOptions" /> instance
        /// </summary>
        /// <returns>A new <see cref="X509CertificateGenerationOptions" /> instance</returns>
        /// <exception cref="NotImplementedException"></exception>
        public X509CertificateGenerationOptions Build()
        {
            throw new NotImplementedException();
        }
    }
}