using Unity;

namespace FlyApp.Core
{
    public static class Bootstrapper
    {
        public static IUnityContainer RegisterCoreDependencies(this IUnityContainer container)
        {
            return container;
        }
    }
}