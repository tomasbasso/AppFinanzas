using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views
{
    public partial class TransaccionesPage : ContentPage
    {
        private readonly TransaccionesViewModel _viewModel;

        public TransaccionesPage()
        {
            InitializeComponent();
            _viewModel = new TransaccionesViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.RecargarTransaccionesAsync();
        }
    }
}
