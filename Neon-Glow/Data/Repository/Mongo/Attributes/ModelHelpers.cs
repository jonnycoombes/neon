/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using System.Collections.Generic;
using JCS.Neon.Glow.Statics;
using JCS.Neon.Glow.Statics.Reflection;
using MongoDB.Driver;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    public static class ModelHelpers
    {
        /// <summary>
        ///     Static logger for this class
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(ModelHelpers));

        /// <summary>
        ///     Builds a list of <see cref="CreateIndexModel{TDocument}" /> instances for a given type <typeparamref name="T" />, based on any
        ///     found instances of the <see cref="Index" /> and <see cref="IndexField" /> attributes
        /// </summary>
        /// <param name="namingConvention">A function which can take a list of field names, and then create a name for an index</param>
        /// <typeparam name="T">The type to inspect</typeparam>
        /// <returns>A (potentially empty) list of <see cref="CreateIndexModel{TDocument}" /> instances</returns>
        public static List<CreateIndexModel<T>> BuildIndexModelsFromAttributes<T>(Func<string[], string> namingConvention)
        {
            Logging.MethodCall(_log);
            var entityType = typeof(T);
            var models = new List<CreateIndexModel<T>>();
            if (Attributes.GetCustomAttributes<Index>(AttributeTargets.Class, entityType).IsSome(out var indexAttributes))
            {
                foreach (var indexAttribute in indexAttributes)
                {
                    Logging.Debug(_log, $"Located an index attribute on type \"{entityType.Name}\"");
                    var optionsBuilder = new CreateIndexOptionsBuilder<T>()
                        .Name(indexAttribute.Name ?? namingConvention(indexAttribute.Fields))
                        .Sparse(indexAttribute.Sparse)
                        .Unique(indexAttribute.Unique)
                        .Background(indexAttribute.Background);
                    var keyDefinitions = BuildKeyDefinitionsFromAttributes<T>(indexAttribute);
                    models.Add(new CreateIndexModel<T>(keyDefinitions, optionsBuilder.Build()));
                }
            }

            return models;
        }

        /// <summary>
        ///     Given a specific instance of <see cref="Index" /> attribute, will combine the fields property with any associated
        ///     <see cref="Indexfield" />
        ///     instances on the attributed class in order to derive an instance of <see cref="IndexKeysDefinition{TDocument}" />
        ///     for the class
        /// </summary>
        /// <param name="indexAttribute">The instance of <see cref="Index" /> to base the key definitions on</param>
        /// <typeparam name="T">The attributed class type</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException">This will be thrown if the index attribute has a dodgy fields definition</exception>
        public static IndexKeysDefinition<T>? BuildKeyDefinitionsFromAttributes<T>(Index indexAttribute)
        {
            Logging.MethodCall(_log);
            var entityType = typeof(T);

            var builder = Builders<T>.IndexKeys;
            List<IndexKeysDefinition<T>> definitions = new();
            if (indexAttribute.Fields.Length == 0)
            {
                Logging.Warning(_log, "Index attribute found with no field definitions");
                throw new ArgumentException("The fields attribute for associated index attribute is empty");
            }

            foreach (var field in indexAttribute.Fields)
            {
                if (Attributes.GetCustomAttribute<IndexField>(AttributeTargets.Property, entityType, field).IsSome(out var fieldAttribute))
                {
                    if (fieldAttribute.IsText)
                    {
                        definitions.Add(builder.Text(field));
                    }
                    else
                    {
                        if (fieldAttribute.Ascending)
                        {
                            definitions.Add(builder.Ascending(field));
                        }
                        else
                        {
                            definitions.Add(builder.Descending(field));
                        }
                    }
                }
                else
                {
                    definitions.Add(builder.Ascending(field));
                }
            }

            return builder.Combine(definitions.ToArray());
        }

        /// <summary>
        ///     Creates an instance of <see cref="CreateCollectionOptionsBuilder" /> based on a given type
        ///     <typeparamref name="T" />. This method looks for an instance of the <see cref="Collection" /> attribute, and if found
        ///     will pre-configure the builder with values extracted from it
        /// </summary>
        /// <typeparam name="T">The type to inspect for attributes</typeparam>
        /// <returns>A new instance of <see cref="CreateCollectionOptionsBuilder" /></returns>
        public static CreateCollectionOptionsBuilder CollectionOptionsBuilderFromAttributes<T>()
        {
            Logging.MethodCall(_log);
            var builder = new CreateCollectionOptionsBuilder();
            if (Attributes.GetCustomAttribute<Collection>(AttributeTargets.Class, typeof(T)).IsSome(out var collectionAttribute))
            {
                Logging.Debug(_log, "Found a collection attribute, using this to set options");
                builder
                    .ValidationAction(collectionAttribute.ValidationAction)
                    .ValidationLevel(collectionAttribute.ValidationLevel)
                    .Capped(collectionAttribute.Capped)
                    .MaxDocuments(collectionAttribute.MaxDocuments)
                    .MaxSize(collectionAttribute.MaxSize)
                    .UsePowerOf2Sizes(collectionAttribute.PowerOf2Sizes);
            }

            return builder;
        }
    }
}