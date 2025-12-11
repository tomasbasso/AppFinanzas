using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class UsuarioFormViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();
        private bool _isSaving;

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

        private string _contrasena;
        public string Contrasena
        {
            get => _contrasena;
            set => SetProperty(ref _contrasena, value);
        }

        private string? _rolSeleccionado;
        public string? RolSeleccionado
        {
            get => _rolSeleccionado;
            set => SetProperty(ref _rolSeleccionado, value);
        }

        private bool _esEdicion;
        public bool EsEdicion
        {
            get => _esEdicion;
            set => SetProperty(ref _esEdicion, value);
        }

        public ObservableCollection<string> Roles { get; } =
            new ObservableCollection<string> { "Cliente", "Administrador" };

        public string TituloPagina =>
            EsEdicion ? "Editar Usuario" : "Nuevo Usuario";
        public string TextoBotonGuardar =>
            EsEdicion ? "Guardar Cambios" : "Crear Usuario";
        public string PlaceholderContrasena =>
            EsEdicion
                ? "Nueva Contrasena (opcional)"
                : "Contrasena";

        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }

        public UsuarioFormViewModel()
        {
            GuardarCommand = new Command(async () => await GuardarUsuario(), () => !_isSaving);
            CancelarCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            EsEdicion = false;
        }

        public void CargarUsuario(UsuarioDto usuario)
        {
            UsuarioId = usuario.UsuarioId;
            Nombre = usuario.Nombre;
            Email = usuario.Email;
            RolSeleccionado = usuario.Rol;
            Contrasena = string.Empty;
            EsEdicion = true;

            OnPropertyChanged(nameof(TituloPagina));
            OnPropertyChanged(nameof(TextoBotonGuardar));
            OnPropertyChanged(nameof(PlaceholderContrasena));
        }

        private bool EmailValido(string email) =>
            !string.IsNullOrWhiteSpace(email) &&
            Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        private async Task GuardarUsuario()
        {
            if (_isSaving)
                return;

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                await Shell.Current.DisplayAlert("Error", "El nombre es obligatorio.", "OK");
                return;
            }

            if (!EmailValido(Email))
            {
                await Shell.Current.DisplayAlert("Error", "Email invalido.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(RolSeleccionado))
            {
                await Shell.Current.DisplayAlert("Error", "Debes seleccionar un rol.", "OK");
                return;
            }

            if (!EsEdicion && (string.IsNullOrWhiteSpace(Contrasena) || Contrasena.Length < 6))
            {
                await Shell.Current.DisplayAlert("Error", "La contraseña debe tener al menos 6 caracteres.", "OK");
                return;
            }

            if (EsEdicion && !string.IsNullOrWhiteSpace(Contrasena) && Contrasena.Length < 6)
            {
                await Shell.Current.DisplayAlert("Error", "La nueva contraseña debe tener al menos 6 caracteres.", "OK");
                return;
            }

            try
            {
                _isSaving = true;
                ((Command)GuardarCommand).ChangeCanExecute();

                if (EsEdicion)
                {
                    var dto = new UsuarioEdicionDto
                    {
                        UsuarioId = UsuarioId,
                        Nombre = Nombre.Trim(),
                        Rol = RolSeleccionado,
                        Contrasena = string.IsNullOrWhiteSpace(Contrasena)
                                      ? null
                                      : Contrasena
                    };
                    await _apiService.ActualizarUsuarioAsync(UsuarioId, dto);
                    await Shell.Current.DisplayAlert("Listo", "Usuario editado correctamente", "OK");
                }
                else
                {
                    var dto = new UsuarioRegistroDto
                    {
                        Nombre = Nombre.Trim(),
                        Email = Email.Trim(),
                        Contrasena = Contrasena,
                        Rol = RolSeleccionado
                    };
                    await _apiService.CrearUsuarioAsync(dto);
                    await Shell.Current.DisplayAlert("Listo", "Usuario creado correctamente", "OK");
                }

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                _isSaving = false;
                ((Command)GuardarCommand).ChangeCanExecute();
            }
        }
    }
}
