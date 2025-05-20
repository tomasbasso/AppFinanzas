using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class UsuarioViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        public ObservableCollection<UsuarioDto> Usuarios { get; } = new();

        // Propiedades para el panel de edición
        public UsuarioEdicionDto UsuarioEnEdicion { get; set; } = new();
        public UsuarioRegistroDto UsuarioNuevo { get; set; } = new();
        public bool UsuarioEnEdicionVisible { get; set; }
        public bool UsuarioNuevoVisible { get; set; }

        // Comandos
        public ICommand CargarUsuariosCommand { get; }
        public ICommand EditarUsuarioCommand { get; }
        public ICommand EliminarUsuarioCommand { get; }
        public ICommand GuardarUsuarioCommand { get; }
        public ICommand AgregarUsuarioCommand { get; }
        public ICommand GuardarUsuarioNuevoCommand { get; }
        public ICommand CancelarEdicionCommand { get; }
        public ICommand CancelarNuevoCommand { get; }

        public UsuarioViewModel()
        {
            CargarUsuariosCommand = new Command(async () => await CargarUsuarios());
            EditarUsuarioCommand = new Command<UsuarioDto>(EditarUsuario);
            EliminarUsuarioCommand = new Command<int>(async (id) => await EliminarUsuario(id));
            GuardarUsuarioCommand = new Command(async () => await GuardarUsuario());
            AgregarUsuarioCommand = new Command(AgregarUsuario);
            GuardarUsuarioNuevoCommand = new Command(async () => await GuardarUsuarioNuevo());
            CancelarEdicionCommand = new Command(CancelarEdicion);
            CancelarNuevoCommand = new Command(CancelarNuevo);

            CargarUsuariosCommand.Execute(null);
        }

        // Listar usuarios
        private async Task CargarUsuarios()
        {
            try
            {
                var usuarios = await _apiService.GetUsuariosAsync();
                Usuarios.Clear();
                foreach (var u in usuarios)
                    Usuarios.Add(u);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        // Editar usuario existente
        private void EditarUsuario(UsuarioDto usuario)
        {
            UsuarioEnEdicion = new UsuarioEdicionDto
            {
                UsuarioId = usuario.UsuarioId,
                Nombre = usuario.Nombre,
                Rol = usuario.Rol
            };
            UsuarioEnEdicionVisible = true;
            UsuarioNuevoVisible = false;
            OnPropertyChanged(nameof(UsuarioEnEdicion));
            OnPropertyChanged(nameof(UsuarioEnEdicionVisible));
            OnPropertyChanged(nameof(UsuarioNuevoVisible));
        }

        // Guardar edición
        private async Task GuardarUsuario()
        {
            try
            {
                await _apiService.ActualizarUsuarioAsync(UsuarioEnEdicion.UsuarioId, UsuarioEnEdicion);
                UsuarioEnEdicionVisible = false;
                await CargarUsuarios();
                await App.Current.MainPage.DisplayAlert("Listo", "Usuario actualizado", "OK");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            OnPropertyChanged(nameof(UsuarioEnEdicionVisible));
        }

        // Agregar usuario nuevo
        private void AgregarUsuario()
        {
            UsuarioNuevo = new UsuarioRegistroDto();
            UsuarioNuevoVisible = true;
            UsuarioEnEdicionVisible = false;
            OnPropertyChanged(nameof(UsuarioNuevo));
            OnPropertyChanged(nameof(UsuarioNuevoVisible));
            OnPropertyChanged(nameof(UsuarioEnEdicionVisible));
        }

        // Guardar nuevo usuario
        private async Task GuardarUsuarioNuevo()
        {
            try
            {
                await _apiService.CrearUsuarioAsync(UsuarioNuevo);
                UsuarioNuevoVisible = false;
                await CargarUsuarios();
                await App.Current.MainPage.DisplayAlert("Listo", "Usuario creado", "OK");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            OnPropertyChanged(nameof(UsuarioNuevoVisible));
        }

        // Eliminar usuario
        private async Task EliminarUsuario(int usuarioId)
        {
            var confirmar = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Eliminar usuario?", "Sí", "No");
            if (!confirmar) return;

            try
            {
                await _apiService.EliminarUsuarioAsync(usuarioId);
                await CargarUsuarios();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        // Cancelar edición/creación
        private void CancelarEdicion()
        {
            UsuarioEnEdicionVisible = false;
            OnPropertyChanged(nameof(UsuarioEnEdicionVisible));
        }
        private void CancelarNuevo()
        {
            UsuarioNuevoVisible = false;
            OnPropertyChanged(nameof(UsuarioNuevoVisible));
        }
    }
}
