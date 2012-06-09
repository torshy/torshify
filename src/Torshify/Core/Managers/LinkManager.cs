using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Torshify.Core.Native;

namespace Torshify.Core.Managers
{
    internal class LinkManager
    {
        #region Fields

        private static readonly object _instanceLock = new object();

        private static Dictionary<IntPtr, NativeLink> _instances = new Dictionary<IntPtr, NativeLink>();

        #endregion Fields

        #region Internal Static Methods

        internal static ILink Get(
            ISession session,
            IntPtr handle,
            string linkAsString = null,
            TimeSpan? trackLinkOffset = null)
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
                        case LinkType.LocalTrack:
                            if (!string.IsNullOrEmpty(linkAsString))
                            {
                                instance = new NativeTrackAndOffsetLink(session, handle, linkAsString);
                            }
                            else if (trackLinkOffset.HasValue)
                            {
                                instance = new NativeTrackAndOffsetLink(session, handle, trackLinkOffset);
                            }
                            else
                            {
                                instance = new NativeTrackAndOffsetLink(session, handle);
                            }

                            break;
                        case LinkType.Album:
                            instance = new NativeAlbumLink(session, handle);
                            break;
                        case LinkType.Artist:
                            instance = new NativeArtistLink(session, handle);
                            break;
                        case LinkType.Search:
                            instance = new NativeSearchLink(session, handle, linkAsString);
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
                        case LinkType.Image:
                            instance = new NativeImageLink(session, handle);
                            break;
                        default:
                            throw new ArgumentException("Invalid link.");
                    }

                    if (SessionFactory.IsInternalCachingEnabled)
                    {
                        _instances.Add(handle, instance);
                    }

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

        internal static string LinkAsString(IntPtr linkHandle)
        {
            int bufferSize = Spotify.STRINGBUFFER_SIZE;

            try
            {
                int length;
                StringBuilder builder = new StringBuilder(bufferSize);

                lock (Spotify.Mutex)
                {
                    length = Spotify.sp_link_as_string(linkHandle, builder, bufferSize);
                }

                if (length == -1)
                {
                    return string.Empty;
                }

                return builder.ToString().Replace("%3a", ":");
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion Internal Static Methods
    }
}