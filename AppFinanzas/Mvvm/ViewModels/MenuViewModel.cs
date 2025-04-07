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

        public MenuViewModel()
        {
            IrACuentasCommand = new Command(async () => await Shell.Current.GoToAsync("CuentasPage")); 
            IrATransaccionesCommand = new Command(async () => await Shell.Current.GoToAsync("TransaccionesPage"));
            IrAPresupuestosCommand = new Command(async () => await Shell.Current.GoToAsync("PresupuestosPage"));
            IrAMetasCommand = new Command(async () => await Shell.Current.GoToAsync("MetasPage"));
            IrATipoCambioCommand = new Command(async () => await Shell.Current.GoToAsync("TipoCambioPage"));
            IrAPerfilCommand = new Command(async () => await Shell.Current.GoToAsync("PerfilPage"));
        }
    }
}
