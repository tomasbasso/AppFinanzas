using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views;

public partial class GestionUsuariosPage : ContentPage
{
	public GestionUsuariosPage()
	{
		InitializeComponent();
        BindingContext = new UsuarioViewModel();
    }
}