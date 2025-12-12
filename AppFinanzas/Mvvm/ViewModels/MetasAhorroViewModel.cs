using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Text.Json;
using AppFinanzas.Mvvm.Views;
using AppFinanzas.Data;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class MetasAhorroViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly ApiService _apiService = new();

        public ObservableCollection<MetaAhorroDto> MetasAhorro { get; } = new();
        public ICommand CargarMetasCommand { get; }
        public ICommand IrANuevaCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand VolverCommand { get; }
        public ICommand EliminarCommand { get; }

        public MetasAhorroViewModel()
        {
            CargarMetasCommand = new Command(async () => await CargarMetas());
            CargarMetasCommand.Execute(null);
         
            IrANuevaCommand = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new NuevaMetaAhorroPage());
            });
            EditarCommand = new Command<MetaAhorroDto>(async (meta_ahorro) => await EditarMetaAhorro(meta_ahorro));
            EliminarCommand = new Command<MetaAhorroDto>(async (meta_ahorro) => await EliminarMetaAhorro(meta_ahorro));
            VolverCommand = new Command(async () => await VolverAlMenuAsync());
        }
        private async Task VolverAlMenuAsync()
        {
            var rol = SesionActual.Usuario?.Rol;

            if (string.Equals(rol, "Administrador", StringComparison.OrdinalIgnoreCase))
            {
                // Menu para admin
                await Shell.Current.GoToAsync("//MenuAdminPage");
            }
            else
            {
                // Menu de usuario (si no hay sesion va aca)
                await Shell.Current.GoToAsync("//MenuPage");
            }
        }
        private async Task EliminarMetaAhorro(MetaAhorroDto meta_ahorro)
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "¿Seguro que desea eliminar esta meta ahorro?", "Sí", "No");

            if (!confirm) return;

            try
            {
                await _apiService.EliminarMetaAhorroAsync(meta_ahorro.MetaId);
                MetasAhorro.Remove(meta_ahorro);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error al borrar", ex.Message, "OK");
            }
        }
          private async Task EditarMetaAhorro(MetaAhorroDto meta_ahorro)
        {
            await Shell.Current.GoToAsync(nameof(NuevaMetaAhorroPage), new Dictionary<string, object>
            {
                ["MetaAhorro"] = meta_ahorro
            });
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("Refrescar"))
            {
                _ = CargarMetas();
            }
        }
        private async Task CargarMetas()
        {
            try
            {
                var lista = await _apiService.GetMetasAhorroAsync();
                MetasAhorro.Clear();
                foreach (var meta in lista)
                    MetasAhorro.Add(meta);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
