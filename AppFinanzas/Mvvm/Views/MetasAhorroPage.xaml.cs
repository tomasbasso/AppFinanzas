using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views;

public partial class MetasAhorroPage : ContentPage
{
	public MetasAhorroPage()
	{
		InitializeComponent();
        BindingContext = new MetasAhorroViewModel();
    }
}