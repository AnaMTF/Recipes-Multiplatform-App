using Microsoft.Maui.Controls;
using Proiect.Data;
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
            // Pentru a popula Picker-ul cu rețete din baza de date:
            
            RecipeDao recipeDao = new RecipeDao();
            var dbRecipes = recipeDao.GetAllRecipes();  // preia lista de rețete din DB


            // Populează colecțiile pentru Carousel, Picker, Recomandate
            AllRecipes.Clear();
            foreach (var recipe in dbRecipes)
            {
                AllRecipes.Add(recipe);
            }

            // Exemplu: pentru Carousel și Recomandate
            TopRecipes.Clear();
            foreach (var recipe in dbRecipes.Take(5))
            {
                TopRecipes.Add(recipe);
            }
            RecommendedRecipes.Clear();
            foreach (var recipe in dbRecipes.Where(r => r.Themes != null && r.Themes.Contains("christmas")).Take(5))
            {
                RecommendedRecipes.Add(recipe);
            }

            // Extrage categoriile unice din rețete
            var uniqueCategories = dbRecipes
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
                    ImageUrl = $"https://via.placeholder.com/60.png?text={Uri.EscapeDataString(cat)}"
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
