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
        private int _usuarioId;
        public int UsuarioId
        {
            get => _usuarioId;
            set => SetProperty(ref _usuarioId, value);
        }

        private string _nombre;
        public string Nombre
        {
            get => _nombre;
            set => SetProperty(ref _nombre, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        // esto ya lo tenías bien:
        private string? _rolSeleccionado;
        public string? RolSeleccionado
        {
            get => _rolSeleccionado;
            set => SetProperty(ref _rolSeleccionado, value);
        }

        // y lo mismo para la contraseña si querés:
        private string _contrasena;
        public string Contrasena
        {
            get => _contrasena;
            set => SetProperty(ref _contrasena, value);
        }

        private bool _esEdicion;
        public bool EsEdicion
        {
            get => _esEdicion;
            set => SetProperty(ref _esEdicion, value);
        }
        public ObservableCollection<UsuarioDto> Usuarios { get; } = new();

        public ICommand CargarCommand { get; }
        public ICommand IrANuevoCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand VolverCommand { get; }

        public UsuarioViewModel()
        {
            CargarCommand = new Command(async () => await CargarUsuariosAsync());
            IrANuevoCommand = new Command(async () =>   await Application.Current.MainPage.Navigation.PushAsync(new UsuarioFormPage()));
            EditarCommand = new Command<UsuarioDto>(async (usuario) => await EditarUsuario(usuario));
            EliminarCommand = new Command<UsuarioDto>(async (usuario) => await EliminarUsuario(usuario));
            VolverCommand = new Command(async () =>  await Application.Current.MainPage.Navigation.PopAsync());

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
