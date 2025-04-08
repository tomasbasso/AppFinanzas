using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views
{
    public partial class TipoCambioPage : ContentPage
    {
        public TipoCambioPage()
        {
            InitializeComponent();
            BindingContext = new TipoCambioViewModel();
        }
    }
}
