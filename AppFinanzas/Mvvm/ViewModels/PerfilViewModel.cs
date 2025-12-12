using AppFinanzas.Data;
using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class PerfilViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        private UsuarioDto _usuario;
        public ICommand VolverCommand { get; }
        public UsuarioDto Usuario
        {
            get => _usuario;
            set => SetProperty(ref _usuario, value);
        }

        public ICommand CargarPerfilCommand { get; }

        public PerfilViewModel()
        {
            CargarPerfilCommand = new Command(async () => await CargarPerfilAsync());
            CargarPerfilCommand.Execute(null);
            VolverCommand = new Command(async () => await VolverAlMenuAsync());
        }
        private async Task VolverAlMenuAsync()
        {
            var rol = SesionActual.Usuario?.Rol;

            if (string.Equals(rol, "Administrador", StringComparison.OrdinalIgnoreCase))
            {
                // Menu para admin
                await Shell.Current.GoToAsync("//MenuAdminPage");
            }
            else
            {
                // Menu de usuario (si no hay sesion va aca)
                await Shell.Current.GoToAsync("//MenuPage");
            }
        }

        private async Task CargarPerfilAsync()
        {
            try
            {
                Usuario = await _apiService.GetPerfilAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}

