using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views
{
    public partial class PresupuestosPage : ContentPage
    {
        public PresupuestosPage()
        {
            InitializeComponent();
            BindingContext = new PresupuestosViewModel();
        }
    }
}
