using AppFinanzas.Mvvm.Views;
using AppFinanzas.Services;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new ApiService();
        private bool _isLoggingOut;

        public ICommand IrACuentasCommand { get; }
        public ICommand IrATransaccionesCommand { get; }
        public ICommand IrAPresupuestosCommand { get; }
        public ICommand IrAMetasCommand { get; }
        public ICommand IrATipoCambioCommand { get; }
        public ICommand IrAPerfilCommand { get; }
        public ICommand IrAConfiguracionCommand { get; }
        public ICommand LogoutCommand { get; }

        public MenuViewModel()
        {
            IrACuentasCommand = new Command(async () => await Shell.Current.GoToAsync("CuentasPage"));
            IrATransaccionesCommand = new Command(async () => await Shell.Current.GoToAsync("TransaccionesPage"));
            IrAPresupuestosCommand = new Command(async () => await Shell.Current.GoToAsync("PresupuestosPage"));
            IrAMetasCommand = new Command(async () => await Shell.Current.GoToAsync("MetasAhorroPage"));
            IrATipoCambioCommand = new Command(async () => await Shell.Current.GoToAsync("TipoCambioPage"));
            IrAPerfilCommand = new Command(async () => await Shell.Current.GoToAsync("PerfilPage"));
            
            IrAConfiguracionCommand = new Command(async () => await Shell.Current.GoToAsync("//MenuConfiguracionPage?forAdmin=false"));
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

