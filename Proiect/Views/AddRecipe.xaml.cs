using Microsoft.Maui.Controls;
using Proiect.Data;
using Proiect.Models;
using Proiect.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Proiect.Views
{
    public partial class AddRecipe : ContentPage, INotifyPropertyChanged
    {
        // URL-ul pentru lista de ingrediente (din XML)
        string urlIngr = "https://raw.githubusercontent.com/AnaMTF/cookbook-json/refs/heads/main/ingredients.xml";
        List<Ingredient> listaIngrediente = new List<Ingredient>();

        // Colecție pentru ingredientele selectate
        public ObservableCollection<Ingredient> SelectedIngredients { get; set; } = new ObservableCollection<Ingredient>();

        // Liste pentru categorii și teme disponibile (de exemplu, preluate din baza de date; aici folosiți valori demo)
        public ObservableCollection<string> AvailableCategories { get; set; } = new ObservableCollection<string>
        {
            "Aperitive", "Fel Principal", "Desert", "Other"
        };

        public ObservableCollection<string> AvailableThemes { get; set; } = new ObservableCollection<string>
        {
            "christmas", "holiday", "kids", "baking", "other"
        };

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

        // Totalul de câmpuri obligatorii pentru progress: 9 (vezi explicația de mai jos)
        // 1 - RecipeName, 2 - Description, 3 - Ingredients, 4 - Steps,
        // 5 - PreppingTime, 6 - CookingTime, 7 - Category, 8 - Themes, 9 - Photo.
        int totalFields = 9;

        public AddRecipe()
        {
            InitializeComponent();
            BindingContext = this;

            // Inițializează lista de ingrediente preluate din XML (se presupune că IngredientsService.GetIngredient returnează o List<Ingredient>)
            listaIngrediente = IngredientsService.GetIngredient(urlIngr);

            // Populează Picker-ul pentru ingrediente
            IngredientPicker.ItemsSource = listaIngrediente;
            // Leagă lista de ingrediente selectate la CollectionView
            IngredientsList.ItemsSource = SelectedIngredients;

            // Alte inițializări pentru Picker-ul de categorie și de teme (acestea sunt deja setate prin Binding în XAML, dacă ai legat AvailableCategories / AvailableThemes)

            UpdateProgress();
            UpdateTotalTime();
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Actualizează progress bar-ul în funcție de completarea câmpurilor:
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
                {
                    filled++;
                }
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
            if (int.TryParse(PreppingTime.Text, out int prep) &&
                int.TryParse(CookingTime.Text, out int cook))
            {
                int total = prep + cook;
                TotalTimeLabel.Text = total.ToString() + " minutes";
            }
            else
            {
                TotalTimeLabel.Text = "0 minutes";
            }
        }

        // Eveniment pentru câmpurile de text (RecipeName, Description, Steps)
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
            // Poți adăuga eventual cod suplimentar aici, dacă e necesar.
        }

        // Adaugă ingredientul selectat în lista de ingrediente
        void AddIngredient_Clicked(object sender, EventArgs e)
        {
            if (IngredientPicker.SelectedIndex != -1)
            {
                var selectedIngredient = listaIngrediente[IngredientPicker.SelectedIndex];
                // Verifică dacă ingredientul nu a fost deja adăugat (după nume)
                if (!SelectedIngredients.Any(i => i.IngredientName == selectedIngredient.IngredientName))
                {
                    SelectedIngredients.Add(selectedIngredient);
                    UpdateProgress();
                }
            }
        }

        // Eveniment pentru alegerea unei poze pentru rețetă
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
                    RecipeImage.Source = ImageSource.FromStream(() => stream);
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
            // Dacă se alege "Other", se afișează Entry-ul pentru categoria personalizată
            if (CategoryPicker.SelectedItem?.ToString() == "Other")
                CustomCategoryEntry.IsVisible = true;
            else
                CustomCategoryEntry.IsVisible = false;

            UpdateProgress();
        }

        // Handler pentru Picker-ul de teme
        void ThemePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Dacă se selectează "other" (ignorând diferența de litere, comparăm în minusculă)
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
            // Nu adăugăm tema aici, ci doar o așteptăm de la utilizator.
        }

        // Un singur buton pentru adăugarea temei – acesta va adăuga tema din Picker dacă nu este "other", sau din CustomThemeEntry altfel
        void AddTheme_Clicked(object sender, EventArgs e)
        {
            if (ThemePicker.SelectedIndex != -1)
            {
                string selectedTheme = ThemePicker.SelectedItem.ToString();
                if (selectedTheme.ToLower() == "other")
                {
                    // Dacă se alege "other", adăugăm tema personalizată din Entry (dacă este completată)
                    if (!string.IsNullOrWhiteSpace(CustomThemeEntry.Text))
                    {
                        if (!SelectedThemes.Contains(CustomThemeEntry.Text))
                        {
                            SelectedThemes.Add(CustomThemeEntry.Text);
                            CustomThemeEntry.Text = string.Empty;
                        }
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
                UpdateProgress();
            }
        }

        // Salvarea rețetei (exemplu de alertă; aici integrezi logica de salvare în baza de date)
        async void OnSaveClicked(object sender, EventArgs e)
        {
            // Preluăm categoria: dacă se alege "Other", folosim textul din CustomCategoryEntry, altfel valoarea Picker-ului
            string category;
            if (CategoryPicker.SelectedItem?.ToString() == "Other")
                category = CustomCategoryEntry.Text;
            else
                category = CategoryPicker.SelectedItem?.ToString();

            // Se preiau și celelalte câmpuri din interfață (exemplu)
            string recipeName = RecipeName.Text;
            string description = Description.Text;
            List<string> stepsList = MethodEditor.Text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                                      .Select(s => s.Trim())
                                                      .ToList();
            string preppingTime = PreppingTime.Text;
            string cookingTime = CookingTime.Text;

            List<string> ingredientsList = SelectedIngredients.Select(i => i.IngredientName).ToList();
            List<string> themesList = SelectedThemes.ToList();

            string totalTime = "no time";
            if (int.TryParse(preppingTime, out int prep) && int.TryParse(cookingTime, out int cook))
            {
                totalTime = (prep + cook).ToString() + " Minutes";
            }

            //obiectul recipe
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
                Themes = themesList
                // Nu stocăm direct poza în model, deoarece proprietatea ImageUrl e calculată;
                // dacă vreti putem pastra calea imaginii (trebuie o proprietate suplimentară (gen photoPath))

            };

            RecipeDao recipeDao = new RecipeDao();
            int result =  recipeDao.InsertRecipe(newRecipe);

            if(result > 0)
            {
                await DisplayAlert("Saved",
                    $"Recipe '{recipeName}' saved successfully!\nCategory: {category}\nThemes: {string.Join(", ", themesList)}",
                    "OK");
            }
            else
            {
                await DisplayAlert("Error", "Failed to save the recipe.", "OK");
            }

        }
    }
}
