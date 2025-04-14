using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Mvvm.ViewModels;
using System.Transactions;

namespace AppFinanzas.Mvvm.Views
{
    [QueryProperty(nameof(Transaccion), "Transaccion")]
    public partial class NuevaTransaccionPage : ContentPage
    {
        public TransaccionDto Transaccion { get; set; }

        public NuevaTransaccionPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = new NuevaTransaccionViewModel(Transaccion);
        }
    }
}
