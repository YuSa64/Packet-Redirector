using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PacketRedirector.Models
{
    public class LocalizationEntry
    {
        [JsonPropertyName("lang")]
        public string Lang { get; set; } = "";

        [JsonPropertyName("text")]
        public Dictionary<string, string> Text { get; set; } = new();

        public override string ToString() => Lang;
    }
}