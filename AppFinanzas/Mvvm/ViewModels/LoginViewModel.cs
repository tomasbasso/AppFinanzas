using AppFinanzas.Services;
using System;
using AppFinanzas.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new ApiService();

        public string Email { get; set; }
        public string Contrasena { get; set; }

        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await Login());
        }

        private async Task Login()
        {
            try
            {
                var usuario = await _apiService.LoginAsync(Email, Contrasena);

                SesionActual.Usuario = usuario;
                if (usuario.Rol == "Cliente")
                { 
                await Application.Current.MainPage.DisplayAlert("Éxito", $"Bienvenido {usuario.Nombre}", "OK");
                await Shell.Current.GoToAsync("//MenuPage");
            } else
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
