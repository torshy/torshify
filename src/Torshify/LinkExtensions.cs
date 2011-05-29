using System;
using Torshify.Core;
using Torshify.Core.Managers;
using Torshify.Core.Native;

namespace Torshify
{
    public static class LinkExtensions
    {
        public static string ToLink(this IImage image)
        {
            return CreateLink(image, Spotify.sp_link_create_from_image);
        }

        public static string ToLink(this IArtist artist)
        {
            return CreateLink(artist, Spotify.sp_link_create_from_artist);
        }

        public static string ToLink(this ITrack track)
        {
            return CreateLink<ITrackAndOffset>(() =>
                                                {
                                                    var wrapper = track as INativeObject;
                                                    return wrapper == null ? IntPtr.Zero : wrapper.Handle;
                                                },
                                                track.Session,
                                                h => Spotify.sp_link_create_from_track(h, 0));
        }

        public static string ToLink(this ITrack track, TimeSpan offset)
        {
            return CreateLink<ITrackAndOffset>(() =>
                                                {
                                                    var wrapper = track as INativeObject;
                                                    return wrapper == null ? IntPtr.Zero : wrapper.Handle;
                                                },
                                                track.Session,
                                                h => Spotify.sp_link_create_from_track(h, (int)offset.TotalMilliseconds));
        }

        public static string ToLink(this IAlbum album)
        {
            return CreateLink(album, Spotify.sp_link_create_from_album);
        }

        public static string AlbumCoverToLink(this IAlbum album)
        {
            return CreateLink<IImage>(() =>
                                          {
                                              var wrapper = album as INativeObject;
                                              return wrapper == null ? IntPtr.Zero : wrapper.Handle;
                                          },
                                          album.Session,
                                          Spotify.sp_link_create_from_album_cover);
        }

        public static string ArtistPortraitToLink(this IArtistBrowse artistBrowse, int artistIndex)
        {
            return CreateLink<IImage>(() =>
                                        {
                                            var wrapper = artistBrowse as INativeObject;
                                            return wrapper == null ? IntPtr.Zero : wrapper.Handle;
                                        },
                                        artistBrowse.Session,
                                        h => Spotify.sp_link_create_from_artist_portrait(h, artistIndex));
        }

        public static string ToLink(this IPlaylist playlist)
        {
            return CreateLink(playlist, Spotify.sp_link_create_from_playlist);
        }

        public static string ToLink(this IUser user)
        {
            return CreateLink(user, Spotify.sp_link_create_from_user);
        }

        public static string ToLink(this ISearch search)
        {
            var wrapper = search as INativeObject;

            if (wrapper == null)
            {
                return string.Empty;
            }

            IntPtr linkPtr;

            lock (Spotify.Mutex)
            {
                linkPtr = Spotify.sp_link_create_from_search(wrapper.Handle);
            }

            using (var link = (ILink<ISearch>)LinkManager.Get(wrapper.Session, linkPtr, search))
            {
                return link.ToString();
            }
        }

        private static string CreateLink<T>(T instance, Func<IntPtr, IntPtr> create)
        {
            var wrapper = instance as INativeObject;

            if (wrapper == null)
            {
                return string.Empty;
            }

            return CreateLink<T>(() => wrapper.GetHandle(), wrapper.Session, create);
        }

        private static string CreateLink<T>(Func<IntPtr> getInstanceHandle, ISession session, Func<IntPtr, IntPtr> create)
        {
            IntPtr handle = getInstanceHandle();
            IntPtr linkPtr;

            lock (Spotify.Mutex)
            {
                linkPtr = create(handle);
            }

            using (var link = (ILink<T>)LinkManager.Get(session, linkPtr))
            {
                return link.ToString();
            }
        }
    }
}