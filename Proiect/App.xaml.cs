using Proiect.Data;

namespace Proiect
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        // A SE STERGE IN PROIECTUL REAL - ESTE DOAR DE TEST PANA SE DEZVOLTA FUNCTIONALITATEA DE ADAUGARE FAVORITE ==========================
        protected override void OnStart()
        {
            base.OnStart();

            
            var recipeDao = new RecipeDao();
            var recipes = recipeDao.GetAllRecipes();

            if (recipes.Count > 0)
            {
                try
                {
                    recipeDao.Connection.Execute("ALTER TABLE recipes ADD COLUMN is_favorite INTEGER DEFAULT 0");
                }
                catch { }

                
                for (int i = 0; i < Math.Min(3, recipes.Count); i++)
                {
                    recipeDao.Connection.Execute("UPDATE recipes SET is_favorite = 1 WHERE id = ?", recipes[i].Id);
                }
            }
        }
        // PANA AICI SE STERGE ===========================

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}