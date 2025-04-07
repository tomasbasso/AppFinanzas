using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views
{
    public partial class CuentasPage : ContentPage
    {
        public CuentasPage()
        {
            InitializeComponent();
            BindingContext = new CuentasViewModel(); // conexión con el ViewModel
        }
    }
}
