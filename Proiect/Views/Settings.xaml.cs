using Microsoft.Maui.Controls;

namespace Proiect.Views;

public partial class Settings : ContentPage
{
    private bool isDark = false;

    public Settings()
    {
        InitializeComponent();
        LoadCurrentTheme();
    }

    private void LoadCurrentTheme()
    {
        isDark = App.Current.UserAppTheme == AppTheme.Dark;
        ThemeToggleButton.Text = isDark ? "Switch to Light Theme" : "Switch to Dark Theme";
    }

    private void OnThemeToggleClicked(object sender, EventArgs e)
    {
        isDark = !isDark;
        App.Current.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
        ThemeToggleButton.Text = isDark ? "Switch to Light Theme" : "Switch to Dark Theme";

        // Actualizare dinamică a culorilor
        if (isDark)
        {
            Application.Current.Resources["TextColorPrimary"] = Application.Current.Resources["DarkTextColorPrimary"];
            Application.Current.Resources["TextColorSecondary"] = Application.Current.Resources["DarkTextColorSecondary"];
            Application.Current.Resources["FrameBackground"] = Application.Current.Resources["DarkFrameBackground"];
            Application.Current.Resources["AccentColor"] = Application.Current.Resources["DarkAccentColor"];
        }
        else
        {
            Application.Current.Resources["TextColorPrimary"] = Application.Current.Resources["LightTextColorPrimary"];
            Application.Current.Resources["TextColorSecondary"] = Application.Current.Resources["LightTextColorSecondary"];
            Application.Current.Resources["FrameBackground"] = Application.Current.Resources["LightFrameBackground"];
            Application.Current.Resources["AccentColor"] = Application.Current.Resources["LightAccentColor"];
        }
    }


    private void OnFontSizeChanged(object sender, EventArgs e)
    {
        var selected = FontSizePicker.SelectedItem as string;

        double baseSize = 14; // default (Medium)

        switch (selected)
        {
            case "Small":
                baseSize = 12;
                break;
            case "Medium":
                baseSize = 14;
                break;
            case "Large":
                baseSize = 18;
                break;
        }

        if (Application.Current.Resources.ContainsKey("FontSizeBase"))
        {
            Application.Current.Resources["FontSizeBase"] = baseSize;
        }
        else
        {
            Application.Current.Resources.Add("FontSizeBase", baseSize);
        }
    }
}