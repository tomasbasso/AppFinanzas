using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class CuentasViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        public ObservableCollection<CuentaDto> Cuentas { get; set; }
        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        public CuentasViewModel()
        {
            _apiService = new ApiService(); // o inyectalo si usás DI
            Cuentas = new ObservableCollection<CuentaDto>();
            LoadCuentasCommand = new Command(async () => await LoadCuentasAsync());

            LoadCuentasCommand.Execute(null);
        }

        public Command LoadCuentasCommand { get; }

        private async Task LoadCuentasAsync()
        {
            IsBusy = true;

            try
            {
                var cuentas = await _apiService.GetCuentasAsync();
                Cuentas.Clear();

                foreach (var cuenta in cuentas)
                    Cuentas.Add(cuenta);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}