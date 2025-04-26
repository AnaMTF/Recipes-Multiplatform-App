using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using SQLite;

namespace Proiect.Models
{
    [Table("recipes")]
    public class Recipe
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [Column("url")]
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [Column("description")]
        [JsonPropertyName("Description")]
        public string Description { get; set; }

        [Column("author")]
        [JsonPropertyName("Author")]
        public string Author { get; set; }

        // Pentru Ingredients, vom salva JSON-ul în baza de date.
        [Column("ingredients")]
        public string IngredientsJson { get; set; }

        [Ignore]
        [JsonPropertyName("Ingredients")]
        public List<string> Ingredients
        {
            get => !string.IsNullOrWhiteSpace(IngredientsJson)
                    ? JsonSerializer.Deserialize<List<string>>(IngredientsJson)
                    : new List<string>();
            set => IngredientsJson = JsonSerializer.Serialize(value);
        }

        // Similar pentru Method
        [Column("method")]
        public string MethodJson { get; set; }

        [Ignore]
        [JsonPropertyName("Method")]
        public List<string> Method
        {
            get => !string.IsNullOrWhiteSpace(MethodJson)
                    ? JsonSerializer.Deserialize<List<string>>(MethodJson)
                    : new List<string>();
            set => MethodJson = JsonSerializer.Serialize(value);
        }

        [Column("prep_time")]
        [JsonPropertyName("Prepping Time")]
        public string PreppingTime { get; set; }

        [Column("cook_time")]
        [JsonPropertyName("Cooking Time")]
        public string CookingTime { get; set; }

        [Column("total_time")]
        [JsonPropertyName("Total Time")]
        public string TotalTime { get; set; }

        [Column("category")]
        [JsonPropertyName("Category")]
        public string Category { get; set; }

        // Pentru Themes, similar cu cele două liste de mai sus:
        [Column("themes")]
        public string ThemesJson { get; set; }

        [Ignore]
        [JsonPropertyName("Themes")]
        public List<string> Themes
        {
            get => !string.IsNullOrWhiteSpace(ThemesJson)
                    ? JsonSerializer.Deserialize<List<string>>(ThemesJson)
                    : new List<string>();
            set => ThemesJson = JsonSerializer.Serialize(value);
        }

        [Column("image_url")]
        [JsonPropertyName("image")]
        public string ImageUrlDb { get; set; }

        // Proprietate calculată pentru a obține un URL pentru imagine.
        // În lipsa unei proprietăți "ImageUrl" în JSON, folosim un placeholder.
        [Ignore]
        public string ImageUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(ImageUrlDb))
                    return ImageUrlDb;
                return $"https://firstbenefits.org/wp-content/uploads/2017/10/placeholder-1024x1024.png?text={Uri.EscapeDataString(Name)}";
            }
        }
    }
}
