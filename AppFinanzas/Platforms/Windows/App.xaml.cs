using Microsoft.UI.Xaml;

// Para saber mas sobre WinUI y la estructura del proyecto: http://aka.ms/winui-project-info.

namespace AppFinanzas.WinUI
{
    /// <summary>
    /// Agrega comportamiento propio de la app encima de la clase Application por defecto.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Inicializa la instancia unica de la app; es similar a main() o WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

}
