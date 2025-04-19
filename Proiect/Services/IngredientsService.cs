using Proiect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Proiect.Services
{
    internal class IngredientsService
    {
        public static List<Ingredient> GetIngredient(string url)
        {
            List<Ingredient> listaIngrediente = new List<Ingredient>();
            XmlReader reader = XmlReader.Create(url);
            string category = string.Empty;

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    if(reader.Name == "category")
                    {
                        category = reader["name"];
                    }
                    if (reader.Name == "ingredient")
                    {
                        Ingredient ingr = new Ingredient();
                        ingr.Id = Convert.ToInt32(reader["id"]);
                        ingr.Category = category;
                        reader.Read(); // numele ingr
                        ingr.IngredientName = reader.Value;
                        listaIngrediente.Add(ingr);
                    }
                }
            }

            return listaIngrediente;
        }
    }
}
