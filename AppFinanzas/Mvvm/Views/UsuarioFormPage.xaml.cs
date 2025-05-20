using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views;

public partial class UsuarioFormPage : ContentPage
{
    public UsuarioFormPage()
    {
        InitializeComponent();
        BindingContext = new UsuarioFormViewModel();
    }

    public UsuarioFormPage(UsuarioDto usuario) : this()
    {
        (BindingContext as UsuarioFormViewModel)?.CargarUsuario(usuario);
    }
}
