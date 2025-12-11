using AppFinanzas.Mvvm.ViewModels;
using AppFinanzas.Services;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace AppFinanzas.Mvvm.Views
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
            BindingContext = new MenuViewModel();
            // Set initial background from ThemeService and subscribe to changes
            this.BackgroundColor = ThemeService.PrimaryMenuColor;
            ThemeService.OnThemeChanged += ThemeService_OnThemeChanged;
            SizeChanged += MenuPage_SizeChanged;
        }

        private void MenuPage_SizeChanged(object? sender, EventArgs e)
        {
            // Force the ScrollView container to be at least as tall as the page
            // so the inner stack can use Center alignment when there's extra space.
            if (Height > 0)
            {
                CenterContainer.MinimumHeightRequest = Height;
            }
        }

        private void ThemeService_OnThemeChanged(object? sender, EventArgs e)
        {
            // Animate the background color transition on the UI thread
            var target = ThemeService.PrimaryMenuColor;
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
                // Fallback: set immediately if animation fails for any reason
                this.BackgroundColor = toColor;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ThemeService.OnThemeChanged -= ThemeService_OnThemeChanged;
        }
    }
}
