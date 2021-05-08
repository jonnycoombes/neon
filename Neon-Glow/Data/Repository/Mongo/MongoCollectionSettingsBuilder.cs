/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using JCS.Neon.Glow.Types;
using MongoDB.Driver;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Builder class for new instancefs of <see cref="MongoCollectionSettings" />
    /// </summary>
    public class MongoCollectionSettingsBuilder : IBuilder<MongoCollectionSettings>
    {
        /// <summary>
        ///     The acutal settings
        /// </summary>
        private readonly MongoCollectionSettings _settings = new();


        /// <summary>
        ///     Returns the built <see cref="MongoCollectionSettings" />
        /// </summary>
        /// <returns>A built instance of <see cref="MongoCollectionSettings" /></returns>
        public MongoCollectionSettings Build()
        {
            return _settings;
        }
    }
}