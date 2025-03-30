using System.Collections.ObjectModel;

namespace Proiect.Views;

public partial class AddRecipe : ContentPage
{
    // Colectie pentru ingredientele selectate
    public ObservableCollection<string> SelectedIngredients { get; set; } = new ObservableCollection<string>();

    // Lista de ingrediente predefinite (momentan așa, dar se poate citi dintr-o sursă externă gen json sau xml dacă vreți așa)
    public ObservableCollection<string> AvailableIngredients { get; set; } = new ObservableCollection<string>
        {
            "Tomato", "Cheese", "Onion", "Garlic", "Basil", "Olive Oil", "Salt", "Pepper", "Chicken", "Beef"
        };

    // Numărul de câmpuri obligatorii: RecipeName, Description, Ingredients, Steps, PreppingTime, CookingTime
    int totalFields = 6;

    public AddRecipe()
    {
        InitializeComponent();
        BindingContext = this;
        // Populăm Picker‑ul cu ingrediente
        IngredientPicker.ItemsSource = AvailableIngredients;
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
            string ingredient = AvailableIngredients[IngredientPicker.SelectedIndex];
            if (!SelectedIngredients.Contains(ingredient))
            {
                SelectedIngredients.Add(ingredient);
                UpdateProgress();
            }
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

        await DisplayAlert("Salvat", "Recipe saved successfully!", "OK");
    }
}