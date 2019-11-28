using Prism.Ioc;
using Unity;

namespace FlyApp.ViewModels
{
    public static class Bootstrapper
    {
        public static IUnityContainer RegisterViewModelsDependencies(this IUnityContainer container)
        {
            return container;
        }
    }
}