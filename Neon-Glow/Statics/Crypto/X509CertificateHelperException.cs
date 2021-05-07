/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;

#endregion

namespace JCS.Neon.Glow.Statics.Crypto
{
    public class X509CertificateException : Exception
    {
        public X509CertificateException(string? message) : base(message)
        {
        }

        public X509CertificateException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}