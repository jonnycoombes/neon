/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

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
        /// Test private field
        /// </summary>
        private string _privateField;

        /// <summary>
        /// Test public field
        /// </summary>
        public string _publicField;
        
        /// <summary>
        /// Test private property
        /// </summary>
        private string PrivateProperty { get; set; }

        /// <summary>
        /// Test public property
        /// </summary>
        public string PublicProperty { get; set; }
    }

    [Trait("Category", "TypeReflection")]
    public class TypeReflectionTests : TestBase
    {
        public TypeReflectionTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact(DisplayName = "Can check for public properties")]
        public void CheckPublicProperties()
        {
            var truth = TypeReflection.GetProperty(typeof(TestType), "PublicProperty");
            var falsity = TypeReflection.GetProperty(typeof(TestType), "Cheese");
            Assert.True(truth.IsSome());
            Assert.False(falsity.IsSome());
            Assert.True(TypeReflection.HasProperty(typeof(TestType), "PublicProperty"));
            Assert.False(TypeReflection.HasProperty(typeof(TestType), "Cheese"));
        }

        [Fact(DisplayName = "Can check for public fields")]
        public void CheckPublicFields()
        {
            var truth = TypeReflection.GetField(typeof(TestType), "_publicField");
            var falsity = TypeReflection.GetField(typeof(TestType), "Cheese");
            Assert.True(truth.IsSome());
            Assert.False(falsity.IsSome());
            Assert.True(TypeReflection.HasField(typeof(TestType),"_publicField"));
            Assert.False(TypeReflection.HasField(typeof(TestType),"Cheese"));
        }
    }
}