using AppFinanzas.Data;
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
    public ICommand VolverCommand { get; }
    public TipoCambioViewModel()
    {
        CargarCommand = new Command(async () => await CargarTiposCambio());
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