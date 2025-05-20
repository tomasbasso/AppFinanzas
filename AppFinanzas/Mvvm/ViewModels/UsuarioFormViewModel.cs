using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class UsuarioFormViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        // Propiedades bindables
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public string Contrasena { get; set; }

        public bool EsEdicion { get; set; } // true=editar, false=crear


        // Propiedades de ayuda
        public string TituloPagina => EsEdicion ? "Editar Usuario" : "Nuevo Usuario";
        public string TextoBotonGuardar => EsEdicion ? "Guardar Cambios" : "Crear Usuario";
        public string PlaceholderContrasena => EsEdicion ? "Nueva Contraseña (opcional)" : "Contraseña";

        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }

        public UsuarioFormViewModel()
        {
            GuardarCommand = new Command(async () => await GuardarUsuario());
            CancelarCommand = new Command(async () => await Cancelar());
            RolSeleccionado = null;
        }
        public ObservableCollection<string> Roles { get; set; } = new ObservableCollection<string> { "Cliente", "Administrador" };

        private string _rolSeleccionado;
        public string RolSeleccionado
        {
            get => _rolSeleccionado;
            set
            {
                SetProperty(ref _rolSeleccionado, value);
            }

        }

        // Llamar este método desde el constructor de la Page para editar:
        public void CargarUsuario(UsuarioDto usuario)
        {
            UsuarioId = usuario.UsuarioId;
            Nombre = usuario.Nombre;
            Email = usuario.Email;
            Rol = usuario.Rol;
            EsEdicion = true;
            OnPropertyChanged(nameof(EsEdicion));
            OnPropertyChanged(nameof(TituloPagina));
            OnPropertyChanged(nameof(TextoBotonGuardar));
            OnPropertyChanged(nameof(PlaceholderContrasena));
            OnPropertyChanged(nameof(RolSeleccionado));
        }

        // Para crear, EsEdicion = false por default

        private async Task GuardarUsuario()
        {
            try
            {
                if (EsEdicion)
                {
                    var dto = new UsuarioEdicionDto
                    {
                        UsuarioId = UsuarioId,
                        Nombre = Nombre,
                        Rol = RolSeleccionado,
                        Contrasena = string.IsNullOrWhiteSpace(Contrasena) ? null : Contrasena
                    };
                    if (string.IsNullOrEmpty(RolSeleccionado))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Debés seleccionar un rol.", "OK");
                        return;
                    }
                    await _apiService.ActualizarUsuarioAsync(UsuarioId, dto);
                    await App.Current.MainPage.DisplayAlert("Listo", "Usuario editado correctamente", "OK");
                }
                else
                {
                    var dto = new UsuarioRegistroDto
                    {
                        Nombre = Nombre,
                        Email = Email,
                        Contrasena = Contrasena,
                        Rol = RolSeleccionado
                    };
                    await _apiService.CrearUsuarioAsync(dto);
                    await App.Current.MainPage.DisplayAlert("Listo", "Usuario creado correctamente", "OK");
                }
                // Volver (ejemplo: Shell navigation)
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task Cancelar()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
