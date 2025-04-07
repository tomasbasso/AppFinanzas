using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
            BindingContext = new MenuViewModel();
        }
    }
}
