using Proiect.Data;
using Proiect.Models;

namespace Proiect.Views;

public partial class EditRecipePage : ContentPage
{
    private RecipeDao _recipeDao = new RecipeDao();
    private List<Recipe> _allRecipes = new();
    private Recipe _selectedRecipe;

    private List<string> _allCategories = new();
    private List<string> _allThemes = new();

    public EditRecipePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadData();
    }

    private void LoadData()
    {
        // Încărcăm rețetele o singură dată
        _allRecipes = _recipeDao.GetAllRecipes();
        recipePicker.ItemsSource = _allRecipes;

        // CATEGORII DISTINCTE
        _allCategories = _allRecipes
            .Select(r => r.Category)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct()
            .ToList();
        categoryPicker.ItemsSource = _allCategories;

        // TEME DISTINCTE
        _allThemes = _allRecipes
            .SelectMany(r => r.Themes ?? new List<string>())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .ToList();
        themesPicker.ItemsSource = _allThemes;
    }

    private void OnRecipeSelected(object sender, EventArgs e)
    {
        if (recipePicker.SelectedIndex == -1)
            return;

        _selectedRecipe = recipePicker.SelectedItem as Recipe;

        if (_selectedRecipe != null)
        {
            nameEntry.Text = _selectedRecipe.Name;
            descriptionEditor.Text = _selectedRecipe.Description;
            methodEditor.Text = string.Join("\n", _selectedRecipe.Method);
            prepTimeEntry.Text = _selectedRecipe.PreppingTime;
            cookTimeEntry.Text = _selectedRecipe.CookingTime;

            // Setăm categoria
            if (!string.IsNullOrEmpty(_selectedRecipe.Category))
                categoryPicker.SelectedItem = _selectedRecipe.Category;
            else
                categoryPicker.SelectedIndex = -1;

            // Setăm tema
            if (_selectedRecipe.Themes != null && _selectedRecipe.Themes.Any())
                themesPicker.SelectedItem = _selectedRecipe.Themes.First();
            else
                themesPicker.SelectedIndex = -1;

            // Ingredients (ca text)
            if (_selectedRecipe.Ingredients != null)
                ingredientsEditor.Text = string.Join("\n", _selectedRecipe.Ingredients);
            else
                ingredientsEditor.Text = string.Empty;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_selectedRecipe != null)
        {
            _selectedRecipe.Name = nameEntry.Text;
            _selectedRecipe.Description = descriptionEditor.Text;
            _selectedRecipe.Category = categoryPicker.SelectedItem?.ToString();
            _selectedRecipe.Themes = new List<string> { themesPicker.SelectedItem?.ToString() ?? string.Empty };
            _selectedRecipe.Ingredients = ingredientsEditor.Text
                .Split('\n')
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .ToList();
            _selectedRecipe.Method = methodEditor.Text
                .Split('\n')
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .ToList();
            _selectedRecipe.PreppingTime = prepTimeEntry.Text;
            _selectedRecipe.CookingTime = cookTimeEntry.Text;

            _recipeDao.UpdateRecipe(_selectedRecipe);

            await DisplayAlert("Success", "Recipe updated!", "OK");

            ResetUI();
        }
    }

    private void ResetUI()
    {
        nameEntry.Text = string.Empty;
        descriptionEditor.Text = string.Empty;
        ingredientsEditor.Text = string.Empty;
        methodEditor.Text = string.Empty;
        prepTimeEntry.Text = string.Empty;
        cookTimeEntry.Text = string.Empty;

        recipePicker.SelectedIndex = -1;
        categoryPicker.SelectedIndex = -1;
        themesPicker.SelectedIndex = -1;

        _selectedRecipe = null;
    }
}
