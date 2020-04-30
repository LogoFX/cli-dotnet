using Microsoft.Extensions.DependencyInjection;
using Solid.Core;
using Solid.Practices.Composition;

namespace Common.Bootstrapping
{
    public static class ServiceCollectionExtensions
    {
        public static void UseDynamicLoad(this IServiceCollection serviceCollection)
        {
            AssemblyLoader.LoadAssembliesFromPaths = DynamicAssemblyLoader.Get;
        }
    }

    public static class BootstrappingExtensions
    {
        public static void UseDynamicLoad(this IInitializable initializable)
        {
            AssemblyLoader.LoadAssembliesFromPaths = DynamicAssemblyLoader.Get;
        }
    }
}
