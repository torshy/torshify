using System;

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
                _instances.SetValue(handle, artist);
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

        #endregion Internal Static Methods
    }
}