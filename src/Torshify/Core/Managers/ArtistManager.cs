using System;
using System.Collections.Generic;

using Torshify.Core.Native;

namespace Torshify.Core.Managers
{
    internal class ArtistManager
    {
        #region Fields

        private static readonly object _instanceLock = new object();

        private static Dictionary<IntPtr, NativeArtist> _instances = new Dictionary<IntPtr, NativeArtist>();

        #endregion Fields

        #region Internal Static Methods

        internal static IArtist Get(ISession session, IntPtr handle)
        {
            lock (_instanceLock)
            {
                NativeArtist instance;

                if (!_instances.TryGetValue(handle, out instance))
                {
                    instance = new NativeArtist(session, handle);
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
                NativeArtist instance;

                if (_instances.TryGetValue(handle, out instance))
                {
                    _instances.Remove(handle);
                }
            }
        }

        #endregion Internal Static Methods
    }
}