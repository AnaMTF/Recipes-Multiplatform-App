using Proiect.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proiect.Data
{
    public class CategoryDao
    {
        private SQLiteConnection conn;

        public CategoryDao()
        {
            conn = new SQLiteConnection(Path.Combine(FileSystem.AppDataDirectory, "categories.db"));
            conn.CreateTable<Category>();
        }

        public List<Category> GetCategoryByName(string name)
        {
            return conn.Query<Category>("SELECT * FROM categories WHERE name = ?", name).ToList();
        }

        public List<Category> GetAllCategories()
        {
            return conn.Table<Category>().ToList();
        }

        public int InsertCategory(Category category)
        {
            return conn.Insert(category);
        }

        public int InsertCategories(List<Category> categories)
        {
            return conn.InsertAll(categories);
        }
    }
}
