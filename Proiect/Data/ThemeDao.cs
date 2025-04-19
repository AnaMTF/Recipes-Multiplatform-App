using Proiect.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proiect.Data
{
    public class ThemeDao
    {
        private SQLiteConnection conn;

        public ThemeDao()
        {
            conn = new SQLiteConnection(Path.Combine(FileSystem.AppDataDirectory, "themes.db"));
            conn.CreateTable<Theme>();
        }
        public List<Theme> GetThemeByName(string name)
        {
            return conn.Query<Theme>("SELECT * FROM themes WHERE name = ?", name).ToList();
        }

        public List<Theme> GetAllThemes()
        {
            return conn.Table<Theme>().ToList();
        }

        public int InsertTheme(Theme theme)
        {
            return conn.Insert(theme);
        }

        public int InsertThemes(List<Theme> themes)
        {
            return conn.InsertAll(themes);
        }
    }
}
