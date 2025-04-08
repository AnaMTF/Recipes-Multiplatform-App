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
    }
}
