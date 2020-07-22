using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace GoldTracker
{
    public class Character
    {
        public static string[] FromJson(string json) => JsonConvert.DeserializeObject<string[]>(json, GoldTracker.Converter.Settings);
    }

    public static class Serialize4
    {
        public static string ToJson(this string[] self) => JsonConvert.SerializeObject(self, GoldTracker.Converter.Settings);
    }

    internal static class Converter4
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