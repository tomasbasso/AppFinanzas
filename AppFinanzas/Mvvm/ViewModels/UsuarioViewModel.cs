using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Mvvm.Views;
using AppFinanzas.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class UsuarioViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        public ObservableCollection<UsuarioDto> Usuarios { get; } = new();

        public ICommand CargarCommand { get; }
        public ICommand IrANuevoCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand VolverCommand { get; }

        public UsuarioViewModel()
        {
            CargarCommand = new Command(async () => await CargarUsuariosAsync());
            IrANuevoCommand = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new UsuarioFormPage());
            });
            EditarCommand = new Command<UsuarioDto>(async (usuario) => await EditarUsuario(usuario));
            EliminarCommand = new Command<UsuarioDto>(async (usuario) => await EliminarUsuario(usuario));
            VolverCommand = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PopAsync();
            });

            CargarCommand.Execute(null);
        }

        private async Task CargarUsuariosAsync()
        {
            try
            {
                var lista = await _apiService.GetUsuariosAsync();
                Usuarios.Clear();
                foreach (var usuario in lista)
                    Usuarios.Add(usuario);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task EditarUsuario(UsuarioDto usuario)
        {           
            await Application.Current.MainPage.Navigation.PushAsync(new UsuarioFormPage(usuario));           
        }

        private async Task EliminarUsuario(UsuarioDto usuario)
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "¿Eliminar usuario?", "Sí", "No");
            if (!confirm) return;

            try
            {
                await _apiService.EliminarUsuarioAsync(usuario.UsuarioId);
                Usuarios.Remove(usuario);
                await CargarUsuariosAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        public async Task RecargarUsuarios()
        {
            await CargarUsuariosAsync();
        }
    }
}
