/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using JCS.Neon.Glow.Types.Extensions;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace JCS.Neon.Glow.Test.Types.Extensions
{
    /// <summary>
    ///     Tests for <see cref="string" /> extensions
    /// </summary>
    public class StringTests : TestBase
    {
        public StringTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory(DisplayName = "Can convert strings to camel case")]
        [Trait("Category", "Extensions")]
        [InlineData("StringToBeCamelCased", "stringToBeCamelCased")]
        [InlineData("thisIsAstring", "thisIsAstring")]
        [InlineData("CollectionNameToBeConverted", "collectionNameToBeConverted")]
        public void CheckCamelCaseConversion(string source, string expected)
        {
            Assert.True(expected.Equals(source.ToCamelCase()));
        }
    }
}