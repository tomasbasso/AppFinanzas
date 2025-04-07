using AppFinanzas.Mvvm.ModelsDto;
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

        public TransaccionesViewModel()
        {
            CargarTransaccionesCommand = new Command(async () => await CargarTransacciones());
            CargarTransaccionesCommand.Execute(null);
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
    }
}
