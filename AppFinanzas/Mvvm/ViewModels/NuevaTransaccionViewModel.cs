using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System.Windows.Input;
using System.Globalization;
using AppFinanzas.Data;
using System.Transactions;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class NuevaTransaccionViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        public List<string> Tipos { get; } = new() { "Ingreso", "Gasto" };
        public ICommand VolverCommand { get; }

        private TransaccionDto? _transaccionExistente;
        public List<CuentaDto> Cuentas { get; set; } = new();
        public CuentaDto CuentaSeleccionada { get; set; }
        public string Tipo { get; set; } = "Gasto";
        public string Monto { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Today;

        public ICommand GuardarCommand { get; }

        public NuevaTransaccionViewModel(TransaccionDto? transaccion = null)
        {
            GuardarCommand = new Command(async () => await GuardarAsync());
            _ = CargarCuentasAsync();
            VolverCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("//MenuPage/TransaccionesPage");
            });


            _transaccionExistente = transaccion;

            Tipos = new() { "Ingreso", "Gasto" };
            Fecha = DateTime.Today;

            if (_transaccionExistente != null)
            {
                Tipo = _transaccionExistente.Tipo;
                Monto = _transaccionExistente.Monto.ToString();
                Descripcion = _transaccionExistente.Descripcion;
                Fecha = _transaccionExistente.Fecha;
                CuentaSeleccionada = new CuentaDto { CuentaId = _transaccionExistente.CuentaId }; // Asumimos lista cargada luego
            }

        }
        private async Task CargarCuentasAsync()
        {
            var cuentas = await _apiService.GetCuentasAsync();
            Cuentas = cuentas;
            CuentaSeleccionada = Cuentas.FirstOrDefault(); // selecciona la primera por defecto
            OnPropertyChanged(nameof(Cuentas));
            OnPropertyChanged(nameof(CuentaSeleccionada));
        }
        private async Task VolverAsync()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        private async Task GuardarAsync()
        {
            if (!decimal.TryParse(Monto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal montoDecimal))
            {
                await Shell.Current.DisplayAlert("Error", "Monto inválido.", "OK");
                return;
            }

            var transaccion = new TransaccionDto
            {
                TransaccionId = _transaccionExistente?.TransaccionId ?? 0,
                Tipo = Tipo,
                Monto = montoDecimal,
                Descripcion = Descripcion,
                Fecha = Fecha,
                CuentaId = CuentaSeleccionada?.CuentaId ?? 1,
                CategoriaGastoId = Tipo == "Gasto" ? 1 : null,
                CategoriaIngresoId = Tipo == "Ingreso" ? 1 : null,
                UsuarioId = SesionActual.Usuario!.UsuarioId
            };

            try
            {
                if (_transaccionExistente == null)
                    await _apiService.CrearTransaccionAsync(transaccion);
                else
                    await _apiService.EditarTransaccionAsync(transaccion);

                await Shell.Current.DisplayAlert("Éxito", "Transacción guardada.", "OK");
                await Shell.Current.GoToAsync("//MenuPage/TransaccionesPage");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
