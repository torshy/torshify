using System;

using Torshify.Core.Native;

namespace Torshify.Core.Managers
{
    internal class TrackManager
    {
        #region Fields

        private static readonly object _instanceLock = new object();
        private static WeakHandleDictionary<NativeTrack> _instances = new WeakHandleDictionary<NativeTrack>();

        #endregion Fields

        #region Internal Static Methods

        internal static ITrack Get(ISession session, IntPtr handle)
        {
            lock (_instanceLock)
            {
                NativeTrack track;

                if (_instances.TryGetValue(handle, out track))
                {
                    return track;
                }

                track = new NativeTrack(session, handle);
                track.Initialize();
                _instances.SetValue(handle, track);
                return track;
            }
        }

        internal static void Remove(IntPtr handle)
        {
            lock (_instanceLock)
            {
                _instances.Remove(handle);
            }
        }

        internal static void DisposeAll(ISession session)
        {
            lock (_instanceLock)
            {
                foreach (var key in _instances.Keys)
                {
                    NativeTrack track;

                    if (_instances.TryGetValue(key, out track))
                    {
                        if (track != null)
                        {
                            track.Dispose();
                        }
                    }
                }

                _instances.Clear();
            }
        }

        #endregion Internal Static Methods
    }
}