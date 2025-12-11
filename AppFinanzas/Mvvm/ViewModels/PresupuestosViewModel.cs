using AppFinanzas.Data;
using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Mvvm.Views;
using AppFinanzas.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class PresupuestosViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        public ObservableCollection<PresupuestoDto> Presupuestos { get; } = new();
        public ICommand CargarPresupuestosCommand { get; }
        public ICommand IrANuevaCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand VolverCommand { get; }
        public ICommand EliminarCommand { get; }

        public PresupuestosViewModel()
        {
            CargarPresupuestosCommand = new Command(async () => await CargarPresupuestos());
            CargarPresupuestosCommand.Execute(null);
            IrANuevaCommand = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new NuevoPresupuestoPage());
            });
            EditarCommand = new Command<PresupuestoDto>(async (presupuesto) => await EditarPresupuesto(presupuesto));
            EliminarCommand = new Command<PresupuestoDto>(async (presupuesto) => await EliminarPresupuesto(presupuesto));
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
        private async Task EliminarPresupuesto(PresupuestoDto presupuesto)
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "¿Eliminar cuenta?", "Sí", "No");

            if (!confirm) return;

            try
            {
                await _apiService.EliminarPresupuestoAsync(presupuesto.PresupuestoId);
                Presupuestos.Remove(presupuesto);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
        private async Task EditarPresupuesto(PresupuestoDto presupuesto)
        {
            await Shell.Current.GoToAsync(nameof(NuevoPresupuestoPage), new Dictionary<string, object>
            {
                ["Presupuesto"] = presupuesto
            });
        }


        private async Task CargarPresupuestos()
        {
            try
            {
                var lista = await _apiService.GetPresupuestosAsync();
                Presupuestos.Clear();
                foreach (var p in lista)
                    Presupuestos.Add(p);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
