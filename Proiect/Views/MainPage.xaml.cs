using Microsoft.Maui.Controls;
using Proiect.Models;
using Proiect.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace Proiect.Views
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Recipe> TopRecipes { get; set; } = new ObservableCollection<Recipe>();
        public ObservableCollection<Recipe> AllRecipes { get; set; } = new ObservableCollection<Recipe>();
        public ObservableCollection<Recipe> RecommendedRecipes { get; set; } = new ObservableCollection<Recipe>();
        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var recipeService = new RecipeService();
            var recipes = await recipeService.GetRecipesAsync();

            // Populează colecțiile pentru Carousel, Picker, Recomandate
            TopRecipes.Clear();
            AllRecipes.Clear();
            RecommendedRecipes.Clear();

            foreach (var recipe in recipes.Take(5))
            {
                TopRecipes.Add(recipe);
            }
            foreach (var recipe in recipes)
            {
                AllRecipes.Add(recipe);
            }
            // recomandate – rețetele care au tema "christmas"
            foreach (var recipe in recipes.Where(r => r.Themes != null && r.Themes.Contains("christmas")).Take(5))
            {
                RecommendedRecipes.Add(recipe);
            }

            // Extrage categoriile unice din rețete
            var uniqueCategories = recipes
                .Where(r => !string.IsNullOrWhiteSpace(r.Category))
                .Select(r => r.Category)
                .Distinct();

            // Populează colecția de categorii cu obiecte Category 
            Categories.Clear();
            foreach (var cat in uniqueCategories)
            {
                Categories.Add(new Category
                {
                    Name = cat,
                    // poza de exemplu pentru categorie trebuie scimbata
                    ImageUrl = $"https://blogs.biomedcentral.com/on-medicine/wp-content/uploads/sites/6/2019/09/iStock-1131794876.t5d482e40.m800.xtDADj9SvTVFjzuNeGuNUUGY4tm5d6UGU5tkKM0s3iPk-620x342.jpg?text={Uri.EscapeDataString(cat)}"
                });
            }

            // Exemplu de animație: fade-in pentru header nu cred ca merge
            await HeaderLabel.FadeTo(1, 1000);
        }

        //note, hottest ultimele x retete dupa id, recomandate math.random sau ceva de genu
        // Butoanele de filtrare
        private void OnFilterClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            string filter = button?.CommandParameter?.ToString();
            //  logica de filtrare a rețetelor în funcție de filtru.
        }
    }
}
