using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proiect.Models;

namespace Proiect.Data
{
    public class RecipeDao
    {
        private SQLiteConnection conn;

        // SE STERGE DUPA ADAUGAREA FUNCTIONALITATII DE FAVORITE ==========================
        public SQLiteConnection Connection
        {
            get { return conn; }
        }

        // ==================================================================================

        public RecipeDao()
        {
            conn = new SQLiteConnection(Path.Combine(FileSystem.AppDataDirectory,"recipes.db"));
            conn.CreateTable<Recipe>();
        }

        public List<Recipe> GetRecipe(int id)
        {
            return conn.Query<Recipe>("SELECT * FROM recipes WHERE id = ?", id); //returneaza reteta dupa id
        }

        public List<Recipe> GetAllRecipes()
        {
            return conn.Table<Recipe>().ToList(); //returneaza toate retetele
        }

        public int InsertRecipe(Recipe recipe)
        {
            return conn.Insert(recipe); //one singular recipe
        }

        public int InsertRecipes(List<Recipe> recipes)
        {
            return conn.InsertAll(recipes); //multiple recipes
        }

        public List<Recipe> GetFavoriteRecipes()
        {
            return conn.Table<Recipe>().Where(r => r.IsFavorite).ToList(); //returneaza retetele favorite
        }

        public int ToggleFavorite(int id)
        {
            var recipe = conn.Table<Recipe>().FirstOrDefault(r => r.Id == id);
            if (recipe != null)
            {
                recipe.IsFavorite = !recipe.IsFavorite;
                return conn.Update(recipe);
            }
            return 0;
        }

        public int UpdateRecipe(Recipe recipe)
        {
            return conn.Update(recipe); // simplu și corect
        }
    }
}
