using Proiect.Models;
using Proiect.Data;

namespace Proiect.Views
{
    public partial class Details : ContentPage
    {
        private RecipeDao recipeDao;
        private Recipe recipe;

        public Details()
        {
            InitializeComponent();
            recipeDao = new RecipeDao();
        }

        public Details(Recipe recipe) : this()
        {
            this.recipe = recipe;
            BindingContext = recipe;
            UpdateFavoriteButton();
        }

        public Details(int id) : this()
        {
            LoadRecipeById(id);
        }

        private void LoadRecipeById(int id)
        {
            // Preia reteta din db
            var recipeList = recipeDao.GetRecipe(id);

            if (recipeList != null && recipeList.Count > 0)
            {
                recipe = recipeList[0];
                BindingContext = recipe;
                UpdateFavoriteButton();
            }
            else
            {
                DisplayAlert("Error", "Recipe not found", "OK");
            }
        }

        private void UpdateFavoriteButton()
        {
            if (recipe != null)
            {
                // Actualizeaza butonul favorit
                FavoriteButton.Text = recipe.IsFavorite ? "♥" : "♡";
                FavoriteButton.TextColor = recipe.IsFavorite ? Colors.Red : Colors.Gray;
            }
        }

        private void OnFavoriteClicked(object sender, EventArgs e)
        {
            if (recipe != null)
            {
                // Schimba starea favoritului in db
                recipeDao.ToggleFavorite(recipe.Id);

                recipe.IsFavorite = !recipe.IsFavorite;
                
                UpdateFavoriteButton();
            }
        }
    }
}
