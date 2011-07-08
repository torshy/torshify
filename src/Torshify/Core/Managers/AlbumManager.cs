using System;

using Torshify.Core.Native;

namespace Torshify.Core.Managers
{
    internal class AlbumManager
    {
        #region Fields

        private static readonly object _instanceLock = new object();

        private static WeakHandleDictionary<NativeAlbum> _instances = new WeakHandleDictionary<NativeAlbum>();

        #endregion Fields

        #region Internal Static Methods

        internal static IAlbum Get(ISession session, IntPtr handle)
        {
            lock (_instanceLock)
            {
                NativeAlbum album;

                if (_instances.TryGetValue(handle, out album))
                {
                    return album;
                }

                album = new NativeAlbum(session, handle);
                album.Initialize();
                _instances.SetValue(handle, album);
                return album;
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