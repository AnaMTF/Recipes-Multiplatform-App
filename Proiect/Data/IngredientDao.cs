using Proiect.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proiect.Data
{
    public class IngredientDao
    {
        SQLiteConnection conn;

        public IngredientDao()
        {
            conn = new SQLiteConnection(Path.Combine(FileSystem.AppDataDirectory, "ingredients.db"));
            conn.CreateTable<Ingredient>();
        }

        public List<Ingredient> GetIngredientByName(string name)
        {
            return conn.Query<Ingredient>("SELECT * FROM ingredients WHERE name =?", name).ToList();
        }

        public List<Ingredient> GetAllIngredients()
        {
            return conn.Table<Ingredient>().ToList();
        }

        public int InsertIngredient(Ingredient ingredient)
        {
            return conn.Insert(ingredient);
        }

        public int InsertCategories(List<Ingredient> ingredients)
        {
            return conn.InsertAll(ingredients);
        }
    }
}
