/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
using Xunit;
using Xunit.Abstractions;

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    /// Tests covering basic collection functionality such as binding, checking CRUD operations etc...
    /// </summary>
    public class CollectionTests : TestBase, IClassFixture<Fixtures>
    {
        /// <summary>
        /// The fixtures to be used by this test
        /// </summary>
        protected Fixtures Fixtures { get; set; }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="output"></param>
        /// <param name="fixtures"></param>
        public CollectionTests(ITestOutputHelper output, Fixtures fixtures) : base(output)
        {
            Fixtures = fixtures;
        }
        
        
    }
}