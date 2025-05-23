using AppFinanzas.Services;
using System;
using AppFinanzas.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using System.Windows.Input;
using AppFinanzas.Mvvm.Views; // Para Preferences

namespace AppFinanzas.Mvvm.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new ApiService();

        public string Email { get; set; }
        public string Contrasena { get; set; }

        public Command LoginCommand { get; }
        public ICommand IrARegistroCommand { get; }
        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await Login());
            IrARegistroCommand = new Command(async () => await Shell.Current.GoToAsync("//RegistroUsuarioPage"));


        }
        private async Task Login()
        {
            try
            {
                var loginResponse = await _apiService.LoginAsync(Email, Contrasena);

                // Guardar en sesión en memoria
                SesionActual.Token = loginResponse.Token;
                SesionActual.Usuario = loginResponse.Usuario;

                // Guardar en Preferences (persistente)
                Preferences.Default.Set("jwt", loginResponse.Token);
                Preferences.Default.Set("usuarioEmail", loginResponse.Usuario.Email);

                var usuario = loginResponse.Usuario;

                if (usuario.Rol == "Cliente")
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", $"Bienvenido {usuario.Nombre}", "OK");
                    await Shell.Current.GoToAsync("//MenuPage");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", $"Bienvenido {usuario.Nombre}", "OK");
                    await Shell.Current.GoToAsync("//MenuAdminPage");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
