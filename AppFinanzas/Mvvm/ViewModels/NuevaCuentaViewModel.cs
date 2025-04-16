using AppFinanzas.Data;
using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System.Globalization;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class NuevaCuentaViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();
        private CuentaDto? _cuentaExistente;

        public string Nombre { get; set; }
        public string Banco { get; set; }
        public string TipoCuenta { get; set; } = string.Empty;
        public string Saldo { get; set; }

        public ICommand GuardarCommand { get; }
        public ICommand VolverCommand { get; }

        public NuevaCuentaViewModel(CuentaDto? cuenta = null)
        {
            GuardarCommand = new Command(async () => await GuardarAsync());
            VolverCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("//MenuPage/CuentasPage");
            });

            _cuentaExistente = cuenta;

            if (_cuentaExistente != null)
            {
                Nombre = _cuentaExistente.Nombre;
                Banco = _cuentaExistente.Banco;
                TipoCuenta = _cuentaExistente.TipoCuenta;
                Saldo = _cuentaExistente.Saldo.ToString(CultureInfo.InvariantCulture);
            }
        }
        public List<string> TiposCuenta { get; } = new()
        {
            "Caja de Ahorro",
            "Caja de Ahorro en USD",
            "Cuenta Corriente"
        };
        private async Task GuardarAsync()
        {
            if (!decimal.TryParse(Saldo, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal saldoDecimal))
            {
                await Shell.Current.DisplayAlert("Error", "Saldo inválido.", "OK");
                return;
            }

            var cuenta = new CuentaDto
            {
                CuentaId = _cuentaExistente?.CuentaId ?? 0,
                Nombre = Nombre,
                Banco = Banco,
                TipoCuenta = TipoCuenta,
                Saldo = saldoDecimal,
                UsuarioId = SesionActual.Usuario!.UsuarioId
            };

            try
            {
                if (_cuentaExistente == null)
                    await _apiService.CrearCuentaAsync(cuenta);
                else
                    await _apiService.EditarCuentaAsync(cuenta);

                await Shell.Current.DisplayAlert("Éxito", "Cuenta guardada correctamente.", "OK");
                await Shell.Current.GoToAsync("//MenuPage/CuentasPage");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
