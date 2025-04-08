using AppFinanzas.Mvvm.ModelsDto;
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

        public PresupuestosViewModel()
        {
            CargarPresupuestosCommand = new Command(async () => await CargarPresupuestos());
            CargarPresupuestosCommand.Execute(null);
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
