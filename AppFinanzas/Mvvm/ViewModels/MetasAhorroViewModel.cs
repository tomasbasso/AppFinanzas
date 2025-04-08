using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Text.Json;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class MetasAhorroViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        public ObservableCollection<MetaAhorroDto> Metas { get; } = new();
        public ICommand CargarMetasCommand { get; }

        public MetasAhorroViewModel()
        {
            CargarMetasCommand = new Command(async () => await CargarMetas());
            CargarMetasCommand.Execute(null);
        }

        private async Task CargarMetas()
        {
            try
            {
                var lista = await _apiService.GetMetasAhorroAsync();
                Metas.Clear();
                foreach (var meta in lista)
                    Metas.Add(meta);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}