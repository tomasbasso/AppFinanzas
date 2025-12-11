using AppFinanzas.Services;
using Microsoft.Maui.Graphics;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppFinanzas.Mvvm.ViewModels
{
    public class MenuConfiguracionViewModel : BaseViewModel
    {
        public ObservableCollection<Color> AvailableColors { get; } = new()
        {
            Color.FromArgb("#caa755"),
            Color.FromArgb("#2b8a3e"),
            Color.FromArgb("#1e90ff"),
            Color.FromArgb("#8a2be2"),
            Color.FromArgb("#d9534f"),
            Color.FromArgb("#333333")
        };

        private Color? _selectedColor;
        public Color? SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (SetProperty(ref _selectedColor, value))
                {
                    // Preview immediately when selection changes
                    if (value != null)
                    {
                        if (ForAdmin)
                        {
                            Debug.WriteLine($"MenuConfiguracionViewModel: ForAdmin=true, setting AdminMenuColor to {value}");
                            ThemeService.AdminMenuColor = value;
                        }
                        else
                            ThemeService.PrimaryMenuColor = value;
                    }
                }
            }
        }

    private readonly Color _originalColor;
    private readonly Color _originalAdminColor;

    public bool ForAdmin { get; set; } = false;

        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get; }

        public MenuConfiguracionViewModel()
        {
            _originalColor = ThemeService.PrimaryMenuColor;
            _originalAdminColor = ThemeService.AdminMenuColor;
            // default selected color reflects whether we're editing admin or primary
            _selectedColor = ThemeService.PrimaryMenuColor;
            ApplyCommand = new Command(async () => await ExecuteApply());
            CancelCommand = new Command(async () => await ExecuteCancel());
        }

        private async Task ExecuteApply()
        {
            // selection already previewed; navigate back to the appropriate menu
            var route = ForAdmin ? "//MenuAdminPage" : "//MenuPage";
            await Shell.Current.GoToAsync(route);
        }

        private async Task ExecuteCancel()
        {
            // revert depending on mode
            if (ForAdmin)
                ThemeService.AdminMenuColor = _originalAdminColor;
            else
                ThemeService.PrimaryMenuColor = _originalColor;

            var route = ForAdmin ? "//MenuAdminPage" : "//MenuPage";
            await Shell.Current.GoToAsync(route);
        }
    }
}
