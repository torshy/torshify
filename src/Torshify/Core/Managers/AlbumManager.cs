using System;
using System.Collections.Generic;

using Torshify.Core.Native;

namespace Torshify.Core.Managers
{
    internal class AlbumManager
    {
        #region Fields

        private static readonly object _instanceLock = new object();

        private static Dictionary<IntPtr, NativeAlbum> _instances = new Dictionary<IntPtr, NativeAlbum>();

        #endregion Fields

        #region Internal Static Methods

        internal static IAlbum Get(ISession session, IntPtr handle)
        {
            lock (_instanceLock)
            {
                NativeAlbum instance;

                if (!_instances.TryGetValue(handle, out instance))
                {
                    instance = new NativeAlbum(session, handle);
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
                NativeAlbum instance;

                if (_instances.TryGetValue(handle, out instance))
                {
                    _instances.Remove(handle);
                }
            }
        }

        #endregion Internal Static Methods
    }
}