using Proiect.Models;
using Proiect.Services;
using System.Collections.ObjectModel;

namespace Proiect.Views;

public partial class AddRecipe : ContentPage
{
    string urlIngr = "https://raw.githubusercontent.com/AnaMTF/cookbook-json/refs/heads/main/ingredients.xml";
    List<Ingredient> listaIngrediente = new List<Ingredient>();

    // Colectie pentru ingredientele selectate
    public ObservableCollection<Ingredient> SelectedIngredients { get; set; } = new ObservableCollection<Ingredient>();


    // Numărul de câmpuri obligatorii: RecipeName, Description, Ingredients, Steps, PreppingTime, CookingTime
    int totalFields = 6;

    public AddRecipe()
    {
        InitializeComponent();
        listaIngrediente = IngredientsService.GetIngredient(urlIngr);
        BindingContext = this;
        // Populăm Picker‑ul cu ingrediente
        IngredientPicker.ItemsSource = listaIngrediente;
        // Legăm lista de ingrediente selectate la CollectionView
        IngredientsList.ItemsSource = SelectedIngredients;
        // Inițializăm progress bar-ul și total time
        UpdateProgress();
        UpdateTotalTime();
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

    // Eveniment pentru schimbarea selecției în Picker
    void IngredientPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        // TO DO (ceva) 
    }
    
    // Adaugă ingredientul selectat în lista de ingrediente
    void AddIngredient_Clicked(object sender, EventArgs e)
    {
        if (IngredientPicker.SelectedIndex != -1)
        {
            var selectedIngredient = listaIngrediente[IngredientPicker.SelectedIndex];
            // Verificăm dacă nu a fost deja adăugat (folosind numele ca identificator)
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
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to pick photo: {ex.Message}", "OK");
        }
    }

    // Salvarea rețetei (exemplu de alertă; aici se poate integra salvarea în baza de date sau altă logică)
    async void OnSaveClicked(object sender, EventArgs e)
    {
        // Colectează datele introduse (exemplu simplu)
        string recipeName = RecipeName.Text;
        string description = Description.Text;
        string steps = MethodEditor.Text;
        string preppingTime = PreppingTime.Text;
        string cookingTime = CookingTime.Text;
        // Ingredientele sunt în SelectedIngredients

        await DisplayAlert("Saved", "Recipe saved successfully!", "OK");
    }
}