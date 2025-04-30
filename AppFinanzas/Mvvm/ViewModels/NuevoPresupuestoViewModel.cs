using AppFinanzas.Data;
using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;


namespace AppFinanzas.Mvvm.ViewModels
{
    [QueryProperty(nameof(Presupuesto), "Presupuesto")]
    public class NuevoPresupuestoViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        public ObservableCollection<CategoriaGastoDto> Categorias { get; } = new();
        public List<string> Meses { get; } = new()
        {
            "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
        };

        // Propiedades públicas
        public CategoriaGastoDto CategoriaSeleccionada { get; set; }
        public string MontoLimite { get; set; }
        public string MesSeleccionado { get; set; }
        public string Anio { get; set; }

        public PresupuestoDto Presupuesto { get; set; }

        public ICommand GuardarCommand { get; }
        public ICommand VolverCommand { get; }

        public NuevoPresupuestoViewModel()
        {
            GuardarCommand = new Command(async () => await GuardarAsync());
            VolverCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("//MenuPage/PresupuestosPage");
            });

            _ = CargarCategoriasAsync();
        }
        

        private async Task CargarCategoriasAsync()
        {
            var lista = await _apiService.GetCategoriasGastoAsync();
            Categorias.Clear();
            foreach (var cat in lista)
                Categorias.Add(cat);

            // Si se está editando y ya vino el presupuesto, cargar datos
            if (Presupuesto != null)
                CargarDesdePresupuesto();
        }

        private void CargarDesdePresupuesto()
        {
            if (Presupuesto == null) return;

            MontoLimite = Presupuesto.MontoLimite.ToString(CultureInfo.InvariantCulture);
            Anio = Presupuesto.Año.ToString();
            MesSeleccionado = Meses[Presupuesto.Mes - 1]; // Convertir 1 a "Enero"
            CategoriaSeleccionada = Categorias.FirstOrDefault(c => c.CategoriaGastoId == Presupuesto.CategoriaGastoId);

            OnPropertyChanged(nameof(MontoLimite));
            OnPropertyChanged(nameof(Anio));
            OnPropertyChanged(nameof(MesSeleccionado));
            OnPropertyChanged(nameof(CategoriaSeleccionada));
        }

        private async Task GuardarAsync()
        {
            if (CategoriaSeleccionada == null)
            {
                await Shell.Current.DisplayAlert("Error", "Debe seleccionar una categoría.", "OK");
                return;
            }

            if (!decimal.TryParse(MontoLimite, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal montoDecimal))
            {
                await Shell.Current.DisplayAlert("Error", "Monto inválido.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(MesSeleccionado))
            {
                await Shell.Current.DisplayAlert("Error", "Debe seleccionar un mes.", "OK");
                return;
            }

            if (!int.TryParse(Anio, out int anioNum))
            {
                await Shell.Current.DisplayAlert("Error", "Año inválido.", "OK");
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
                UsuarioId = SesionActual.Usuario!.UsuarioId
            };

            try
            {
                if (Presupuesto == null || Presupuesto.PresupuestoId == 0)
                    await _apiService.CrearPresupuestoAsync(dto);
                else
                    await _apiService.EditarPresupuestoAsync(dto);

                await Shell.Current.DisplayAlert("Éxito", "Presupuesto guardado.", "OK");
                await Shell.Current.GoToAsync("//MenuPage/PresupuestosPage");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
