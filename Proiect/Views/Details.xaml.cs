using Proiect.Models;

namespace Proiect.Views
{
    public partial class Details : ContentPage
    {
        public Details()
        {
            InitializeComponent();
        }

        public Details(Recipe recipe) : this()
        {
            // Set BindingContextul pe rețeta primită
            BindingContext = recipe;
        }
    }
}
