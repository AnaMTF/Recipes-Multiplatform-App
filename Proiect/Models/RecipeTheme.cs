using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proiect.Models
{
    [Table("recipe_themes")]
    public class RecipeTheme
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        [Column("recipe_id")]
        public int RecipeId { get; set; }
        [Column("theme_id")]
        public int ThemeId { get; set; }
    }
}
