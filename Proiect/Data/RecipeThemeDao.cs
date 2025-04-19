using Proiect.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proiect.Data
{
    class RecipeThemeDao
    {
        private SQLiteConnection conn;

        public RecipeThemeDao()
        {
            conn = new SQLiteConnection(Path.Combine(FileSystem.AppDataDirectory, "recipe_themes.db"));
            conn.CreateTable<RecipeTheme>();
        }
        public List<RecipeTheme> GetRecipeThemeByName(int id)
        {
            return conn.Query<RecipeTheme>("SELECT * FROM recipe_themes WHERE id = ?", id).ToList();
        }

        public List<RecipeTheme> GetAllRecipeThemes()
        {
            return conn.Table<RecipeTheme>().ToList();
        }

        public int InsertRecipeTheme(RecipeTheme rec_theme)
        {
            return conn.Insert(rec_theme);
        }

        public int InsertRecipeThemes(List<RecipeTheme> rec_themes)
        {
            return conn.InsertAll(rec_themes);
        }
    }
}
