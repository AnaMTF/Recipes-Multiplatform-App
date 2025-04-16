using Microsoft.Maui.Controls;
using Proiect.Data;
using Proiect.Models;
using Proiect.Services;
using System;
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
            // Preia rețetele din baza de date
            RecipeDao recipeDao = new RecipeDao();
            var dbRecipes = recipeDao.GetAllRecipes();  // Lista completă din DB

            // Populează AllRecipes
            AllRecipes.Clear();
            foreach (var recipe in dbRecipes)
            {
                AllRecipes.Add(recipe);
            }

            // Top Hottest Recipes: cele mai recente 5 rețete (ordonare descrescătoare după ID)
            TopRecipes.Clear();
            foreach (var recipe in dbRecipes.OrderByDescending(r => r.Id).Take(5))
            {
                TopRecipes.Add(recipe);
            }

            // Recommended for You: 5 rețete alese aleatoriu
            RecommendedRecipes.Clear();
            // Folosim OrderBy(x => Guid.NewGuid()) pentru a obține o listă "amestecată"
            foreach (var recipe in dbRecipes.OrderBy(x => Guid.NewGuid()).Take(5))
            {
                RecommendedRecipes.Add(recipe);
            }

            // Extrage categoriile unice din rețete
            var uniqueCategories = dbRecipes
                .Where(r => !string.IsNullOrWhiteSpace(r.Category))
                .Select(r => r.Category)
                .Distinct();

            Categories.Clear();
            foreach (var cat in uniqueCategories)
            {
                Categories.Add(new Category
                {
                    Name = cat,
                    ImageUrl = $"https://via.placeholder.com/60.png?text={Uri.EscapeDataString(cat)}"
                });
            }

            // Exemplu de animație: fade-in pentru header (dacă dorești)
            await HeaderLabel.FadeTo(1, 1000);
        }

        // Butoanele de filtrare
        private void OnFilterClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            string filter = button?.CommandParameter?.ToString();
            // Logica de filtrare a rețetelor în funcție de filtru, după preferințe.
        }
    }
}
