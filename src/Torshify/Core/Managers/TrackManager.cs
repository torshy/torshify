using System;
using System.Collections.Generic;
using System.Linq;
using Torshify.Core.Native;

namespace Torshify.Core.Managers
{
    internal class TrackManager
    {
        #region Fields

        private static readonly object _instanceLock = new object();

        private static Dictionary<IntPtr, NativeTrack> _instances = new Dictionary<IntPtr, NativeTrack>();

        #endregion Fields

        #region Internal Static Methods

        internal static ITrack Get(ISession session, IntPtr handle)
        {
            lock (_instanceLock)
            {
                NativeTrack instance;

                if (!_instances.TryGetValue(handle, out instance))
                {
                    instance = new NativeTrack(session, handle);
                    _instances.Add(handle, instance);
                    instance.Initialize();
                }

                return instance;
            }
        }

        internal static void Remove(IntPtr handle)
        {
            lock (_instanceLock)
            {
                NativeTrack instance;

                if (_instances.TryGetValue(handle, out instance))
                {
                    _instances.Remove(handle);
                }
            }
        }

        internal static void RemoveAll(ISession session)
        {
            lock (_instanceLock)
            {
                List<IntPtr> toRemove = (from nativeTrack in _instances where nativeTrack.Value.Session == session select nativeTrack.Key).Cast<IntPtr>().ToList();

                foreach (var track in toRemove)
                {
                    var t = _instances[track];
                    t.Dispose();
                    _instances.Remove(track);
                }
            }
        }

        #endregion Internal Static Methods
    }
}