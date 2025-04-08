using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Text.Json;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class TipoCambioViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        public ObservableCollection<TipoCambioDto> TiposCambio { get; } = new();
        public ICommand CargarTiposCambioCommand { get; }

        public TipoCambioViewModel()
        {
            CargarTiposCambioCommand = new Command(async () => await CargarTiposCambioAsync());
            CargarTiposCambioCommand.Execute(null);
        }

        private async Task CargarTiposCambioAsync()
        {
            try
            {
                var lista = await _apiService.GetTiposCambioAsync();
                TiposCambio.Clear();
                foreach (var item in lista)
                    TiposCambio.Add(item);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
