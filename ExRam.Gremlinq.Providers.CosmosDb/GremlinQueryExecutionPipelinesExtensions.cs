﻿using System;
using System.Collections.Generic;
using Gremlin.Net.Structure.IO.GraphSON;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NullGuard;

namespace ExRam.Gremlinq.Core
{
    public static class GremlinQueryExecutionPipelinesExtensions
    {
        private sealed class TimespanConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(TimeSpan);
            }

            public override object ReadJson(JsonReader reader, Type objectType, [AllowNull] object existingValue, JsonSerializer serializer)
            {
                return TimeSpan.FromMilliseconds(serializer.Deserialize<long>(reader));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotSupportedException();
            }

            public override bool CanRead => true;
            public override bool CanWrite => true;
        }

        private sealed class TimeSpanSerializer : IGraphSONSerializer, IGraphSONDeserializer
        {
            public Dictionary<string, dynamic> Dictify(dynamic objectData, GraphSONWriter writer)
            {
                TimeSpan value = objectData;
                return GraphSONUtil.ToTypedValue("Double", value.TotalMilliseconds);
            }

            public dynamic Objectify(JToken graphsonObject, GraphSONReader reader)
            {
                var duration = graphsonObject.ToObject<double>();
                return TimeSpan.FromMilliseconds(duration);
            }
        }

        public static IGremlinQueryExecutionPipeline UseCosmosDbDeserializer(this IGremlinQueryExecutionPipeline pipeline)
        {
            return pipeline
                .UseDeserializer(GremlinQueryExecutionResultDeserializer.GraphsonWithJsonConverters(new TimespanConverter()));
        }

        public static IGremlinQueryExecutionPipeline UseCosmosDbSerializer(this IGremlinQueryExecutionPipeline pipeline)
        {
            return pipeline
                .ConfigureSerializer(serializer => serializer
                    .UseCosmosDbWorkarounds()
                    .ToGroovy());
        }

        public static IGremlinQueryExecutionPipeline UseCosmosDbExecutor(this IGremlinQueryExecutionPipeline pipeline, Uri uri, string database, string graphName, string authKey, ILogger logger)
        {
            return pipeline
                .UseWebSocketExecutor(
                    uri,
                    $"/dbs/{database}/colls/{graphName}",
                    authKey,
                    "g",
                    GraphsonVersion.V2,
                    new Dictionary<Type, IGraphSONSerializer>
                    {
                        {typeof(TimeSpan), new TimeSpanSerializer()}
                    },
                    default,
                    logger);
        }
    }
}
