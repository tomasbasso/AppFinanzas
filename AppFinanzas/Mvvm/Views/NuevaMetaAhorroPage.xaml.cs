using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views;

public partial class NuevaMetaAhorroPage : ContentPage
{
	public NuevaMetaAhorroPage()
	{
		InitializeComponent();
        BindingContext = new NuevaMetaAhorroViewModel();
    }
}