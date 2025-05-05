using AppFinanzas.Data;
using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System.Globalization;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class NuevaMetaAhorroViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        public string Nombre { get; set; }
        public string MontoObjetivo { get; set; }
        public DateTime FechaLimite { get; set; } = DateTime.Today.AddMonths(3);
        public decimal ProgresoActual { get; set; }
        public ICommand GuardarCommand { get; }
        public ICommand VolverCommand { get; }

        public NuevaMetaAhorroViewModel()
        {
            GuardarCommand = new Command(async () => await GuardarAsync());
            VolverCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("//MenuPage/MetasAhorroPage");
            });
            
        }

        private async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                await Shell.Current.DisplayAlert("Error", "Debe ingresar un nombre para la meta.", "OK");
                return;
            }

            if (!decimal.TryParse(MontoObjetivo, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal montoDecimal))
            {
                await Shell.Current.DisplayAlert("Error", "Monto inválido.", "OK");
                return;
            }

            var meta = new MetaAhorroDto
            {
                Nombre = Nombre,
                MontoObjetivo = montoDecimal,
                FechaLimite = FechaLimite,
                UsuarioId = SesionActual.Usuario!.UsuarioId,
                ProgresoActual = ProgresoActual
            };

            try
            {
                await _apiService.CrearMetaAhorroAsync(meta);
                await Shell.Current.DisplayAlert("Éxito", "Meta de ahorro guardada.", "OK");
                await Shell.Current.GoToAsync("//MenuPage/MetasAhorroPage");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
