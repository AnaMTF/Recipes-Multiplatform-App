using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proiect.Models
{
    [Table("ingredients")]
    public class Ingredient
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        [Column("category")]
        public string Category { get; set; }
        [Column("name")]
        public string IngredientName { get; set; }
        
        public Ingredient() { }

        public override string ToString()
        {
            return IngredientName;
        }
    }
}
