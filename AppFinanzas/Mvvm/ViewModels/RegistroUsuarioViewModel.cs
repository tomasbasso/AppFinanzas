using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class RegistroUsuarioViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Contrasena { get; set; }

        public ObservableCollection<string> Roles { get; set; } = new ObservableCollection<string> { "Cliente", "Administrador" };
        public string RolSeleccionado { get; set; }

        public ICommand RegistrarCommand { get; }
        public ICommand VolverCommand { get; }

        public RegistroUsuarioViewModel()
        {
            RegistrarCommand = new Command(async () => await RegistrarUsuario());
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync("//LoginPage"));

        }

        private async Task RegistrarUsuario()
        {
            try
            {
                var dto = new UsuarioRegistroDto
                {
                    Nombre = Nombre,
                    Email = Email,
                    Contrasena = Contrasena,
                    Rol = RolSeleccionado
                };
                await _apiService.CrearUsuarioAsync(dto);
                await App.Current.MainPage.DisplayAlert("¡Éxito!", "Usuario creado correctamente, Ahora puede volver y loguearse correctamente", "OK");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}