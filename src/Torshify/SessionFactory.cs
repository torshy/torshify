using Torshify.Core.Managers;

namespace Torshify
{
    public class SessionFactory
    {
        public static ISession CreateSession(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent)
        {
            return SessionManager.Create(applicationKey, cacheLocation, settingsLocation, userAgent);
        }
    }
}