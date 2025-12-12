using AppFinanzas.Mvvm.Views;
using AppFinanzas.Services;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public partial class MenuAdminViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new ApiService();
        private bool _isLoggingOut;

        public ICommand IrACuentasCommand { get; }
        public ICommand IrATransaccionesCommand { get; }
        public ICommand IrAPresupuestosCommand { get; }
        public ICommand IrAMetasCommand { get; }
        public ICommand IrATipoCambioCommand { get; }
        public ICommand IrAPerfilCommand { get; }

        // Opciones solo para admin
        public ICommand IrAGestionUsuariosCommand { get; }
        public ICommand IrAReportesCommand { get; }
        public ICommand IrAConfiguracionCommand { get; }
        public ICommand LogoutCommand { get; }

        public MenuAdminViewModel()
        {
            IrACuentasCommand = new Command(async () => await Shell.Current.GoToAsync("//CuentasPage"));
            IrATransaccionesCommand = new Command(async () => await Shell.Current.GoToAsync("TransaccionesPage"));
            IrAPresupuestosCommand = new Command(async () => await Shell.Current.GoToAsync("PresupuestosPage"));
            IrAMetasCommand = new Command(async () => await Shell.Current.GoToAsync("MetasAhorroPage"));
            IrATipoCambioCommand = new Command(async () => await Shell.Current.GoToAsync("TipoCambioPage"));
            IrAPerfilCommand = new Command(async () => await Shell.Current.GoToAsync("PerfilPage"));
            // para admin
            IrAGestionUsuariosCommand = new Command(async () => await Application.Current.MainPage.Navigation.PushAsync(new GestionUsuariosPage()));
            IrAReportesCommand = new Command(async () => await App.Current.MainPage.DisplayAlert("PrÃ³ximamente", "Esta funcionalidad estarÃ¡ disponible en futuras actualizaciones.", "Aceptar"));
            IrAConfiguracionCommand = new Command(async () => await Shell.Current.GoToAsync("//MenuConfiguracionPage?forAdmin=true"));
            LogoutCommand = new Command(async () => await LogoutAsync(), () => !_isLoggingOut);
        }

        private async Task LogoutAsync()
        {
            if (_isLoggingOut)
                return;

            _isLoggingOut = true;
            ((Command)LogoutCommand).ChangeCanExecute();

            try
            {
                await _apiService.LogoutAsync();
                await Shell.Current.GoToAsync("//LoginPage");
            }
            finally
            {
                _isLoggingOut = false;
                ((Command)LogoutCommand).ChangeCanExecute();
            }
        }
    }
}

