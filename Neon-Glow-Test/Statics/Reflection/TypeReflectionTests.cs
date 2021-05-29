/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using JCS.Neon.Glow.Statics.Reflection;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace JCS.Neon.Glow.Test.Statics.Reflection
{
    /// <summary>
    ///     Type used during tests
    /// </summary>
    public class TestType
    {
        /// <summary>
        ///     Test private field
        /// </summary>
        private string _privateField;

        /// <summary>
        ///     Test protected field
        /// </summary>
        protected string _protectedField;

        /// <summary>
        ///     Test public field
        /// </summary>
        public string _publicField;

        /// <summary>
        ///     Test private property
        /// </summary>
        private string PrivateProperty { get; set; }

        /// <summary>
        ///     Test public property
        /// </summary>
        public string PublicProperty { get; set; }

        /// <summary>
        ///     Test protected property
        /// </summary>
        protected string ProtectedProperty { get; set; }

        /// <summary>
        ///     Public event to test purposes
        /// </summary>
        public event EventHandler PublicEvent;

        public void PublicMethod()
        {
        }

        private void PrivateMethod()
        {
        }

        protected void ProtectedMethod()
        {
        }
    }

    [Trait("Category", "TypeReflection")]
    public class TypeReflectionTests : TestBase
    {
        public TypeReflectionTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact(DisplayName = "Can check for  properties")]
        public void CheckProperties()
        {
            var t = typeof(TestType);
            Assert.True(TypeReflection.GetProperty(t, "PublicProperty").IsSome());
            Assert.True(TypeReflection.GetProperty(t, "PrivateProperty").IsSome());
            Assert.True(TypeReflection.GetProperty(t, "ProtectedProperty").IsSome());
        }

        [Fact(DisplayName = "Can check for fields")]
        public void CheckFields()
        {
            var t = typeof(TestType);
            Assert.True(TypeReflection.GetField(t, "_publicField").IsSome());
            Assert.True(TypeReflection.GetField(t, "_privateField").IsSome());
            Assert.True(TypeReflection.GetField(t, "_protectedField").IsSome());
        }

        [Fact(DisplayName = "Can check for public methods")]
        public void CheckMethods()
        {
            var t = typeof(TestType);
            Assert.True(TypeReflection.GetMethod(t, "PublicMethod").IsSome());
            Assert.True(TypeReflection.GetMethod(t, "PrivateMethod").IsSome());
            Assert.True(TypeReflection.GetMethod(t, "ProtectedMethod").IsSome());
        }

        [Fact(DisplayName = "Can check for events")]
        public void CheckEvents()
        {
            var t = typeof(TestType);
            Assert.True(TypeReflection.GetEvent(t, "PublicEvent").IsSome());
        }
    }
}