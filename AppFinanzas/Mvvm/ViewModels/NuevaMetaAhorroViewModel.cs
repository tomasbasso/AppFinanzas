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
        private MetaAhorroDto _metaEnEdicion;

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
                await Shell.Current.DisplayAlert("Error", "Monto invalido.", "OK");
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
                    UsuarioId = SesionActual.Usuario!.UsuarioId
                };

            meta.Nombre = Nombre;
            meta.MontoObjetivo = montoDecimal;
            meta.FechaLimite = FechaLimite;
            meta.ProgresoActual = ProgresoActual;

            try
            {
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
