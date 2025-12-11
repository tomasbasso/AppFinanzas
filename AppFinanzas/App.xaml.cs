using AppFinanzas.Data;
using Microsoft.Maui.Storage;
using System;

namespace AppFinanzas
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Intentar cargar token seguro al inicio (SecureStorage) y fallback a Preferences
            try
            {
                var token = SecureStorage.GetAsync("jwt").GetAwaiter().GetResult();
                if (!string.IsNullOrEmpty(token))
                    SesionActual.Token = token;
                else
                    SesionActual.Token = Preferences.Default.Get("jwt", string.Empty);
            }
            catch (Exception)
            {
                SesionActual.Token = Preferences.Default.Get("jwt", string.Empty);
            }
            MainPage = new AppShell();
        }
    }
}
