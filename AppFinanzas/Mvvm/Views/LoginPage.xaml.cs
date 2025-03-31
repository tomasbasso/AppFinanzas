using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();
    }
}