using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proiect.Models
{
    internal class Ingredient
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string IngredientName { get; set; }
        
        public Ingredient() { }

        public override string ToString()
        {
            return IngredientName;
        }
    }
}
