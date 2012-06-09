using System;
using System.Collections.Generic;
using System.Linq;

using Torshify.Core.Native;

namespace Torshify.Core.Managers
{
    internal class PlaylistManager
    {
        #region Fields

        private static readonly object _instanceLock = new object();

        private static Dictionary<IntPtr, NativePlaylist> _instances = new Dictionary<IntPtr, NativePlaylist>();

        #endregion Fields

        #region Internal Static Methods

        internal static IPlaylist Get(ISession session, IntPtr handle)
        {
            lock (_instanceLock)
            {
                NativePlaylist instance;

                if (!_instances.TryGetValue(handle, out instance))
                {
                    instance = new NativePlaylist(session, handle);
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
                NativePlaylist instance;

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
                List<IntPtr> toRemove = (from playlist in _instances
                                         where playlist.Value.Session == session
                                         select playlist.Key).ToList();

                foreach (var handle in toRemove)
                {
                    var playlist = _instances[handle];
                    playlist.Dispose();
                    _instances.Remove(handle);
                }
            }
        }

        #endregion Internal Static Methods
    }
}