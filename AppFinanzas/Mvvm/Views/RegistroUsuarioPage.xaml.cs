using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views;

public partial class RegistroUsuarioPage : ContentPage
{
	public RegistroUsuarioPage()
	{
		InitializeComponent();
        BindingContext = new RegistroUsuarioViewModel();
    }
}