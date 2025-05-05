using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Mvvm.Views;
using AppFinanzas.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class TransaccionesViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        public ObservableCollection<TransaccionDto> Transacciones { get; } = new();
        public ICommand CargarTransaccionesCommand { get; }
        public ICommand IrANuevaCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand VolverCommand { get; }
        public ICommand EliminarCommand { get; }

        public TransaccionesViewModel()
        {
            CargarTransaccionesCommand = new Command(async () => await CargarTransacciones());
            CargarTransaccionesCommand.Execute(null);

            IrANuevaCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("//NuevaTransaccionPage");
            });

            _ = CargarTransacciones();

            EditarCommand = new Command<TransaccionDto>(async (t) => await EditarTransaccion(t));
            EliminarCommand = new Command<TransaccionDto>(async (t) => await EliminarTransaccion(t));
            VolverCommand = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PopAsync();
            });
        }

        private async Task CargarTransacciones()
        {
            try
            {
                var lista = await _apiService.GetTransaccionesAsync();
                Transacciones.Clear();
                foreach (var transaccion in lista)
                    Transacciones.Add(transaccion);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
        public async Task RecargarTransaccionesAsync()
        {
            await CargarTransacciones();
        }
        private async Task EditarTransaccion(TransaccionDto transaccion)
        {
            await Shell.Current.GoToAsync(nameof(NuevaTransaccionPage), new Dictionary<string, object>
            {
                ["Transaccion"] = transaccion
            });
        }

        private async Task EliminarTransaccion(TransaccionDto transaccion)
        {
            bool confirm = await Shell.Current.DisplayAlert("Eliminar", "¿Deseás eliminar esta transacción?", "Sí", "No");

            if (!confirm)
                return;

            try
            {
                await _apiService.EliminarTransaccionAsync(transaccion.TransaccionId);
                Transacciones.Remove(transaccion);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}


