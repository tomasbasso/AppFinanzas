using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views
{
    public partial class TransaccionesPage : ContentPage
    {
        public TransaccionesPage()
        {
            InitializeComponent();
            BindingContext = new TransaccionesViewModel();
        }
    }
}
