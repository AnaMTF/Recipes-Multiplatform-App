using Proiect.Data;
using Proiect.Models;

namespace Proiect.Views;

public partial class EditRecipePage : ContentPage
{
    private RecipeDao _recipeDao = new RecipeDao();
    private List<Recipe> _allRecipes;
    private Recipe _selectedRecipe;

    public EditRecipePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadRecipes();
    }

    private void LoadRecipes()
    {
        _allRecipes = _recipeDao.GetAllRecipes();
        recipePicker.ItemsSource = _allRecipes;
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
            ingredientsEditor.Text = string.Join("\n", _selectedRecipe.Ingredients);
            methodEditor.Text = string.Join("\n", _selectedRecipe.Method);
            prepTimeEntry.Text = _selectedRecipe.PreppingTime;
            cookTimeEntry.Text = _selectedRecipe.CookingTime;
            categoryEntry.Text = _selectedRecipe.Category;
            themesEditor.Text = string.Join(", ", _selectedRecipe.Themes);
        }
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        if (_selectedRecipe != null)
        {
            _selectedRecipe.Name = nameEntry.Text;
            _selectedRecipe.Description = descriptionEditor.Text;
            _selectedRecipe.Category = categoryEntry.Text;
            _selectedRecipe.Themes = themesEditor.Text.Split(',').Select(t => t.Trim()).ToList();
            _selectedRecipe.Ingredients = ingredientsEditor.Text.Split('\n').Where(i => !string.IsNullOrWhiteSpace(i)).ToList();
            _selectedRecipe.Method = methodEditor.Text.Split('\n').Where(m => !string.IsNullOrWhiteSpace(m)).ToList();
            _selectedRecipe.PreppingTime = prepTimeEntry.Text;
            _selectedRecipe.CookingTime = cookTimeEntry.Text;

            _recipeDao.UpdateRecipe(_selectedRecipe);

            DisplayAlert("Success", "Recipe updated!", "OK");

            // Golește toate câmpurile din UI
            nameEntry.Text = string.Empty;
            descriptionEditor.Text = string.Empty;
            categoryEntry.Text = string.Empty;
            themesEditor.Text = string.Empty;
            ingredientsEditor.Text = string.Empty;
            methodEditor.Text = string.Empty;
            prepTimeEntry.Text = string.Empty;
            cookTimeEntry.Text = string.Empty;

            // Resetează selecția
            recipePicker.SelectedIndex = -1;
            _selectedRecipe = null;
        }
    }

}
