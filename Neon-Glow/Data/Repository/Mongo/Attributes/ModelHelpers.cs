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

namespace JCS.Neon.Glow.Data.Repository.Mongo.Attributes
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
            if (Statics.Reflection.Attributes.GetCustomAttributes<Index>(AttributeTargets.Class, entityType).IsSome(out var indexAttributes))
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
                if (Statics.Reflection.Attributes.GetCustomAttribute<IndexField>(AttributeTargets.Property, entityType, field).IsSome(out var fieldAttribute))
                {
                    if (fieldAttribute.IsText)
                    {
                        definitions.Add(builder.Text(field));
                    }
                    else
                    {
                        if (fieldAttribute.IsWildcard)
                        {
                            definitions.Add(builder.Wildcard(field));
                        }
                        else
                        {
                            definitions.Add(fieldAttribute.Ascending ? builder.Ascending(field) : builder.Descending(field));
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
            if (Statics.Reflection.Attributes.GetCustomAttribute<Collection>(AttributeTargets.Class, typeof(T)).IsSome(out var collectionAttribute))
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

        /// <summary>
        ///     Given a specific collection type, function which determines the name of the collection.  The name is either based
        ///     on an explicit value entered via attribution (using the <see cref="Attributes.Collection" />) custom attribute or
        ///     via a currently configured naming convention function
        /// </summary>
        /// <param name="namingConvention">The function used to derive the collection name if no valid attribution if present</param>
        /// <typeparam name="T">The type of the entities to be contained in the collection</typeparam>
        /// <returns>A name for a given collection</returns>
        public static string DeriveCollectionName<T>(Func<string, string> namingConvention)
        {
            Logging.MethodCall(_log);
            var t = typeof(T);
            var option = Statics.Reflection.Attributes.GetCustomAttribute<Collection>(AttributeTargets.Class, t);
            if (option.IsSome(out var attribute))
            {
                return attribute.Name ?? namingConvention(t.Name);
            }

            return namingConvention(t.Name);
        }
    }
}