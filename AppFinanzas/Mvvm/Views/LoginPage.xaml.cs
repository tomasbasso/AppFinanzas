using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();
        SizeChanged += LoginPage_SizeChanged;
    }

    private void LoginPage_SizeChanged(object? sender, EventArgs e)
    {
        if (Height > 0)
        {
            CenterContainer.MinimumHeightRequest = Height;
        }
    }
}
