using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Core;
using System.Reflection;

namespace Presentation;

public static class StartupExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection services, params Assembly[] additionalAssemblies)
    {
        var assemblies = new List<Assembly>
        {
            typeof(StartupExtensions).Assembly
        };
        assemblies.AddRange(additionalAssemblies);

        foreach (var a in assemblies)
        {
            foreach (var type in a.GetExportedTypes().Where(x => typeof(ViewModelBase).IsAssignableFrom(x) && !x.IsAbstract))
            {
                services.AddTransient(type);
            }
        }

        return services
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
    }
}