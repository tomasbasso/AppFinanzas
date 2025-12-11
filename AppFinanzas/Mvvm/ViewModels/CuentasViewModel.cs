using AppFinanzas.Data;
using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Mvvm.Views;
using AppFinanzas.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class CuentasViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        public ObservableCollection<CuentaDto> Cuentas { get; } = new();

        public ICommand CargarCommand { get; }
        public ICommand IrANuevaCommand { get; }
        public ICommand VolverCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand EliminarCommand { get; }

        public CuentasViewModel()
        {
            CargarCommand = new Command(async () => await CargarCuentasAsync());
            IrANuevaCommand = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new NuevaCuentaPage());
            });
            EditarCommand = new Command<CuentaDto>(async (cuenta) => await EditarCuenta(cuenta));
            EliminarCommand = new Command<CuentaDto>(async (cuenta) => await EliminarCuenta(cuenta));

            CargarCommand.Execute(null);
            VolverCommand = new Command(async () => await VolverAlMenuAsync());
        }
        private async Task VolverAlMenuAsync()
        {
            var rol = SesionActual.Usuario?.Rol;

            if (string.Equals(rol, "Administrador", StringComparison.OrdinalIgnoreCase))
            {
                // Menú para admin
                await Shell.Current.GoToAsync("//MenuAdminPage");
            }
            else
            {
                // Menú de usuario común (fallback si no hay sesión)
                await Shell.Current.GoToAsync("//MenuPage");
            }
        }
        private async Task CargarCuentasAsync()
        {
            try
            {
                var lista = await _apiService.GetCuentasAsync();
                Cuentas.Clear();
                foreach (var cuenta in lista)
                    Cuentas.Add(cuenta);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task EditarCuenta(CuentaDto cuenta)
        {
            await Shell.Current.GoToAsync(nameof(NuevaCuentaPage), new Dictionary<string, object>
            {
                ["Cuenta"] = cuenta
            });
        }

        private async Task EliminarCuenta(CuentaDto cuenta)
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "¿Eliminar cuenta?", "Sí", "No");

            if (!confirm) return;

            try
            {
                await _apiService.EliminarCuentaAsync(cuenta.CuentaId);
                Cuentas.Remove(cuenta);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        public async Task RecargarCuentas()
        {
            await CargarCuentasAsync();
        }
    }
}
