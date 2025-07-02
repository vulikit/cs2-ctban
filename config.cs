using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace cs2_ctban
{
    public class s03 : BasePluginConfig
    {
        [JsonPropertyName("Prefix")] 
        public string Prefix { get; set; } = "{blue}⌈ varkit ⌋";

        [JsonPropertyName("CTBanPermmissions")]
        public List<string> CTBanPermmissions { get; set; } = new List<string>() {
            "@css/root"
        };
    }
}
