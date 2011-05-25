using Microsoft.Practices.Unity;
using Torshify.Server.Interfaces;

namespace Torshify.Server.Extensions
{
    public static class UnityExtensions
    {
        public static void RegisterStartable<TFrom, TTo>(this IUnityContainer container)
            where TTo : IStartable, TFrom
        {
            container.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
            container.RegisterType(typeof(IStartable),
                typeof(TTo),
                typeof(TTo).Name,
                new InjectionFactory(unity => unity.Resolve(typeof(TTo))));
        }
    }
}