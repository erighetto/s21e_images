using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace S21eimagescollect.Model
{


    public partial class Tblarticoli
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }

        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("database", NullValueHandling = NullValueHandling.Ignore)]
        public string Database { get; set; }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public Datum[] Data { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty("CodArt")]
        public string CodArt { get; set; }

        [JsonProperty("CodEan")]
        public string CodEan { get; set; }

        [JsonProperty("DescArticolo")]
        public string DescArticolo { get; set; }
    }
}
