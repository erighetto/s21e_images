using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace S21eimagesrefine
{
    public partial class Proxies
    {
        [JsonProperty("data")]
        public List<Datum> Data { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("port")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Port { get; set; }

        [JsonProperty("time")]
        public double Time { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            value = value.Replace(",", "").Replace(".", "");
            if (Int64.TryParse(value, out long l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}