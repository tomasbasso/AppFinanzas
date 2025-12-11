using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AppFinanzas.Data;
using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new ApiService();
 
        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
 
        private string _contrasena = string.Empty;
        public string Contrasena
        {
            get => _contrasena;
            set => SetProperty(ref _contrasena, value);
        }
 
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                    OnPropertyChanged(nameof(IsNotLoading));
            }
        }
 
        public bool IsNotLoading => !IsLoading;
 
        public ICommand LoginCommand { get; }
        public ICommand IrARegistroCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await Login());
            IrARegistroCommand = new Command(async () => await Shell.Current.GoToAsync("//RegistroUsuarioPage"));
        }

        private async Task Login()
        {
            if (IsLoading)
                return;

            try
            {
                IsLoading = true;

                var loginResponse = await _apiService.LoginAsync(Email, Contrasena);

                SesionActual.Token = loginResponse.Token;
                SesionActual.Usuario = loginResponse.Usuario;

                // Guardar token de forma segura (SecureStorage) y fallback a Preferences
                if (!string.IsNullOrEmpty(loginResponse?.Token))
                {
                    try { await SecureStorage.SetAsync("jwt", loginResponse.Token); }
                    catch { try { Preferences.Default.Set("jwt", loginResponse.Token); } catch { } }
                }

                if (!string.IsNullOrEmpty(loginResponse?.Usuario?.Email))
                {
                    try { Preferences.Default.Set("usuarioEmail", loginResponse.Usuario.Email); } catch { }
                }

                var usuario = loginResponse?.Usuario;
                var page = Application.Current?.MainPage;
                if (usuario != null && string.Equals(usuario.Rol, "Cliente", StringComparison.OrdinalIgnoreCase))
                {
                    if (page != null) await page.DisplayAlert("Éxito", $"Bienvenido {usuario.Nombre}", "OK");
                    await Shell.Current.GoToAsync("//MenuPage");
                }
                else
                {
                    if (page != null) await page.DisplayAlert("Éxito", $"Bienvenido {usuario?.Nombre}", "OK");
                    await Shell.Current.GoToAsync("//MenuAdminPage");
                }
            }
            catch (Exception ex)
            {
                var pageErr = Application.Current?.MainPage;
                if (pageErr != null) await pageErr.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
