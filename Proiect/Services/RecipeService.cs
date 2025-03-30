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
        private const string RecipesUrl = "https://raw.githubusercontent.com/AnaMTF/cookbook-json/refs/heads/main/updated_recipes_with_times.json";

        public async Task<List<Recipe>> GetRecipesAsync()
        {
            using (var client = new HttpClient())
            {
                var json = await client.GetStringAsync(RecipesUrl);
                var recipes = JsonSerializer.Deserialize<List<Recipe>>(json);
                return recipes;
            }
        }
    }
}
