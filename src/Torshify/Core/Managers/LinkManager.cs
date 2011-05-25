using System;
using System.Collections.Generic;
using System.Linq;
using Torshify.Core.Native;

namespace Torshify.Core.Managers
{
    internal class LinkManager
    {
        #region Fields

        private static Dictionary<IntPtr, NativeLink> _instances = new Dictionary<IntPtr, NativeLink>();
        private static readonly object _instanceLock = new object();

        #endregion Fields

        internal static ILink Get(ISession session, IntPtr handle, ISearch search = null)
        {
            lock (_instanceLock)
            {
                NativeLink instance;

                if (!_instances.TryGetValue(handle, out instance))
                {
                    LinkType type;
                    lock (Spotify.Mutex)
                    {
                        type = Spotify.sp_link_type(handle);
                    }

                    switch (type)
                    {
                        case LinkType.Track:
                            instance = new NativeTrackAndOffsetLink(session, handle);
                            break;
                        case LinkType.Album:
                            instance = new NativeAlbumLink(session, handle);
                            break;
                        case LinkType.Artist:
                            instance = new NativeArtistLink(session, handle);
                            break;
                        case LinkType.Search:
                            instance = new NativeSearchLink(session, handle, search);
                            break;
                        case LinkType.Playlist:
                            instance = new NativePlaylistLink(session, handle);
                            break;
                        case LinkType.Profile:
                            instance = new NativeUserLink(session, handle);
                            break;
                        case LinkType.Starred:
                            instance = new NativePlaylistLink(session, handle);
                            break;
                        case LinkType.LocalTrack:
                            instance = new NativeTrackAndOffsetLink(session, handle);
                            break;
                        default:
                            throw new ArgumentException("Invalid link.");
                    }

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
                NativeLink instance;

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

                foreach (var track in toRemove)
                {
                    var t = _instances[track];
                    t.Dispose();
                    _instances.Remove(track);
                }
            }
        }
    }
}