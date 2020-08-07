using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace S21eimagesscrape.Model
{
    public class CatalogProductEntity
    {

        [JsonProperty("CodArt")]
        public string CodArt { get; set; }

        [JsonProperty("DescArticolo")]
        public string DescArticolo { get; set; }

        [JsonProperty("CodEan")]
        public string CodEan { get; set; }
    }

    public class CatalogProductEntities
    {

        [JsonProperty("catalog_product_entity")]
        public IList<CatalogProductEntity> CatalogProductEntity { get; set; }
    }
}
