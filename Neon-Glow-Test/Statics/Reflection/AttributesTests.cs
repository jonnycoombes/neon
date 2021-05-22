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
    ///     Test custom class attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TestClassAttribute : Attribute
    {
        public string Value { get; set; }
    }

    /// <summary>
    ///     Test property attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TestPropertyAttribute : Attribute
    {
        public string Value { get; set; }
    }

    /// <summary>
    ///     Test method attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestMethodAttribute : Attribute
    {
        public string Value { get; set; }
    }

    /// <summary>
    ///     Test field attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TestFieldAttribute : Attribute
    {
        public string Value { get; set; }
    }

    /// <summary>
    ///     Fixture for testing attribute inspection
    /// </summary>
    [TestClass(Value = "Value 1")]
    [TestClass(Value = "Value 2")]
    [TestClass(Value = "Value 3")]
    internal class AttributedClass
    {
        [TestField(Value = "Value 1")]
        [TestField(Value = "Value 2")]
        [TestField(Value = "Value 3")]
        private int _testField;

        public AttributedClass(int value)
        {
            _testField = value;
        }

        [TestProperty(Value = "Value 1")]
        [TestProperty(Value = "Value 2")]
        [TestProperty(Value = "Value 3")]
        public string TestProperty { get; set; }

        [TestMethod(Value = "Value 1")]
        [TestMethod(Value = "Value 2")]
        [TestMethod(Value = "Value 3")]
        public void TestMethod()
        {
        }
    }


    [Trait("Category", "Attributes")]
    public class AttributesTests : TestBase
    {
        /// <summary>
        ///     Default constructor - just passes to base
        /// </summary>
        /// <param name="output"></param>
        public AttributesTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact(DisplayName = "Can locate a singular class attribute")]
        [Trait("Category", "Attributes")]
        public void LocateClassAttribute()
        {
            var attribute = Attributes.GetCustomAttribute<TestClassAttribute>(AttributeTargets.Class, typeof(AttributedClass));
            Assert.True(attribute.IsSome());
        }

        [Fact(DisplayName = "Can locate a singular method attribute")]
        [Trait("Category", "Attributes")]
        public void LocateMethodAttribute()
        {
            var attribute = Attributes.GetCustomAttribute<TestMethodAttribute>(AttributeTargets.Method, typeof(AttributedClass), nameof
                (AttributedClass.TestMethod));
            Assert.True(attribute.IsSome());
        }

        [Fact(DisplayName = "Can locate a singular property attribute")]
        [Trait("Category", "Attributes")]
        public void LocatePropertyAttribute()
        {
            var attribute = Attributes.GetCustomAttribute<TestPropertyAttribute>(AttributeTargets.Property, typeof(AttributedClass), nameof
            (AttributedClass.TestProperty));
            Assert.True(attribute.IsSome());
        }

        [Fact(DisplayName = "Can locate a singular field attribute")]
        [Trait("Category", "Attributes")]
        public void LocateFieldAttribute()
        {
            var attribute = Attributes.GetCustomAttribute<TestFieldAttribute>(AttributeTargets.Field, typeof(AttributedClass), 
            "_testField");
            Assert.True(attribute.IsSome());
        }
        
        [Fact(DisplayName = "Can locate a multiple field attributes")]
        [Trait("Category", "Attributes")]
        public void LocateFieldAttributes()
        {
            var result = Attributes.GetCustomAttributes<TestFieldAttribute>(AttributeTargets.Field, typeof(AttributedClass), 
            "_testField");
            Assert.True(result.IsSome());
            if (result.IsSome(out var attributes))
            {
                Assert.True(attributes.Length == 3);
            }
            else
            {
                Assert.True(false);
            }
        }
        [Fact(DisplayName = "Can locate a multiple class attributes")]
        [Trait("Category", "Attributes")]
        public void LocateClassAttributes()
        {
            var result = Attributes.GetCustomAttributes<TestClassAttribute>(AttributeTargets.Class, typeof(AttributedClass));
            Assert.True(result.IsSome());
            if (result.IsSome(out var attributes))
            {
                Assert.True(attributes.Length == 3);
            }
            else
            {
                Assert.True(false);
            }
        }
        
        [Fact(DisplayName = "Can locate a multiple property attributes")]
        [Trait("Category", "Attributes")]
        public void LocatePropertyAttributes()
        {
            var result = Attributes.GetCustomAttributes<TestPropertyAttribute>(AttributeTargets.Property, typeof(AttributedClass), 
            nameof(AttributedClass.TestProperty));
            Assert.True(result.IsSome());
            if (result.IsSome(out var attributes))
            {
                Assert.True(attributes.Length == 3);
            }
            else
            {
                Assert.True(false);
            }
        }
        [Fact(DisplayName = "Can locate a multiple method attributes")]
        [Trait("Category", "Attributes")]
        public void LocateMethodAttributes()
        {
            var result = Attributes.GetCustomAttributes<TestMethodAttribute>(AttributeTargets.Method, typeof(AttributedClass), 
            nameof(AttributedClass.TestMethod));
            Assert.True(result.IsSome());
            if (result.IsSome(out var attributes))
            {
                Assert.True(attributes.Length == 3);
            }
            else
            {
                Assert.True(false);
            }
        }
    }
}