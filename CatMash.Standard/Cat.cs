using System;
using Newtonsoft.Json;

namespace CatMash
{
    public class Cat
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("rate")]
        public UInt16 Rate { get; set; }
        [JsonProperty("nbMash")]
        public UInt16 NbMash { get; set; }
    }
}
