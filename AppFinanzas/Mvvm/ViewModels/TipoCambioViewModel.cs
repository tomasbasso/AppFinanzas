using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Mvvm.ViewModels;
using AppFinanzas.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

public class TipoCambioViewModel : BaseViewModel
{
    private readonly ApiService _apiService = new();

    public ObservableCollection<TipoCambioDto> TiposCambio { get; } = new();
    public ICommand CargarCommand { get; }

    public TipoCambioViewModel()
    {
        CargarCommand = new Command(async () => await CargarTiposCambio());
        CargarCommand.Execute(null);
    }

    private async Task CargarTiposCambio()
    {
        try
        {
            var lista = await _apiService.GetTiposCambioExternosAsync();
            TiposCambio.Clear();
            foreach (var tipo in lista)
                TiposCambio.Add(tipo);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}