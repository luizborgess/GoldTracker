using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace GoldTracker
{
    public partial class Vendor
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("rarity")]
        public string Rarity { get; set; }

        [JsonProperty("vendor_value")]
        public long VendorValue { get; set; }

        [JsonProperty("game_types")]
        public string[] GameTypes { get; set; }

        [JsonProperty("flags")]
        public string[] Flags { get; set; }

        [JsonProperty("restrictions")]
        public object[] Restrictions { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("chat_link")]
        public string ChatLink { get; set; }

        [JsonProperty("icon")]
        public Uri Icon { get; set; }

        [JsonProperty("details")]
        public Details Details { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("default_skin", NullValueHandling = NullValueHandling.Ignore)]
        public long? DefaultSkin { get; set; }
    }

    public partial class Details
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("duration_ms", NullValueHandling = NullValueHandling.Ignore)]
        public long? DurationMs { get; set; }

        [JsonProperty("apply_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? ApplyCount { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Icon { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("damage_type", NullValueHandling = NullValueHandling.Ignore)]
        public string DamageType { get; set; }

        [JsonProperty("min_power", NullValueHandling = NullValueHandling.Ignore)]
        public long? MinPower { get; set; }

        [JsonProperty("max_power", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaxPower { get; set; }

        [JsonProperty("defense", NullValueHandling = NullValueHandling.Ignore)]
        public long? Defense { get; set; }

        [JsonProperty("infusion_slots", NullValueHandling = NullValueHandling.Ignore)]
        public object[] InfusionSlots { get; set; }

        [JsonProperty("attribute_adjustment", NullValueHandling = NullValueHandling.Ignore)]
        public double? AttributeAdjustment { get; set; }

        [JsonProperty("infix_upgrade", NullValueHandling = NullValueHandling.Ignore)]
        public InfixUpgrade InfixUpgrade { get; set; }

        [JsonProperty("suffix_item_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? SuffixItemId { get; set; }

        [JsonProperty("secondary_suffix_item_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SecondarySuffixItemId { get; set; }
    }

    public partial class InfixUpgrade
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("attributes")]
        public Attribute[] Attributes { get; set; }
    }

    public partial class Attribute
    {
        [JsonProperty("attribute")]
        public string AttributeAttribute { get; set; }

        [JsonProperty("modifier")]
        public long Modifier { get; set; }
    }

    public partial class Vendor
    {
        public static Vendor[] FromJson(string json) => JsonConvert.DeserializeObject<Vendor[]>(json, GoldTracker.Converter5.Settings);
    }

    public static class Serialize5
    {
        public static string ToJson(this Vendor[] self) => JsonConvert.SerializeObject(self, GoldTracker.Converter5.Settings);
    }

    internal static class Converter5
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