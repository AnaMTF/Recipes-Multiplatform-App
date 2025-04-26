using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Proiect.Models;

namespace Proiect.Services
{
    public class RecipeService
    {
        private const string RecipesUrl = "https://raw.githubusercontent.com/AnaMTF/cookbook-json/refs/heads/main/final_recipes_diverse_themes.json";

        public async Task<List<Recipe>> GetRecipesAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var json = await client.GetStringAsync(RecipesUrl);
                    var recipes = JsonSerializer.Deserialize<List<Recipe>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return recipes;
                }
                catch (Exception ex)
                {
                    // Într-o aplicație reală aici se poate loga eroarea sau se poate afișa un mesaj
                    throw new Exception("Eroare la descărcarea rețetelor", ex);
                }
            }
        }

        internal async Task<IEnumerable<object>> GetFavouritesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
