using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System.Windows.Input;
using System.Globalization;
using AppFinanzas.Data;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class NuevaTransaccionViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        public List<string> Tipos { get; } = new() { "Ingreso", "Gasto" };

        public string Tipo { get; set; } = "Gasto";
        public string Monto { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Today;

        public ICommand GuardarCommand { get; }

        public NuevaTransaccionViewModel()
        {
            GuardarCommand = new Command(async () => await GuardarAsync());
        }

        private async Task GuardarAsync()
        {
            if (!decimal.TryParse(Monto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal montoDecimal))
            {
                await Shell.Current.DisplayAlert("Error", "Monto inválido.", "OK");
                return;
            }

            var nueva = new TransaccionDto
            {
                Tipo = Tipo,
                Monto = montoDecimal,
                Descripcion = Descripcion,
                Fecha = Fecha,
                CuentaId = 1, // temporal
                CategoriaGastoId = Tipo == "Gasto" ? 1 : null,
                CategoriaIngresoId = Tipo == "Ingreso" ? 1 : null,
                UsuarioId = SesionActual.Usuario!.UsuarioId 
            };


            try
            {
                await _apiService.CrearTransaccionAsync(nueva);
                await Shell.Current.DisplayAlert("Éxito", "Transacción guardada.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
