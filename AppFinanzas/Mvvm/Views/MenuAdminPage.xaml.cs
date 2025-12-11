using AppFinanzas.Mvvm.ViewModels;
using AppFinanzas.Services;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;


namespace AppFinanzas.Mvvm.Views;

public partial class MenuAdminPage : ContentPage
{
	public MenuAdminPage()
	{
        InitializeComponent();
        BindingContext = new MenuAdminViewModel();
        // Apply initial admin menu theme color; subscriptions moved to lifecycle methods
        this.BackgroundColor = ThemeService.AdminMenuColor;
    }

    private void ThemeService_OnThemeChanged(object? sender, EventArgs e)
    {
        var target = ThemeService.AdminMenuColor;
        System.Diagnostics.Debug.WriteLine($"MenuAdminPage: ThemeService_OnThemeChanged invoked. Target admin color: {target}");
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AnimateBackgroundColor(this.BackgroundColor ?? Microsoft.Maui.Graphics.Colors.Transparent, target, 500);
        });
    }

    private void AnimateBackgroundColor(Microsoft.Maui.Graphics.Color fromColor, Microsoft.Maui.Graphics.Color toColor, uint length = 300)
    {
        try
        {
            var animation = new Animation(v =>
            {
                var r = fromColor.Red + (toColor.Red - fromColor.Red) * v;
                var g = fromColor.Green + (toColor.Green - fromColor.Green) * v;
                var b = fromColor.Blue + (toColor.Blue - fromColor.Blue) * v;
                var a = fromColor.Alpha + (toColor.Alpha - fromColor.Alpha) * v;
                this.BackgroundColor = new Microsoft.Maui.Graphics.Color((float)r, (float)g, (float)b, (float)a);
            }, 0, 1);

            animation.Commit(this, "BackgroundColorAnimation", length: length, easing: Easing.SinInOut);
        }
        catch
        {
            this.BackgroundColor = toColor;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        System.Diagnostics.Debug.WriteLine("MenuAdminPage: OnDisappearing - unsubscribing from OnAdminThemeChanged");
        ThemeService.OnAdminThemeChanged -= ThemeService_OnThemeChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine("MenuAdminPage: OnAppearing - subscribing to OnAdminThemeChanged");
        ThemeService.OnAdminThemeChanged -= ThemeService_OnThemeChanged; // ensure not double-subscribed
        ThemeService.OnAdminThemeChanged += ThemeService_OnThemeChanged;
        // Ensure UI reflects the current admin color in case it changed while this page was not active
        var current = ThemeService.AdminMenuColor;
        System.Diagnostics.Debug.WriteLine($"MenuAdminPage: OnAppearing - animating to current AdminMenuColor {current}");
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AnimateBackgroundColor(this.BackgroundColor ?? Microsoft.Maui.Graphics.Colors.Transparent, current, 300);
        });
    }
}