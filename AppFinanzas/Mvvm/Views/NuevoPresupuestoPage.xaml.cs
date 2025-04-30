using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views
{
    public partial class NuevoPresupuestoPage : ContentPage
    {
        public NuevoPresupuestoPage()
        {
            InitializeComponent();
            BindingContext = new NuevoPresupuestoViewModel();
        }
    }
}
