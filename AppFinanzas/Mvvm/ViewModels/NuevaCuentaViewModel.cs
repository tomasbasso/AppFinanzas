using AppFinanzas.Data;
using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class NuevaCuentaViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();
        private readonly CuentaDto? _cuentaExistente;
        private bool _isSaving;

        public string Nombre { get; set; }
        public string Banco { get; set; }
        public string TipoCuenta { get; set; } = string.Empty;
        public string Saldo { get; set; }

        public ICommand VolverCommand { get; }
        public ICommand GuardarCommand { get; }

        public List<string> TiposCuenta { get; } = new()
        {
            "Caja de Ahorro",
            "Caja de Ahorro en USD",
            "Cuenta Corriente"
        };

        public NuevaCuentaViewModel(CuentaDto? cuenta = null)
        {
            _cuentaExistente = cuenta;

            GuardarCommand = new Command(async () => await GuardarAsync(), () => !_isSaving);
            VolverCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("//MenuPage/CuentasPage");
            });

            if (_cuentaExistente != null)
            {
                Nombre = _cuentaExistente.Nombre;
                Banco = _cuentaExistente.Banco;
                TipoCuenta = _cuentaExistente.TipoCuenta;
                var saldoBase = _cuentaExistente.SaldoActual != 0 ? _cuentaExistente.SaldoActual : _cuentaExistente.Saldo;
                Saldo = saldoBase.ToString(CultureInfo.InvariantCulture);
            }
        }

        private async Task GuardarAsync()
        {
            if (_isSaving)
                return;

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                await Shell.Current.DisplayAlert("Error", "El nombre es obligatorio.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Banco))
            {
                await Shell.Current.DisplayAlert("Error", "El banco es obligatorio.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(TipoCuenta))
            {
                await Shell.Current.DisplayAlert("Error", "Debe seleccionar un tipo de cuenta.", "OK");
                return;
            }

            var saldoTexto = (Saldo ?? string.Empty).Trim()
                                                      .Replace(" ", string.Empty)
                                                      .Replace("$", string.Empty)
                                                      .Replace("€", string.Empty)
                                                      .Replace(",", ".");

            var lastDot = saldoTexto.LastIndexOf('.');
            if (lastDot > -1)
            {
                var entero = saldoTexto[..lastDot].Replace(".", string.Empty);
                var decimales = saldoTexto[(lastDot + 1)..].Replace(".", string.Empty);
                saldoTexto = $"{entero}.{decimales}";
            }

            if (!decimal.TryParse(saldoTexto, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal saldoDecimal))
            {
                await Shell.Current.DisplayAlert("Error", "Saldo invalido.", "OK");
                return;
            }

            if (saldoDecimal < 0)
            {
                await Shell.Current.DisplayAlert("Error", "El saldo no puede ser negativo.", "OK");
                return;
            }

            if (SesionActual.Usuario == null)
            {
                await Shell.Current.DisplayAlert("Sesion expirada", "Vuelve a iniciar sesion.", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            var cuenta = new CuentaDto
            {
                CuentaId = _cuentaExistente?.CuentaId ?? 0,
                Nombre = Nombre.Trim(),
                Banco = Banco.Trim(),
                TipoCuenta = TipoCuenta,
                Saldo = saldoDecimal,
                SaldoActual = saldoDecimal,
                UsuarioId = SesionActual.Usuario.UsuarioId
            };

            try
            {
                _isSaving = true;
                ((Command)GuardarCommand).ChangeCanExecute();

                if (_cuentaExistente == null)
                    await _apiService.CrearCuentaAsync(cuenta);
                else
                    await _apiService.EditarCuentaAsync(cuenta);

                await Shell.Current.DisplayAlert("Exito", "Cuenta guardada correctamente.", "OK");
                await Shell.Current.GoToAsync("//MenuPage/CuentasPage");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                _isSaving = false;
                ((Command)GuardarCommand).ChangeCanExecute();
            }
        }
    }
}
