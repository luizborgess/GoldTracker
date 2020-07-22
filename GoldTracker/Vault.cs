using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace GoldTracker
{
    public partial class Vault
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("category")]
        public long Category { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("binding")]
        public string Binding { get; set; }
    }

    public partial class Vault
    {
        public static Vault[] FromJson(string json) => JsonConvert.DeserializeObject<Vault[]>(json, GoldTracker.Converter3.Settings);
    }

    public static class Serialize3
    {
        public static string ToJson(this Vault[] self) => JsonConvert.SerializeObject(self, GoldTracker.Converter3.Settings);
    }

    internal static class Converter3
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}