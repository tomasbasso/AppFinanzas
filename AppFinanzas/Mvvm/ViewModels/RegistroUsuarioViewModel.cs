using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class RegistroUsuarioViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();
        private bool _isSaving;

        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Contrasena { get; set; }

        public ObservableCollection<string> Roles { get; set; } = new ObservableCollection<string> { "Cliente", "Administrador" };
        public string RolSeleccionado { get; set; }

        public ICommand RegistrarCommand { get; }
        public ICommand VolverCommand { get; }

        public RegistroUsuarioViewModel()
        {
            RegistrarCommand = new Command(async () => await RegistrarUsuario(), () => !_isSaving);
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync("//LoginPage"));
        }

        private async Task RegistrarUsuario()
        {
            if (_isSaving)
                return;

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                await Shell.Current.DisplayAlert("Error", "El nombre es obligatorio.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Email) || !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                await Shell.Current.DisplayAlert("Error", "Email invalido.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Contrasena) || Contrasena.Length < 6)
            {
                await Shell.Current.DisplayAlert("Error", "La contraseña debe tener al menos 6 caracteres.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(RolSeleccionado))
            {
                await Shell.Current.DisplayAlert("Error", "Debe seleccionar un rol.", "OK");
                return;
            }

            try
            {
                _isSaving = true;
                ((Command)RegistrarCommand).ChangeCanExecute();

                var dto = new UsuarioRegistroDto
                {
                    Nombre = Nombre.Trim(),
                    Email = Email.Trim(),
                    Contrasena = Contrasena,
                    Rol = RolSeleccionado
                };

                await _apiService.CrearUsuarioAsync(dto);
                await App.Current.MainPage.DisplayAlert("Exito", "Usuario creado correctamente, ahora puede iniciar sesion.", "OK");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                _isSaving = false;
                ((Command)RegistrarCommand).ChangeCanExecute();
            }
        }
    }
}
