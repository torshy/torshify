using System;
using System.Collections.Generic;

using Torshify.Core.Native;

namespace Torshify.Core.Managers
{
    internal class ContainerPlaylistManager
    {
        #region Fields

        private static readonly object _instanceLock = new object();

        private static Dictionary<KeyGen, NativeContainerPlaylist> _instances = new Dictionary<KeyGen, NativeContainerPlaylist>();

        #endregion Fields

        #region Internal Static Methods

        internal static IContainerPlaylist Get(
            ISession session,
            IPlaylistContainer container,
            IntPtr handle,
            IntPtr folderId,
            PlaylistType playlistType)
        {
            KeyGen key = new KeyGen(handle, folderId, playlistType);

            lock (_instanceLock)
            {
                NativeContainerPlaylist instance;

                if (!_instances.TryGetValue(key, out instance))
                {
                    instance = new NativeContainerPlaylist(session, handle, folderId, playlistType, container);
                    instance.Initialize();

                    if (SessionFactory.IsInternalCachingEnabled)
                    {
                        _instances.Add(key, instance);
                    }
                }

                return instance;
            }
        }

        internal static void Remove(IntPtr playlistPtr, IntPtr folderId, PlaylistType type)
        {
            lock (_instanceLock)
            {
                KeyGen key = new KeyGen(playlistPtr, folderId, type);
                _instances.Remove(key);
            }
        }

        internal static void RemoveAll(ISession session)
        {
            lock (_instanceLock)
            {
                List<KeyGen> keysToRemove = new List<KeyGen>();

                foreach (var nativeContainerPlaylist in _instances)
                {
                    if (nativeContainerPlaylist.Value.Session == session)
                    {
                        keysToRemove.Add(nativeContainerPlaylist.Key);
                    }
                }

                foreach (var keyGen in keysToRemove)
                {
                    var i = _instances[keyGen];

                    if (keyGen.Item3 != PlaylistType.StartFolder || keyGen.Item3 != PlaylistType.EndFolder)
                    {
                        i.Dispose();
                    }

                    _instances.Remove(keyGen);
                }
            }
        }

        #endregion Internal Static Methods

        #region Nested Types

        private class KeyGen : Tuple<IntPtr, IntPtr, PlaylistType>
        {
            #region Constructors

            public KeyGen(IntPtr playlistPtr, IntPtr folderId, PlaylistType type)
                : base(playlistPtr, folderId, type)
            {
            }

            #endregion Constructors
        }

        #endregion Nested Types
    }
}