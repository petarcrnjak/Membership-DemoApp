using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Core;

public abstract class ViewModelComponentBase<TViewModel> : ComponentBase, IDisposable
    where TViewModel : ViewModelBase
{
    [Inject] protected IServiceProvider ServiceProvider { get; set; } = default!;

    protected TViewModel ViewModel { get; private set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Create ViewModel instance using DI
        ViewModel = CreateViewModel();

        // Initialize ViewModel (override in derived class if needed)
        InitializeViewModel(ViewModel);
    }

    protected virtual TViewModel CreateViewModel()
    {
        // Resolve ViewModel from DI container
        return (TViewModel)ActivatorUtilities.CreateInstance(ServiceProvider, typeof(TViewModel));
    }

    protected abstract void InitializeViewModel(TViewModel viewModel);

    public virtual void Dispose()
    {
        // Clean up if needed
        GC.SuppressFinalize(this);
    }
}