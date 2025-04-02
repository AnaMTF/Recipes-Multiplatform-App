using Proiect.Models;
using Proiect.Services;
using System.Collections.ObjectModel;

namespace Proiect.Views
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Recipe> TopRecipes { get; set; } = new ObservableCollection<Recipe>();
        public ObservableCollection<Recipe> AllRecipes { get; set; } = new ObservableCollection<Recipe>();
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var service = new RecipeService();
            var recipes = await service.GetRecipesAsync();

            TopRecipes.Clear();
            AllRecipes.Clear();

            // CarouselView
            foreach (var recipe in recipes.Take(5))
            {
                TopRecipes.Add(recipe);
            }

            // Picker
            foreach (var recipe in recipes)
            {
                AllRecipes.Add(recipe);
            }
        }
    }

}
