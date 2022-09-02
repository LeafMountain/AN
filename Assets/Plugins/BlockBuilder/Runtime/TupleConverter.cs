
using System;
using Newtonsoft.Json;

namespace BlockBuilder
{
    public class TupleConverter<U, V> : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Tuple<U, V>) == objectType;
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == Newtonsoft.Json.JsonToken.Null) return null;
            var jObject = Newtonsoft.Json.Linq.JObject.Load(reader);
            var target = (jObject["m_Item1"].ToObject<U>(), jObject["m_Item2"].ToObject<V>());

            return target;
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}