using Proiect.Data;
using Proiect.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace Proiect.Views
{
    public partial class CategoryRecipes : ContentPage
    {
        public string CategoryName { get; set; }
        public ObservableCollection<Recipe> Recipes { get; set; } = new ObservableCollection<Recipe>();

        public CategoryRecipes(string category)
        {
            InitializeComponent();
            CategoryName = category;

            // incarcare retete din DB
            var recipeDao = new RecipeDao();
            var allRecipes = recipeDao.GetAllRecipes();
            var filtered = allRecipes
                .Where(r => r.Category != null && r.Category.Equals(category, System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var recipe in filtered)
                Recipes.Add(recipe);

            BindingContext = this;
        }

        void OnRecipeTapped(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Recipe selectedRecipe)
            {
                Navigation.PushAsync(new Details(selectedRecipe));
                ((CollectionView)sender).SelectedItem = null; // deselectează după navigare
            }
        }
    }
}
