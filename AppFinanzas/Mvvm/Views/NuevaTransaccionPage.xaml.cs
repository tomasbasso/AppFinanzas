using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views
{
    public partial class NuevaTransaccionPage : ContentPage
    {
        public NuevaTransaccionPage()
        {
            InitializeComponent();
            BindingContext = new NuevaTransaccionViewModel();
        }
    }
}
