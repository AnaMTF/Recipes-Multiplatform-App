using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Proiect.Models
{
    public class Recipe
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("Description")]
        public string Description { get; set; }

        [JsonPropertyName("Author")]
        public string Author { get; set; }

        [JsonPropertyName("Ingredients")]
        public List<string> Ingredients { get; set; }

        [JsonPropertyName("Method")]
        public List<string> Method { get; set; }

        [JsonPropertyName("Prepping Time")]
        public string PreppingTime { get; set; }

        [JsonPropertyName("Cooking Time")]
        public string CookingTime { get; set; }

        [JsonPropertyName("Total Time")]
        public string TotalTime { get; set; }

        // Proprietate calculată pentru a obține un URL pentru imagine.
        // În lipsa unei proprietăți "ImageUrl" în JSON, folosim un placeholder.
        public string ImageUrl
        {
            get
            {
                // Exemplu: folosim un serviciu placeholder care afișează numele rețetei
                return $"https://firstbenefits.org/wp-content/uploads/2017/10/placeholder-1024x1024.png?text={Uri.EscapeDataString(Name)}";
            }
        }
    }
}
