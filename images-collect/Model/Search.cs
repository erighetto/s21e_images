using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace S21eimagescollect
{

    public partial class Search
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("url")]
        public Url Url { get; set; }

        [JsonProperty("queries")]
        public Queries Queries { get; set; }

        [JsonProperty("context")]
        public Context Context { get; set; }

        [JsonProperty("searchInformation")]
        public SearchInformation SearchInformation { get; set; }

        [JsonProperty("items")]
        public Item[] Items { get; set; }
    }

    public partial class Context
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("htmlTitle")]
        public string HtmlTitle { get; set; }

        [JsonProperty("link")]
        public Uri Link { get; set; }

        [JsonProperty("displayLink")]
        public string DisplayLink { get; set; }

        [JsonProperty("snippet")]
        public string Snippet { get; set; }

        [JsonProperty("htmlSnippet")]
        public string HtmlSnippet { get; set; }

        [JsonProperty("mime")]
        public string Mime { get; set; }

        [JsonProperty("fileFormat")]
        public string FileFormat { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }
    }

    public partial class Image
    {
        [JsonProperty("contextLink")]
        public Uri ContextLink { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("byteSize")]
        public long ByteSize { get; set; }

        [JsonProperty("thumbnailLink")]
        public Uri ThumbnailLink { get; set; }

        [JsonProperty("thumbnailHeight")]
        public long ThumbnailHeight { get; set; }

        [JsonProperty("thumbnailWidth")]
        public long ThumbnailWidth { get; set; }
    }

    public partial class Queries
    {
        [JsonProperty("request")]
        public Request[] Request { get; set; }
    }

    public partial class Request
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("totalResults")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long TotalResults { get; set; }

        [JsonProperty("searchTerms")]
        public string SearchTerms { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("startIndex")]
        public long StartIndex { get; set; }

        [JsonProperty("inputEncoding")]
        public string InputEncoding { get; set; }

        [JsonProperty("outputEncoding")]
        public string OutputEncoding { get; set; }

        [JsonProperty("safe")]
        public string Safe { get; set; }

        [JsonProperty("cx")]
        public string Cx { get; set; }

        [JsonProperty("searchType")]
        public string SearchType { get; set; }

        [JsonProperty("imgSize")]
        public string ImgSize { get; set; }
    }

    public partial class SearchInformation
    {
        [JsonProperty("searchTime")]
        public double SearchTime { get; set; }

        [JsonProperty("formattedSearchTime")]
        public string FormattedSearchTime { get; set; }

        [JsonProperty("totalResults")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long TotalResults { get; set; }

        [JsonProperty("formattedTotalResults")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long FormattedTotalResults { get; set; }
    }

    public partial class Url
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("template")]
        public string Template { get; set; }
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
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
