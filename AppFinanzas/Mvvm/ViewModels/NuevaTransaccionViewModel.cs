using AppFinanzas.Data;
using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class NuevaTransaccionViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();
        private readonly TransaccionDto? _transaccionExistente;
        private bool _isSaving;

        private List<CuentaDto> _cuentas = new();
        private CuentaDto? _cuentaSeleccionada;
        private string _tipo = "Gasto";
        private string _monto = string.Empty;
        private string _descripcion = string.Empty;
        private DateTime _fecha = DateTime.Today;
        private List<CategoriaGastoDto> _categoriasGasto = new();
        private List<CategoriaIngresoDto> _categoriasIngreso = new();
        private CategoriaGastoDto? _categoriaGastoSeleccionada;
        private CategoriaIngresoDto? _categoriaIngresoSeleccionada;

        public List<string> Tipos { get; } = new() { "Ingreso", "Gasto" };

        public List<CuentaDto> Cuentas
        {
            get => _cuentas;
            set => SetProperty(ref _cuentas, value);
        }

        public CuentaDto? CuentaSeleccionada
        {
            get => _cuentaSeleccionada;
            set => SetProperty(ref _cuentaSeleccionada, value);
        }

        public string Tipo
        {
            get => _tipo;
            set
            {
                if (SetProperty(ref _tipo, value))
                {
                    OnPropertyChanged(nameof(EsGasto));
                    OnPropertyChanged(nameof(EsIngreso));
                    if (EsGasto && CategoriaGastoSeleccionada == null)
                        CategoriaGastoSeleccionada = CategoriasGasto.FirstOrDefault();
                    if (EsIngreso && CategoriaIngresoSeleccionada == null)
                        CategoriaIngresoSeleccionada = CategoriasIngreso.FirstOrDefault();
                }
            }
        }

        public bool EsGasto => string.Equals(Tipo, "Gasto", StringComparison.OrdinalIgnoreCase);
        public bool EsIngreso => string.Equals(Tipo, "Ingreso", StringComparison.OrdinalIgnoreCase);

        public string Monto
        {
            get => _monto;
            set => SetProperty(ref _monto, value);
        }

        public string Descripcion
        {
            get => _descripcion;
            set => SetProperty(ref _descripcion, value);
        }

        public DateTime Fecha
        {
            get => _fecha;
            set => SetProperty(ref _fecha, value);
        }

        public List<CategoriaGastoDto> CategoriasGasto
        {
            get => _categoriasGasto;
            set => SetProperty(ref _categoriasGasto, value);
        }

        public List<CategoriaIngresoDto> CategoriasIngreso
        {
            get => _categoriasIngreso;
            set => SetProperty(ref _categoriasIngreso, value);
        }

        public CategoriaGastoDto? CategoriaGastoSeleccionada
        {
            get => _categoriaGastoSeleccionada;
            set => SetProperty(ref _categoriaGastoSeleccionada, value);
        }

        public CategoriaIngresoDto? CategoriaIngresoSeleccionada
        {
            get => _categoriaIngresoSeleccionada;
            set => SetProperty(ref _categoriaIngresoSeleccionada, value);
        }

        public ICommand GuardarCommand { get; }
        public ICommand VolverCommand { get; }

        public NuevaTransaccionViewModel(TransaccionDto? transaccion = null)
        {
            _transaccionExistente = transaccion;

            GuardarCommand = new Command(async () => await GuardarAsync(), () => !_isSaving);
            VolverCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("//MenuPage/TransaccionesPage");
            });

            if (_transaccionExistente != null)
            {
                Tipo = _transaccionExistente.Tipo;
                Monto = _transaccionExistente.Monto.ToString(CultureInfo.InvariantCulture);
                Descripcion = _transaccionExistente.Descripcion;
                Fecha = _transaccionExistente.Fecha;
                CuentaSeleccionada = new CuentaDto { CuentaId = _transaccionExistente.CuentaId };
                if (_transaccionExistente.CategoriaGastoId.HasValue)
                    CategoriaGastoSeleccionada = new CategoriaGastoDto { CategoriaGastoId = _transaccionExistente.CategoriaGastoId.Value };
                if (_transaccionExistente.CategoriaIngresoId.HasValue)
                    CategoriaIngresoSeleccionada = new CategoriaIngresoDto { CategoriaIngresoId = _transaccionExistente.CategoriaIngresoId.Value };
            }

            _ = CargarDatosInicialesAsync();
        }

        private async Task CargarDatosInicialesAsync()
        {
            try
            {
                var cuentasTask = _apiService.GetCuentasAsync();
                var categoriasGastoTask = _apiService.GetCategoriasGastoAsync();
                var categoriasIngresoTask = _apiService.GetCategoriasIngresoAsync();

                await Task.WhenAll(cuentasTask, categoriasGastoTask, categoriasIngresoTask);

                Cuentas = cuentasTask.Result;
                CategoriasGasto = categoriasGastoTask.Result;
                CategoriasIngreso = categoriasIngresoTask.Result;

                if (_transaccionExistente == null)
                {
                    CuentaSeleccionada = Cuentas.FirstOrDefault();
                    if (EsGasto)
                        CategoriaGastoSeleccionada = CategoriasGasto.FirstOrDefault();
                    if (EsIngreso)
                        CategoriaIngresoSeleccionada = CategoriasIngreso.FirstOrDefault();
                }
                else
                {
                    CuentaSeleccionada = Cuentas.FirstOrDefault(c => c.CuentaId == _transaccionExistente.CuentaId) ?? Cuentas.FirstOrDefault();
                    if (_transaccionExistente.CategoriaGastoId.HasValue)
                        CategoriaGastoSeleccionada = CategoriasGasto.FirstOrDefault(c => c.CategoriaGastoId == _transaccionExistente.CategoriaGastoId) ?? CategoriasGasto.FirstOrDefault();
                    if (_transaccionExistente.CategoriaIngresoId.HasValue)
                        CategoriaIngresoSeleccionada = CategoriasIngreso.FirstOrDefault(c => c.CategoriaIngresoId == _transaccionExistente.CategoriaIngresoId) ?? CategoriasIngreso.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudieron cargar los datos: {ex.Message}", "OK");
            }
        }

        private async Task GuardarAsync()
        {
            if (_isSaving)
                return;

            if (!decimal.TryParse(Monto, NumberStyles.Any, CultureInfo.InvariantCulture, out var montoDecimal))
            {
                await Shell.Current.DisplayAlert("Error", "Monto invalido.", "OK");
                return;
            }

            if (montoDecimal <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "El monto debe ser mayor a cero.", "OK");
                return;
            }

            if (Fecha > DateTime.Today.AddDays(1))
            {
                await Shell.Current.DisplayAlert("Error", "La fecha no puede ser futura.", "OK");
                return;
            }

            if (CuentaSeleccionada == null)
            {
                await Shell.Current.DisplayAlert("Error", "Debe seleccionar una cuenta.", "OK");
                return;
            }

            if (EsGasto && (CategoriaGastoSeleccionada == null || CategoriasGasto.Count == 0))
            {
                await Shell.Current.DisplayAlert("Error", "Debe seleccionar una categoria de gasto.", "OK");
                return;
            }

            if (EsIngreso && (CategoriaIngresoSeleccionada == null || CategoriasIngreso.Count == 0))
            {
                await Shell.Current.DisplayAlert("Error", "Debe seleccionar una categoria de ingreso.", "OK");
                return;
            }

            if (SesionActual.Usuario == null)
            {
                await Shell.Current.DisplayAlert("Sesion expirada", "Vuelve a iniciar sesion.", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            if (EsGasto)
            {
                var saldoDisponible = CuentaSeleccionada.SaldoActual != 0 ? CuentaSeleccionada.SaldoActual : CuentaSeleccionada.Saldo;
                if (montoDecimal > saldoDisponible)
                {
                    await Shell.Current.DisplayAlert("Saldo insuficiente", $"La cuenta seleccionada tiene {saldoDisponible:C} disponibles.", "OK");
                    return;
                }
            }

            var transaccion = new TransaccionDto
            {
                TransaccionId = _transaccionExistente?.TransaccionId ?? 0,
                Tipo = Tipo,
                Monto = montoDecimal,
                Descripcion = Descripcion,
                Fecha = Fecha,
                CuentaId = CuentaSeleccionada.CuentaId,
                CategoriaGastoId = EsGasto ? CategoriaGastoSeleccionada?.CategoriaGastoId : null,
                CategoriaIngresoId = EsIngreso ? CategoriaIngresoSeleccionada?.CategoriaIngresoId : null,
                UsuarioId = SesionActual.Usuario.UsuarioId
            };

            try
            {
                _isSaving = true;
                ((Command)GuardarCommand).ChangeCanExecute();

                if (_transaccionExistente == null)
                    await _apiService.CrearTransaccionAsync(transaccion);
                else
                    await _apiService.EditarTransaccionAsync(transaccion);

                await Shell.Current.DisplayAlert("Exito", "Transaccion guardada.", "OK");
                await Shell.Current.GoToAsync("//MenuPage/TransaccionesPage");
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
