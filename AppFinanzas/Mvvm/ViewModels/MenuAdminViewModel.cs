using System.Windows.Input;
using AppFinanzas.Mvvm.Views; // Solo si necesitás referencias directas
using AppFinanzas.Services;


namespace AppFinanzas.Mvvm.ViewModels
{
    public partial class MenuAdminViewModel : BaseViewModel
    {
        public ICommand IrACuentasCommand { get; }
        public ICommand IrATransaccionesCommand { get; }
        public ICommand IrAPresupuestosCommand { get; }
        public ICommand IrAMetasCommand { get; }
        public ICommand IrATipoCambioCommand { get; }
        public ICommand IrAPerfilCommand { get; }

        // Exclusivas admin
        public ICommand IrAGestionUsuariosCommand { get; }
        public ICommand IrAReportesCommand { get; }
        public ICommand IrAConfiguracionCommand { get; }

        public MenuAdminViewModel()
        {
            IrACuentasCommand = new Command(async () => await Shell.Current.GoToAsync("//CuentasPage"));
            IrATransaccionesCommand = new Command(async () => await Shell.Current.GoToAsync("//TransaccionesPage"));
            IrAPresupuestosCommand = new Command(async () => await Shell.Current.GoToAsync("//PresupuestosPage"));
            IrAMetasCommand = new Command(async () => await Shell.Current.GoToAsync("//MetasAhorroPage"));
            IrATipoCambioCommand = new Command(async () => await Shell.Current.GoToAsync("TipoCambioPage"));
            IrAPerfilCommand = new Command(async () => await Shell.Current.GoToAsync("//PerfilPage"));
            //ADMINS
            IrAGestionUsuariosCommand = new Command(async () => await Shell.Current.GoToAsync("//GestionUsuariosPage"));
            IrAReportesCommand = new Command(async () => await Shell.Current.GoToAsync("//ReportesPage"));
            IrAConfiguracionCommand = new Command(async () => await Shell.Current.GoToAsync("//ConfiguracionPage"));
        }
    }
}
