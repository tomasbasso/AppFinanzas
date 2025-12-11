using AppFinanzas.Mvvm.Views;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        public ICommand IrACuentasCommand { get; }
        public ICommand IrATransaccionesCommand { get; }
        public ICommand IrAPresupuestosCommand { get; }
        public ICommand IrAMetasCommand { get; }
        public ICommand IrATipoCambioCommand { get; }
        public ICommand IrAPerfilCommand { get; }
        public ICommand IrAConfiguracionCommand { get; }

        public MenuViewModel()
        {
            IrACuentasCommand = new Command(async () => await Shell.Current.GoToAsync("CuentasPage")); 
            IrATransaccionesCommand = new Command(async () => await Shell.Current.GoToAsync("TransaccionesPage"));
            IrAPresupuestosCommand = new Command(async () => await Shell.Current.GoToAsync("PresupuestosPage"));
            IrAMetasCommand = new Command(async () => await Shell.Current.GoToAsync("MetasAhorroPage"));
            IrATipoCambioCommand = new Command(async () => await Shell.Current.GoToAsync("TipoCambioPage"));
            IrAPerfilCommand = new Command(async () => await Shell.Current.GoToAsync("PerfilPage"));
            // Use the registered Shell route name. Using the absolute '//' ensures navigation to that route.
            IrAConfiguracionCommand = new Command(async () => await Shell.Current.GoToAsync("//MenuConfiguracionPage?forAdmin=false"));
        }
    }
}
