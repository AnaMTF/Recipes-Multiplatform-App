using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Proiect.Data;
using Proiect.Models;
using Proiect.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace Proiect.Views
{
    public partial class AddRecipe : ContentPage, INotifyPropertyChanged
    {
        // URL-ul pentru lista de ingrediente (din XML)
        string urlIngr = "https://raw.githubusercontent.com/AnaMTF/cookbook-json/refs/heads/main/ingredients.xml";
        List<Ingredient> listaIngrediente = new List<Ingredient>();

        // Colecție pentru ingredientele selectate
        public ObservableCollection<Ingredient> SelectedIngredients { get; set; } = new ObservableCollection<Ingredient>();

        // Liste pentru categorii și teme disponibile (acestea vor fi preluate din baza de date)
        public ObservableCollection<string> AvailableCategories { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AvailableThemes { get; set; } = new ObservableCollection<string>();

        // Colecție pentru temele selectate
        public ObservableCollection<string> SelectedThemes { get; set; } = new ObservableCollection<string>();

        // Proprietate pentru a controla vizibilitatea Entry-ului pentru tema personalizată
        private bool isCustomThemeVisible;
        public bool IsCustomThemeVisible
        {
            get => isCustomThemeVisible;
            set
            {
                isCustomThemeVisible = value;
                OnPropertyChanged(nameof(IsCustomThemeVisible));
            }
        }

        // Totalul de câmpuri obligatorii pentru progress: 9
        // 1 - RecipeName, 2 - Description, 3 - Ingredients, 4 - Steps,
        // 5 - PreppingTime, 6 - CookingTime, 7 - Category, 8 - Themes, 9 - Photo.
        int totalFields = 9;

        public string imagePath { get; set; } = "https://firstbenefits.org/wp-content/uploads/2017/10/placeholder-1024x1024.png"; // pentru imaginea rețetei

        public AddRecipe()
        {
            InitializeComponent();
            BindingContext = this;

            // Inițializează lista de ingrediente din XML
            listaIngrediente = IngredientsService.GetIngredient(urlIngr);
            IngredientPicker.ItemsSource = listaIngrediente;
            IngredientsList.ItemsSource = SelectedIngredients;

            // Preluare AvailableCategories din baza de date
            CategoryDao catDao = new CategoryDao(); // constructor parametru-less
            var dbCategories = catDao.GetAllCategories();
            AvailableCategories.Clear();
            foreach (var cat in dbCategories)
            {
                AvailableCategories.Add(cat.Name);
            }
            if (!AvailableCategories.Contains("Other"))
                AvailableCategories.Add("Other");

            // Preluare AvailableThemes din baza de date
            ThemeDao themeDao = new ThemeDao(); // constructor parametru-less
            var dbThemes = themeDao.GetAllThemes();
            AvailableThemes.Clear();
            foreach (var th in dbThemes)
            {
                AvailableThemes.Add(th.Name);
            }
            if (!AvailableThemes.Contains("other"))
                AvailableThemes.Add("other");

            UpdateProgress();
            UpdateTotalTime();
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Actualizează progress bar-ul în funcție de câmpurile completate
        void UpdateProgress()
        {
            int filled = 0;
            if (!string.IsNullOrWhiteSpace(RecipeName.Text))
                filled++;
            if (!string.IsNullOrWhiteSpace(Description.Text))
                filled++;
            if (SelectedIngredients.Count > 0)
                filled++;
            if (!string.IsNullOrWhiteSpace(MethodEditor.Text))
                filled++;
            if (!string.IsNullOrWhiteSpace(PreppingTime.Text))
                filled++;
            if (!string.IsNullOrWhiteSpace(CookingTime.Text))
                filled++;

            // Verificare pentru Category:
            if (CategoryPicker.SelectedItem != null)
            {
                string cat = CategoryPicker.SelectedItem.ToString();
                if (cat == "Other")
                {
                    if (!string.IsNullOrWhiteSpace(CustomCategoryEntry.Text))
                        filled++;
                }
                else
                    filled++;
            }

            // Verificare pentru Themes:
            if (SelectedThemes.Count > 0)
                filled++;

            // Verificare pentru Recipe Photo:
            if (RecipeImage.Source != null)
                filled++;

            double progress = (double)filled / totalFields;
            FormProgressBar.Progress = progress;
        }

        // Actualizează TotalTime prin suma valorilor din PreppingTime și CookingTime
        void UpdateTotalTime()
        {
            if (int.TryParse(PreppingTime.Text, out int prep) && int.TryParse(CookingTime.Text, out int cook))
            {
                int total = prep + cook;
                TotalTimeLabel.Text = total.ToString() + " minutes";
            }
            else
            {
                TotalTimeLabel.Text = "0 minutes";
            }
        }

        // Eveniment pentru câmpurile de text
        void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProgress();
        }

        // Eveniment pentru câmpurile de timp
        void OnTimeTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProgress();
            UpdateTotalTime();
        }

        // Eveniment pentru schimbarea selecției în Picker-ul de ingrediente
        void IngredientPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Eventual cod suplimentar...
        }

        // Adaugă ingredientul selectat
        void AddIngredient_Clicked(object sender, EventArgs e)
        {
            if (IngredientPicker.SelectedIndex != -1)
            {
                var selectedIngredient = listaIngrediente[IngredientPicker.SelectedIndex];
                if (!SelectedIngredients.Any(i => i.IngredientName == selectedIngredient.IngredientName))
                {
                    SelectedIngredients.Add(selectedIngredient);
                    UpdateProgress();
                }
            }
        }

        // Handler pentru alegerea unei poze pentru rețetă
        private async void OnPickPhotoClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a photo for the recipe"
                });
                if (result != null)
                {
                    var stream = await result.OpenReadAsync();

                    var fileName = result.FileName;
                    var path = Path.Combine(FileSystem.CacheDirectory, fileName);

                    using (var fileStream = File.OpenWrite(path))
                    {
                        await stream.CopyToAsync(fileStream);
                    }

                    this.imagePath = path;
                    RecipeImage.Source = path;

                    UpdateProgress();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to pick photo: {ex.Message}", "OK");
            }
        }

        // Handler pentru Picker-ul de categorii
        void CategoryPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CategoryPicker.SelectedItem?.ToString() == "Other")
                CustomCategoryEntry.IsVisible = true;
            else
                CustomCategoryEntry.IsVisible = false;
            UpdateProgress();
        }

        // Handler pentru Picker-ul de teme
        void ThemePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ThemePicker.SelectedItem?.ToString().ToLower() == "other")
            {
                CustomThemeEntry.IsVisible = true;
                IsCustomThemeVisible = true;
            }
            else
            {
                CustomThemeEntry.IsVisible = false;
                IsCustomThemeVisible = false;
            }
        }

        // Un singur buton pentru adăugarea temei: dacă "other" este selectat, se folosește textul din CustomThemeEntry
        void AddTheme_Clicked(object sender, EventArgs e)
        {
            if (ThemePicker.SelectedIndex != -1)
            {
                string selectedTheme = ThemePicker.SelectedItem.ToString();
                ThemeDao themeDao = new ThemeDao(); // constructor parametru-less

                if (selectedTheme.ToLower() == "other")
                {
                    if (!string.IsNullOrWhiteSpace(CustomThemeEntry.Text))
                    {
                        // Dacă tema personalizată nu există în DB, o inserăm
                        var existingThemes = themeDao.GetAllThemes();
                        if (!existingThemes.Any(t => t.Name.Equals(CustomThemeEntry.Text, StringComparison.OrdinalIgnoreCase)))
                        {
                            themeDao.InsertTheme(new Theme { Name = CustomThemeEntry.Text });
                        }
                        if (!SelectedThemes.Contains(CustomThemeEntry.Text))
                        {
                            SelectedThemes.Add(CustomThemeEntry.Text);
                        }
                        CustomThemeEntry.Text = string.Empty;
                    }
                    else
                    {
                        DisplayAlert("Alert", "Introduceți o temă personalizată.", "OK");
                    }
                }
                else
                {
                    if (!SelectedThemes.Contains(selectedTheme))
                    {
                        SelectedThemes.Add(selectedTheme);
                    }
                }
                // Actualizează AvailableThemes din DB
                var updatedThemes = themeDao.GetAllThemes();
                AvailableThemes.Clear();
                foreach (var th in updatedThemes)
                    AvailableThemes.Add(th.Name);
                if (!AvailableThemes.Contains("other"))
                    AvailableThemes.Add("other");

                UpdateProgress();
            }
        }

        // Salvarea rețetei în baza de date
        async void OnSaveClicked(object sender, EventArgs e)
        {
            string dbPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "recipe.db");

            // Utilizează DAO pentru categorii
            RecipeDao recipeDao = new RecipeDao();
            CategoryDao catDao = new CategoryDao();

            // Gestionează categoria
            string category;
            if (CategoryPicker.SelectedItem?.ToString() == "Other")
            {
                if (!string.IsNullOrWhiteSpace(CustomCategoryEntry.Text))
                {
                    var existingCategories = catDao.GetAllCategories();
                    if (!existingCategories.Any(c => c.Name.Equals(CustomCategoryEntry.Text, StringComparison.OrdinalIgnoreCase)))
                    {
                        catDao.InsertCategory(new Category
                        {
                            Name = CustomCategoryEntry.Text,
                            ImageUrl = $"https://via.placeholder.com/60.png?text={Uri.EscapeDataString(CustomCategoryEntry.Text)}"
                        });
                    }
                    category = CustomCategoryEntry.Text;
                }
                else
                {
                    category = "";
                }
            }
            else
            {
                category = CategoryPicker.SelectedItem?.ToString();
            }

            // Preluare alte câmpuri
            string recipeName = RecipeName.Text;
            string description = Description.Text;
            List<string> stepsList = MethodEditor.Text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                                      .Select(s => s.Trim())
                                                      .ToList();
            string preppingTime = PreppingTime.Text;
            string cookingTime = CookingTime.Text;
            List<string> ingredientsList = SelectedIngredients.Select(i => i.IngredientName).ToList();
            List<string> themesList = SelectedThemes.ToList();

            string totalTime = "0 minutes";
            if (int.TryParse(preppingTime, out int prep) && int.TryParse(cookingTime, out int cook))
            {
                totalTime = (prep + cook).ToString() + " Minutes";
            }

            // Creează obiectul Recipe
            Recipe newRecipe = new Recipe
            {
                Name = recipeName,
                Description = description,
                Author = "Ana M.",
                Ingredients = ingredientsList,
                Method = stepsList,
                PreppingTime = preppingTime,
                CookingTime = cookingTime,
                TotalTime = totalTime,
                Category = category,
                Themes = themesList,
                ImageUrlDb = this.imagePath
            };

            int result = recipeDao.InsertRecipe(newRecipe);

            if (result > 0)
            {
                await DisplayAlert("Saved",
                    $"Recipe '{recipeName}' saved successfully!\nCategory: {category}\nThemes: {string.Join(", ", themesList)}",
                    "OK");
                ClearForm();
            }
            else
            {
                await DisplayAlert("Error", "Failed to save the recipe.", "OK");
            }
        }

        /// <summary>
        /// Resetează toate câmpurile din formular după salvare.
        /// </summary>
        void ClearForm()
        {
            // Resetează câmpurile text
            RecipeName.Text = string.Empty;
            Description.Text = string.Empty;
            MethodEditor.Text = string.Empty;
            PreppingTime.Text = string.Empty;
            CookingTime.Text = string.Empty;
            TotalTimeLabel.Text = "0 minutes";

            // Resetează pickerele și entry-urile personalizate
            CategoryPicker.SelectedIndex = -1;
            CustomCategoryEntry.Text = string.Empty;
            CustomCategoryEntry.IsVisible = false;

            ThemePicker.SelectedIndex = -1;
            CustomThemeEntry.Text = string.Empty;
            CustomThemeEntry.IsVisible = false;
            IsCustomThemeVisible = false;

            // Golește colecțiile de ingrediente și teme selectate
            SelectedIngredients.Clear();
            SelectedThemes.Clear();

            // Resetează imaginea rețetei
            RecipeImage.Source = null;

            // Actualizează progress bar-ul
            UpdateProgress();
        }
    }
}
