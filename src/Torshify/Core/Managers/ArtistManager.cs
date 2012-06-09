using System;
using System.Collections.Generic;
using Torshify.Core.Native;

namespace Torshify.Core.Managers
{
    internal class ArtistManager
    {
        #region Fields

        private static readonly object _instanceLock = new object();

        private static WeakHandleDictionary<NativeArtist> _instances = new WeakHandleDictionary<NativeArtist>();

        #endregion Fields

        #region Internal Static Methods

        internal static IArtist Get(ISession session, IntPtr handle)
        {
            lock (_instanceLock)
            {
                NativeArtist artist;

                if (_instances.TryGetValue(handle, out artist))
                {
                    return artist;
                }

                artist = new NativeArtist(session, handle);
                artist.Initialize();

                if (SessionFactory.IsInternalCachingEnabled)
                {
                    _instances.SetValue(handle, artist);
                }

                return artist;
            }
        }

        internal static void Remove(IntPtr handle)
        {
            lock (_instanceLock)
            {
                _instances.Remove(handle);
            }
        }

        internal static void RemoveAll()
        {
            var toDispose = new List<NativeArtist>();

            lock (_instanceLock)
            {
                foreach (var key in _instances.Keys)
                {
                    NativeArtist instance;
                    if (_instances.TryGetValue(key, out instance))
                    {
                        toDispose.Add(instance);
                    }
                }

                _instances.Clear();
            }

            foreach (var instance in toDispose)
            {
                instance.Dispose();
            }
        }

        #endregion Internal Static Methods
    }
}