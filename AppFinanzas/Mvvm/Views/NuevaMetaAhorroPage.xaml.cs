using AppFinanzas.Mvvm.ModelsDto;
using AppFinanzas.Mvvm.ViewModels;

namespace AppFinanzas.Mvvm.Views;

[QueryProperty(nameof(MetaAhorro), "MetaAhorro")]
public partial class NuevaMetaAhorroPage : ContentPage
{
    private readonly NuevaMetaAhorroViewModel _viewModel;
    private MetaAhorroDto _metaAhorro;

    public MetaAhorroDto MetaAhorro
    {
        get => _metaAhorro;
        set
        {
            _metaAhorro = value;
            if (value != null)
            {
                _viewModel?.CargarMetaAEditar(value);
            }
        }
    }

    public NuevaMetaAhorroPage()
    {
        InitializeComponent();
        _viewModel = new NuevaMetaAhorroViewModel();
        BindingContext = _viewModel;
    }
}
