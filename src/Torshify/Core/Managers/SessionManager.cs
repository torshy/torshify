using System;
using System.Collections.Generic;
using Torshify.Core.Native;

namespace Torshify.Core.Managers
{
    internal class SessionManager
    {
        #region Fields

        private static Dictionary<IntPtr, NativeSession> _sessions = new Dictionary<IntPtr, NativeSession>();

        #endregion Fields

        #region Internal Static Methods

        internal static ISession Create(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent)
        {
            NativeSession session = new NativeSession(applicationKey, cacheLocation, settingsLocation, userAgent);
            session.Initialize();
            _sessions.Add(session.Handle, session);
            //return new ManagedSession(session);
            return session;
        }

        internal static NativeSession Get(IntPtr sessionPtr)
        {
            NativeSession s;
            _sessions.TryGetValue(sessionPtr, out s);
            return s;
        }

        internal static void Remove(IntPtr sessionPtr)
        {
            if (_sessions.ContainsKey(sessionPtr))
            {
                _sessions.Remove(sessionPtr);
            }
        }

        #endregion Internal Static Methods
    }
}