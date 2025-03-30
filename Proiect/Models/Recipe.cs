using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Proiect.Models
{
    public class Recipe
    {
        public string Name { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> Method { get; set; }
        [JsonPropertyName("Prepping Time")]
        public string PreppingTime { get; set; }
        [JsonPropertyName("Cooking Time")]
        public string CookingTime { get; set; }
        [JsonPropertyName("Total Time")]
        public string TotalTime { get; set; }
    }
}
