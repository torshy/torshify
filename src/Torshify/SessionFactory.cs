using Torshify.Core.Managers;

namespace Torshify
{
    public class SessionFactory
    {
        #region Constructors

        static SessionFactory()
        {
            IsInternalCachingEnabled = false;
        }

        #endregion Constructors

        #region Properties

        public static bool IsInternalCachingEnabled
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public static ISession CreateSession(
            byte[] applicationKey, 
            string cacheLocation, 
            string settingsLocation, 
            string userAgent)
        {
            return CreateSession(
                applicationKey,
                new SessionOptions
                {
                    CacheLocation = cacheLocation,
                    SettingsLocation = settingsLocation,
                    UserAgent = userAgent
                });
        }

        public static ISession CreateSession(byte[] applicationKey, SessionOptions options)
        {
            return SessionManager.Create(applicationKey, options);
        }

        #endregion Methods
    }
}