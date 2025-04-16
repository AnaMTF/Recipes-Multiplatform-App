using Microsoft.Maui.Controls;
using Proiect.Data;
using Proiect.Models;
using Proiect.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Proiect.Views
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        public ObservableCollection<Recipe> TopRecipes { get; set; } = new ObservableCollection<Recipe>();
        public ObservableCollection<Recipe> AllRecipes { get; set; } = new ObservableCollection<Recipe>();
        public ObservableCollection<Recipe> RecommendedRecipes { get; set; } = new ObservableCollection<Recipe>();
        public ObservableCollection<Recipe> FilteredRecipes { get; set; } = new ObservableCollection<Recipe>();
        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Populează baza de date (dacă nu e deja populată) din sursele JSON și XML
            await PopulateDatabaseIfNeededAsync();

            // Preia rețetele din DB
            RecipeDao recipeDao = new RecipeDao();
            var dbRecipes = recipeDao.GetAllRecipes();

            // Populează AllRecipes
            AllRecipes.Clear();
            foreach (var recipe in dbRecipes)
            {
                AllRecipes.Add(recipe);
            }

            // TopRecipes: ultimele 5 rețete adăugate (ordonare descrescătoare după ID)
            TopRecipes.Clear();
            foreach (var recipe in dbRecipes.OrderByDescending(r => r.Id).Take(5))
            {
                TopRecipes.Add(recipe);
            }

            // RecommendedRecipes: 5 rețete alese aleatoriu
            RecommendedRecipes.Clear();
            foreach (var recipe in dbRecipes.OrderBy(x => Guid.NewGuid()).Take(5))
            {
                RecommendedRecipes.Add(recipe);
            }

            // Extrage categoriile unice din DB
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

            // Ascunde secțiunea de căutare (search results) dacă nu există text
            FilteredRecipes.Clear();
            SearchResultsCollectionView.IsVisible = false;

            await HeaderLabel.FadeTo(1, 1000);
        }

        private async Task PopulateDatabaseIfNeededAsync()
        {
            // Folosește DAO parametru-less
            // Populare rețete din JSON
            RecipeDao recipeDao = new RecipeDao();
            var dbRecipes = recipeDao.GetAllRecipes();
            RecipeService recipeService = new RecipeService();
            var jsonRecipes = await recipeService.GetRecipesAsync();

            // Verifică dacă DB este populată: compară numele primei rețete, dacă există
            if (!(dbRecipes.Any() && jsonRecipes.Any() &&
                  dbRecipes.First().Name.Equals(jsonRecipes.First().Name, StringComparison.OrdinalIgnoreCase)))
            {
                recipeDao.InsertRecipes(jsonRecipes);
            }

            // Populează categoriile din rețete
            CategoryDao catDao = new CategoryDao();
            var uniqueCats = jsonRecipes
                .Where(r => !string.IsNullOrWhiteSpace(r.Category))
                .Select(r => r.Category)
                .Distinct();
            foreach (var cat in uniqueCats)
            {
                var exists = catDao.GetAllCategories().Any(c => c.Name.Equals(cat, StringComparison.OrdinalIgnoreCase));
                if (!exists)
                {
                    catDao.InsertCategory(new Category
                    {
                        Name = cat,
                        ImageUrl = $"https://via.placeholder.com/60.png?text={Uri.EscapeDataString(cat)}"
                    });
                }
            }

            // Populează temele din rețete
            ThemeDao themeDao = new ThemeDao();
            var uniqueThemes = jsonRecipes.SelectMany(r => r.Themes ?? new System.Collections.Generic.List<string>())
                                          .Distinct();
            foreach (var theme in uniqueThemes)
            {
                var exists = themeDao.GetAllThemes().Any(t => t.Name.Equals(theme, StringComparison.OrdinalIgnoreCase));
                if (!exists)
                {
                    themeDao.InsertTheme(new Theme { Name = theme });
                }
            }

            // Populează ingredientele din XML
            var xmlIngredients = IngredientsService.GetIngredient("https://raw.githubusercontent.com/AnaMTF/cookbook-json/refs/heads/main/ingredients.xml").ToList();
            IngredientDao ingredientDao = new IngredientDao();
            var dbIngredients = ingredientDao.GetAllIngredients();
            if (!(dbIngredients.Any() && xmlIngredients.Any() &&
                  dbIngredients.First().IngredientName.Equals(xmlIngredients.First().IngredientName, StringComparison.OrdinalIgnoreCase)))
            {
                ingredientDao.InsertIngredients(xmlIngredients);
            }
        }

        // Handler pentru SearchBar TextChanged - actualizează colecția filtrată
        void RecipeSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = e.NewTextValue?.ToLower() ?? "";
            FilteredRecipes.Clear();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                SearchResultsCollectionView.IsVisible = false;
            }
            else
            {
                var filtered = AllRecipes.Where(r => (r.Name?.ToLower().Contains(keyword) ?? false)
                                                  || (r.Description?.ToLower().Contains(keyword) ?? false)).ToList();
                foreach (var recipe in filtered)
                {
                    FilteredRecipes.Add(recipe);
                }
                SearchResultsCollectionView.IsVisible = FilteredRecipes.Any();
            }
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Handler pentru selecția din CollectionView-ul "Recomandate pentru tine"
        void RecommendedRecipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Recipe selectedRecipe)
            {
                Navigation.PushAsync(new Details(selectedRecipe));
                ((CollectionView)sender).SelectedItem = null;
            }
        }


        // Handler pentru SelectionChanged în CollectionView-ul de rezultate de căutare
        void SearchResultsCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Obține rețeta selectată
            if (e.CurrentSelection.FirstOrDefault() is Recipe selectedRecipe)
            {
                // Navighează către pagina de detalii, transmițând obiectul Recipe
                Navigation.PushAsync(new Details(selectedRecipe));
                // Resetează selecția
                ((CollectionView)sender).SelectedItem = null;
            }
        }

    }
}
