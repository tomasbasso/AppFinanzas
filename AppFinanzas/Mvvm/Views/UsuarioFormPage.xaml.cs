using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views;

public partial class UsuarioFormPage : ContentPage
{
    UsuarioFormViewModel ViewModel => BindingContext as UsuarioFormViewModel;

    public UsuarioFormPage()
    {
        InitializeComponent();
        BindingContext = new UsuarioFormViewModel();
    }

    public UsuarioFormPage(UsuarioDto usuario) : this()
    {
        ViewModel.CargarUsuario(usuario);
    }
}