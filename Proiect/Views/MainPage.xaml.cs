using Proiect.Models;
using Proiect.Services;

namespace Proiect.Views
{
    public partial class MainPage : ContentPage
    {
        string urlIngr = "https://raw.githubusercontent.com/AnaMTF/cookbook-json/refs/heads/main/ingredients.xml";
        int count = 0;
        List<Ingredient> listaIngrediente = new List<Ingredient>();

        public MainPage()
        {
            InitializeComponent();
            listaIngrediente = IngredientsService.GetIngredient(urlIngr);
            testIngr.ItemsSource = listaIngrediente;
        }
    }

}
