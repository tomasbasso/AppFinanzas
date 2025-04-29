using AppFinanzas.Data;
using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class NuevoPresupuestoViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

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

        public ICommand GuardarCommand { get; }
        public ICommand VolverCommand { get; }

        public NuevoPresupuestoViewModel()
        {
            GuardarCommand = new Command(async () => await GuardarAsync());
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

            _ = CargarCategoriasAsync();
        }

        private async Task CargarCategoriasAsync()
        {
            var lista = await _apiService.GetCategoriasGastoAsync();
            Categorias.Clear();
            foreach (var cat in lista)
                Categorias.Add(cat);

            CategoriaSeleccionada = Categorias.FirstOrDefault();
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
                CategoriaGastoId = CategoriaSeleccionada.CategoriaGastoId,
                NombreCategoria = CategoriaSeleccionada.Nombre,
                MontoLimite = montoDecimal,
                Mes = Meses.IndexOf(MesSeleccionado) + 1, // Enero = 1, etc.
                Año = anioNum,
                UsuarioId = SesionActual.Usuario!.UsuarioId
            };

            try
            {
                await _apiService.CrearPresupuestoAsync(dto);
                await Shell.Current.DisplayAlert("Éxito", "Presupuesto guardado.", "OK");
                await Shell.Current.GoToAsync("//PresupuestosPage");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
