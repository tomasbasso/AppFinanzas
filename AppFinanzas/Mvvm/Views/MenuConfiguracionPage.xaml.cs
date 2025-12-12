using AppFinanzas.Services;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;

namespace AppFinanzas.Mvvm.Views
{
    [QueryProperty("ForAdmin", "forAdmin")]
    public partial class MenuConfiguracionPage : ContentPage
    {
        private AppFinanzas.Mvvm.ViewModels.MenuConfiguracionViewModel? Vm => BindingContext as AppFinanzas.Mvvm.ViewModels.MenuConfiguracionViewModel;

        public MenuConfiguracionPage()
        {
            InitializeComponent();
            BindingContext = new AppFinanzas.Mvvm.ViewModels.MenuConfiguracionViewModel();

            if (Vm != null)
            {
                Vm.PropertyChanged += Vm_PropertyChanged;
            }
        }

        // Parametro para avisar si se configura el menu de admin
        private string? _forAdmin;
        public string? ForAdmin
        {
            get => _forAdmin;
            set
            {
                _forAdmin = value;
                if (BindingContext is AppFinanzas.Mvvm.ViewModels.MenuConfiguracionViewModel vm)
                {
                    vm.ForAdmin = bool.TryParse(value, out var b) && b;
                    // Pongo el color inicial segun si es para admin
                    if (vm.ForAdmin)
                    {
                        vm.SelectedColor = ThemeService.AdminMenuColor;
                        Debug.WriteLine($"MenuConfiguracionPage: ForAdmin=true, initial selected color set to {vm.SelectedColor}");
                    }
                    else
                    {
                        vm.SelectedColor = ThemeService.PrimaryMenuColor;
                        Debug.WriteLine($"MenuConfiguracionPage: ForAdmin=false, initial selected color set to {vm.SelectedColor}");
                    }
                }
            }
        }

        private void Vm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppFinanzas.Mvvm.ViewModels.MenuConfiguracionViewModel.SelectedColor))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Vm?.SelectedColor != null)
                    {
                        SelectedColorLabel.Text = $"Seleccionado: {Vm.SelectedColor}";
                    }
                    else
                    {
                        SelectedColorLabel.Text = "Selecciona un color";
                    }
                });
            }
        }

        private async void OnApplyClicked(object sender, EventArgs e)
        {
            if (Vm?.ApplyCommand?.CanExecute(null) == true)
                Vm.ApplyCommand.Execute(null);
            else
                await Shell.Current.GoToAsync("..");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            if (Vm?.CancelCommand?.CanExecute(null) == true)
                Vm.CancelCommand.Execute(null);
            else
                await Shell.Current.GoToAsync("..");
        }
    }
}


