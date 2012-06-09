using System;
using System.Collections.Generic;
using System.Linq;
using Torshify.Core.Native;

namespace Torshify.Core.Managers
{
    internal class PlaylistTrackManager
    {
        #region Fields

        private static readonly object _tracksLock = new object();

        private static Dictionary<KeyGen, IPlaylistTrack> _instances = new Dictionary<KeyGen, IPlaylistTrack>();
        private static Dictionary<IPlaylist, int> _playlistLengths = new Dictionary<IPlaylist, int>();

        #endregion Fields

        #region Internal Static Methods

        internal static IPlaylistTrack Get(ISession session, IPlaylist playlist, IntPtr trackPointer, int position)
        {
            KeyGen key = new KeyGen(playlist, position);

            lock (_tracksLock)
            {
                IPlaylistTrack instance;

                if (!_instances.TryGetValue(key, out instance))
                {
                    var newInstance = new NativePlaylistTrack(session, trackPointer, playlist, position);

                    if (SessionFactory.IsInternalCachingEnabled)
                    {
                        _instances.Add(key, newInstance);
                    }

                    newInstance.Initialize();
                    instance = newInstance;

                    Error error = instance.Error;
                    if (error != Error.OK && error != Error.IsLoading)
                    {
                        instance.Dispose();
                        instance = null;
                        
                        if (SessionFactory.IsInternalCachingEnabled)
                        {
                            _instances[key] = instance = new ErroneousTrack(session, trackPointer, error, playlist);
                        }
                    }
                }

                return instance;
            }
        }

        internal static void Remove(IPlaylist playlist, int position)
        {
            KeyGen key = new KeyGen(playlist, position);

            lock (_tracksLock)
            {
                IPlaylistTrack instance;

                if (_instances.TryGetValue(key, out instance))
                {
                    _instances.Remove(key);
                }
            }
        }

        internal static void RemoveAll(IPlaylist playlist)
        {
            lock (_tracksLock)
            {
                List<KeyGen> keysToRemove = (from track in _instances
                                             where track.Value.Playlist == playlist
                                             select track.Key).ToList();

                foreach (var keyGen in keysToRemove)
                {
                    var instance = _instances[keyGen];
                    instance.Dispose();
                    _instances.Remove(keyGen);
                }
            }
        }

        internal static void RemoveAll(ISession session)
        {
            lock (_tracksLock)
            {
                List<KeyGen> toRemove = new List<KeyGen>();
                foreach (var keyValue in _instances)
                {
                    if (keyValue.Value.Session == session)
                    {
                        toRemove.Add(keyValue.Key);
                    }
                }

                foreach (var keyGen in toRemove)
                {
                    var t = _instances[keyGen];
                    try
                    {
                        t.Dispose();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _instances.Remove(keyGen);
                    }
                }
            }
        }

        #endregion Internal Static Methods

        #region Nested Types

        private class KeyGen : Tuple<IPlaylist, int>
        {
            #region Constructors

            public KeyGen(IPlaylist playlist, int position)
                : base(playlist, position)
            {
            }

            #endregion Constructors
        }

        #endregion Nested Types
    }
}