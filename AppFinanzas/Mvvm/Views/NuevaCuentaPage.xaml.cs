using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views;
[QueryProperty(nameof(Cuenta), "Cuenta")]
public partial class NuevaCuentaPage : ContentPage
{
    public CuentaDto Cuenta { get; set; }

    public NuevaCuentaPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (Cuenta != null)
        {
            BindingContext = new NuevaCuentaViewModel(Cuenta);
        }
        else
        {
            BindingContext = new NuevaCuentaViewModel();
        }
    }
}
