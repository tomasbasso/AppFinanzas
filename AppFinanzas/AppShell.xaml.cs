﻿using AppFinanzas.Mvvm.Views;

namespace AppFinanzas
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(MenuPage), typeof(MenuPage));
            Routing.RegisterRoute(nameof(CuentasPage), typeof(CuentasPage));
            Routing.RegisterRoute(nameof(TransaccionesPage), typeof(TransaccionesPage));
            Routing.RegisterRoute(nameof(PresupuestosPage), typeof(PresupuestosPage));
            Routing.RegisterRoute(nameof(MetasAhorroPage), typeof(MetasAhorroPage));
            Routing.RegisterRoute(nameof(TipoCambioPage), typeof(TipoCambioPage));
            Routing.RegisterRoute(nameof(PerfilPage), typeof(PerfilPage));
            Routing.RegisterRoute(nameof(NuevaTransaccionPage), typeof(NuevaTransaccionPage));
            Routing.RegisterRoute(nameof(NuevaCuentaPage), typeof(NuevaCuentaPage));
            Routing.RegisterRoute(nameof(NuevoPresupuestoPage), typeof(NuevoPresupuestoPage));
            Routing.RegisterRoute(nameof(NuevaMetaAhorroPage), typeof(NuevaMetaAhorroPage));



        }
    }
}
