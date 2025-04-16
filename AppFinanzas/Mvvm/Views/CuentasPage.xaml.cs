using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views
{
    public partial class CuentasPage : ContentPage
    {
        private readonly CuentasViewModel _viewModel;

        public CuentasPage()
        {
            InitializeComponent();
            _viewModel = new CuentasViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.RecargarCuentas();
        }
    }
}
