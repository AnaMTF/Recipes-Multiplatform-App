using Proiect.Data;
using Proiect.Models;
using System.Collections.ObjectModel;

namespace Proiect.Views
{
    public partial class Favourites : ContentPage
    {
        private RecipeDao recipeDao;
        public ObservableCollection<Recipe> FavoriteRecipes { get; set; }

        public Favourites()
        {
            InitializeComponent();
            recipeDao = new RecipeDao();
            FavoriteRecipes = new ObservableCollection<Recipe>();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadFavoriteRecipes();
        }

        private void LoadFavoriteRecipes()
        {
            FavoriteRecipes.Clear();
            var favorites = recipeDao.GetFavoriteRecipes();

            foreach (var recipe in favorites)
            {
                FavoriteRecipes.Add(recipe);
            }
        }

        private void OnRecipeSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Recipe selectedRecipe)
            {
                // Navigate to recipe details
                Navigation.PushAsync(new Details(selectedRecipe.Id));
                // Clear selection
                ((CollectionView)sender).SelectedItem = null;
            }
        }

        private void OnRemoveFavoriteClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int recipeId)
            {
                recipeDao.ToggleFavorite(recipeId);
                LoadFavoriteRecipes();
            }
        }
    }
}
