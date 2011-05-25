using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace Torshify.Server.Extensions
{
    public static class ServiceLocatorExtensions
    {
        public static T Resolve<T>(this IServiceLocator locator)
        {
            return (T)locator.GetInstance(typeof (T));
        }

        public static T TryResolve<T>(this IServiceLocator locator)
        {
            try
            {
                return Resolve<T>(locator);
            }
            catch
            {
                return default(T);
            }
        }

        public static IEnumerable<T> ResolveAll<T>(this IServiceLocator locator)
        {
            return locator.GetAllInstances(typeof (T)).Cast<T>();
        }
    }
}