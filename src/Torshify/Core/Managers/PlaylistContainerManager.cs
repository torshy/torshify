using System;
using System.Collections.Generic;
using System.Linq;

using Torshify.Core.Native;

namespace Torshify.Core.Managers
{
    internal class PlaylistContainerManager
    {
        #region Fields

        private static readonly object _instanceLock = new object();

        private static Dictionary<IntPtr, NativePlaylistContainer> _instances = new Dictionary<IntPtr, NativePlaylistContainer>();

        #endregion Fields

        #region Internal Static Methods

        internal static IPlaylistContainer Get(ISession session, IntPtr handle)
        {
            lock (_instanceLock)
            {
                NativePlaylistContainer instance;

                if (!_instances.TryGetValue(handle, out instance))
                {
                    instance = new NativePlaylistContainer(session, handle);
                    instance.Initialize();

                    if (SessionFactory.IsInternalCachingEnabled)
                    {
                        _instances.Add(handle, instance);
                    }
                }

                return instance;
            }
        }

        internal static void Remove(IntPtr handle)
        {
            lock (_instanceLock)
            {
                NativePlaylistContainer instance;

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
                List<IntPtr> toRemove = (from native in _instances where native.Value.Session == session select native.Key).ToList();

                foreach (var playlist in toRemove)
                {
                    var t = _instances[playlist];
                    t.Dispose();
                    _instances.Remove(playlist);
                }
            }
        }

        #endregion Internal Static Methods
    }
}