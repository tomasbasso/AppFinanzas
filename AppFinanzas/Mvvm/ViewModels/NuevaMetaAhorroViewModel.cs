using AppFinanzas.Data;
using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class NuevaMetaAhorroViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();
        private MetaAhorroDto _metaEnEdicion;
        private bool _isSaving;

        private string _nombre;
        public string Nombre
        {
            get => _nombre;
            set => SetProperty(ref _nombre, value);
        }

        private string _montoObjetivo;
        public string MontoObjetivo
        {
            get => _montoObjetivo;
            set => SetProperty(ref _montoObjetivo, value);
        }

        private DateTime _fechaLimite = DateTime.Today.AddMonths(3);
        public DateTime FechaLimite
        {
            get => _fechaLimite;
            set => SetProperty(ref _fechaLimite, value);
        }

        private decimal _progresoActual;
        public decimal ProgresoActual
        {
            get => _progresoActual;
            set => SetProperty(ref _progresoActual, value);
        }

        public ICommand GuardarCommand { get; }
        public ICommand VolverCommand { get; }

        public NuevaMetaAhorroViewModel()
        {
            GuardarCommand = new Command(async () => await GuardarAsync(), () => !_isSaving);
            VolverCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("//MenuPage/MetasAhorroPage");
            });
        }

        private async Task GuardarAsync()
        {
            if (_isSaving)
                return;

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                await Shell.Current.DisplayAlert("Error", "Debe ingresar un nombre para la meta.", "OK");
                return;
            }

            if (!decimal.TryParse(MontoObjetivo, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal montoDecimal))
            {
                await Shell.Current.DisplayAlert("Error", "Monto invalido.", "OK");
                return;
            }

            if (montoDecimal <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "El monto objetivo debe ser mayor a cero.", "OK");
                return;
            }

            if (FechaLimite < DateTime.Today)
            {
                await Shell.Current.DisplayAlert("Error", "La fecha limite debe ser igual o posterior a hoy.", "OK");
                return;
            }

            if (SesionActual.Usuario == null)
            {
                await Shell.Current.DisplayAlert("Sesion expirada", "Vuelve a iniciar sesion.", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            var meta = _metaEnEdicion != null
                ? new MetaAhorroDto
                {
                    MetaId = _metaEnEdicion.MetaId,
                    UsuarioId = _metaEnEdicion.UsuarioId
                }
                : new MetaAhorroDto
                {
                    UsuarioId = SesionActual.Usuario.UsuarioId
                };

            meta.Nombre = Nombre.Trim();
            meta.MontoObjetivo = montoDecimal;
            meta.FechaLimite = FechaLimite;
            meta.ProgresoActual = ProgresoActual;

            try
            {
                _isSaving = true;
                ((Command)GuardarCommand).ChangeCanExecute();

                if (_metaEnEdicion != null)
                {
                    await _apiService.EditarMetaAhorroAsync(meta);
                    await Shell.Current.DisplayAlert("Exito", "Meta de ahorro actualizada.", "OK");
                }
                else
                {
                    await _apiService.CrearMetaAhorroAsync(meta);
                    await Shell.Current.DisplayAlert("Exito", "Meta de ahorro guardada.", "OK");
                }

                await Shell.Current.GoToAsync("//MenuPage/MetasAhorroPage");
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

        public void CargarMetaAEditar(MetaAhorroDto meta)
        {
            _metaEnEdicion = meta;
            Nombre = meta.Nombre;
            MontoObjetivo = meta.MontoObjetivo.ToString(CultureInfo.InvariantCulture);
            FechaLimite = meta.FechaLimite;
            ProgresoActual = meta.ProgresoActual;
        }
    }
}
