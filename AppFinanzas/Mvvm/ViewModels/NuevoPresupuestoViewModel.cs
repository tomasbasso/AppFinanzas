using AppFinanzas.Data;
using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    [QueryProperty(nameof(Presupuesto), "Presupuesto")]
    public class NuevoPresupuestoViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();
        private bool _isSaving;

        public ObservableCollection<CategoriaGastoDto> Categorias { get; } = new();
        public List<string> Meses { get; } = new()
        {
            "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
        };

        public CategoriaGastoDto CategoriaSeleccionada { get; set; }
        public string MontoLimite { get; set; }
        public string MesSeleccionado { get; set; }
        public string Anio { get; set; }

        public PresupuestoDto Presupuesto { get; set; }
        public ICommand VolverCommand { get; }
        public ICommand GuardarCommand { get; }

        public NuevoPresupuestoViewModel()
        {
            GuardarCommand = new Command(async () => await GuardarAsync(), () => !_isSaving);
            VolverCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("//MenuPage/PresupuestosPage");
            });

            _ = CargarCategoriasAsync();
        }

        private async Task CargarCategoriasAsync()
        {
            try
            {
                var lista = await _apiService.GetCategoriasGastoAsync();
                Categorias.Clear();
                foreach (var cat in lista)
                    Categorias.Add(cat);

                if (Presupuesto != null)
                    CargarDesdePresupuesto();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudieron cargar categorias: {ex.Message}", "OK");
            }
        }

        private void CargarDesdePresupuesto()
        {
            if (Presupuesto == null) return;

            MontoLimite = Presupuesto.MontoLimite.ToString(CultureInfo.InvariantCulture);
            Anio = Presupuesto.Año.ToString();
            MesSeleccionado = Meses[Presupuesto.Mes - 1];
            CategoriaSeleccionada = Categorias.FirstOrDefault(c => c.CategoriaGastoId == Presupuesto.CategoriaGastoId);

            OnPropertyChanged(nameof(MontoLimite));
            OnPropertyChanged(nameof(Anio));
            OnPropertyChanged(nameof(MesSeleccionado));
            OnPropertyChanged(nameof(CategoriaSeleccionada));
        }

        private async Task GuardarAsync()
        {
            if (_isSaving)
                return;

            if (CategoriaSeleccionada == null)
            {
                await Shell.Current.DisplayAlert("Error", "Debe seleccionar una categoria.", "OK");
                return;
            }

            if (!decimal.TryParse(MontoLimite, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal montoDecimal))
            {
                await Shell.Current.DisplayAlert("Error", "Monto invalido.", "OK");
                return;
            }

            if (montoDecimal <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "El monto debe ser mayor a cero.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(MesSeleccionado))
            {
                await Shell.Current.DisplayAlert("Error", "Debe seleccionar un mes.", "OK");
                return;
            }

            if (!int.TryParse(Anio, out int anioNum))
            {
                await Shell.Current.DisplayAlert("Error", "Año invalido.", "OK");
                return;
            }

            if (anioNum < DateTime.Today.Year - 5 || anioNum > DateTime.Today.Year + 5)
            {
                await Shell.Current.DisplayAlert("Error", "El año ingresado no es válido.", "OK");
                return;
            }

            if (SesionActual.Usuario == null)
            {
                await Shell.Current.DisplayAlert("Sesion expirada", "Vuelve a iniciar sesion.", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            var dto = new PresupuestoDto
            {
                PresupuestoId = Presupuesto?.PresupuestoId ?? 0,
                CategoriaGastoId = CategoriaSeleccionada.CategoriaGastoId,
                NombreCategoria = CategoriaSeleccionada.Nombre,
                MontoLimite = montoDecimal,
                Mes = Meses.IndexOf(MesSeleccionado) + 1,
                Año = anioNum,
                UsuarioId = SesionActual.Usuario.UsuarioId
            };

            try
            {
                _isSaving = true;
                ((Command)GuardarCommand).ChangeCanExecute();

                if (Presupuesto == null || Presupuesto.PresupuestoId == 0)
                    await _apiService.CrearPresupuestoAsync(dto);
                else
                    await _apiService.EditarPresupuestoAsync(dto);

                await Shell.Current.DisplayAlert("Exito", "Presupuesto guardado.", "OK");
                await Shell.Current.GoToAsync("//MenuPage/PresupuestosPage");
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
