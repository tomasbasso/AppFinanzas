using Microsoft.Maui.Graphics;
using System;
using System.Diagnostics;

namespace AppFinanzas.Services
{
    public static class ThemeService
    {
        // Color base que uso en el menu principal (oscuro por defecto)
        private static Color _primaryMenuColor = Color.FromArgb("#25241f");
        public static Color PrimaryMenuColor
        {
            get => _primaryMenuColor;
            set
            {
                if (_primaryMenuColor != value)
                {
                    _primaryMenuColor = value;
                    OnThemeChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        public static event EventHandler? OnThemeChanged;

        // Color aparte para el menu de admin; el menu comun queda igual
        private static Color _adminMenuColor = Color.FromArgb("#25241f");
        public static Color AdminMenuColor
        {
            get => _adminMenuColor;
            set
            {
                if (_adminMenuColor != value)
                {
                    Debug.WriteLine($"ThemeService: AdminMenuColor changing from {_adminMenuColor} to {value}");
                    _adminMenuColor = value;
                    try
                    {
                        OnAdminThemeChanged?.Invoke(null, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"ThemeService: error invoking OnAdminThemeChanged - {ex}");
                    }
                }
            }
        }

        public static event EventHandler? OnAdminThemeChanged;
    }
}



