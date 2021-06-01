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
    /// Tests for <see cref="IRepository{T}"/> functionality
    /// </summary>
    public class RepositoryTests : TestBase, IClassFixture<Fixtures>
    {
        /// <summary>
        ///     The fixtures to be used by this test
        /// </summary>
        protected Fixtures Fixtures { get; set; }
        
        public RepositoryTests(ITestOutputHelper output,Fixtures fixtures) : base(output)
        {
            Fixtures = fixtures;
        }
        
        
    }
}