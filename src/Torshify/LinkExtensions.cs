using System;
using Torshify.Core;
using Torshify.Core.Managers;
using Torshify.Core.Native;

namespace Torshify
{
    public static class LinkExtensions
    {
        public static string AsUri(this ILink link)
        {
            return link == null ? string.Empty : link.ToString();
        }

        public static ILink<IImage> ToLink(this IImage image)
        {
            return CreateLink(image, Spotify.sp_link_create_from_image);
        }

        public static ILink<IArtist> ToLink(this IArtist artist)
        {
            return CreateLink(artist, Spotify.sp_link_create_from_artist);
        }

        public static ILink<ITrackAndOffset> ToLink(this ITrack track)
        {
            return CreateLink<ITrackAndOffset>(
                () =>
                {
                    var wrapper = track as INativeObject;
                    return wrapper == null ? IntPtr.Zero : wrapper.Handle;
                },
                track.Session,
                h => Spotify.sp_link_create_from_track(h, 0));
        }

        public static ILink<ITrackAndOffset> ToLink(this ITrack track, TimeSpan offset)
        {
            return CreateLink<ITrackAndOffset>(
                () =>
                {
                    var wrapper = track as INativeObject;
                    return wrapper == null ? IntPtr.Zero : wrapper.Handle;
                },
                track.Session,
                h => Spotify.sp_link_create_from_track(h, (int)offset.TotalMilliseconds));
        }

        public static ILink<IAlbum> ToLink(this IAlbum album)
        {
            return CreateLink(album, Spotify.sp_link_create_from_album);
        }

        public static ILink<IImage> AlbumCoverToLink(this IAlbum album, ImageSize imageSize = ImageSize.Normal)
        {
            return CreateLink<IImage>(
                () =>
                {
                    var wrapper = album as INativeObject;
                    return wrapper == null ? IntPtr.Zero : wrapper.Handle;
                },
                album.Session,
                handle => Spotify.sp_link_create_from_album_cover(handle, imageSize));
        }

        public static ILink<IImage> ArtistPortraitToLink(this IArtistBrowse artistBrowse, int artistIndex)
        {
            return CreateLink<IImage>(
                () =>
                {
                    var wrapper = artistBrowse as INativeObject;
                    return wrapper == null ? IntPtr.Zero : wrapper.Handle;
                },
                artistBrowse.Session,
                h => Spotify.sp_link_create_from_artistbrowse_portrait(h, artistIndex));
        }

        public static ILink<IPlaylist> ToLink(this IPlaylist playlist)
        {
            return CreateLink(playlist, Spotify.sp_link_create_from_playlist);
        }

        public static ILink<IUser> ToLink(this IUser user)
        {
            return CreateLink(user, Spotify.sp_link_create_from_user);
        }

        public static ILink<ISearch> ToLink(this ISearch search)
        {
            var wrapper = search as INativeObject;

            if (wrapper == null)
            {
                return null;
            }

            IntPtr linkPtr;

            lock (Spotify.Mutex)
            {
                linkPtr = Spotify.sp_link_create_from_search(wrapper.Handle);
            }

            return (ILink<ISearch>)LinkManager.Get(wrapper.Session, linkPtr, search.Query);
        }

        public static ILink<T> FromLink<T>(this ISession session, string link)
        {
            return FromLink(session, link) as ILink<T>;
        }

        public static ILink FromLink(this ISession session, string link)
        {
            IntPtr linkHandle;

            lock (Spotify.Mutex)
            {
                linkHandle = Spotify.sp_link_create_from_string(link);
            }

            return LinkManager.Get(session, linkHandle, link);
        }

        private static ILink<T> CreateLink<T>(T instance, Func<IntPtr, IntPtr> create)
        {
            var wrapper = instance as INativeObject;

            if (wrapper == null)
            {
                return null;
            }

            return CreateLink<T>(wrapper.GetHandle, wrapper.Session, create);
        }

        private static ILink<T> CreateLink<T>(
            Func<IntPtr> getInstanceHandle,
            ISession session,
            Func<IntPtr, IntPtr> create,
            TimeSpan? trackLinkOffset = null)
        {
            IntPtr handle = getInstanceHandle();

            if (handle == IntPtr.Zero)
            {
                return null;
            }

            IntPtr linkPtr;

            lock (Spotify.Mutex)
            {
                linkPtr = create(handle);
            }

            return (ILink<T>)LinkManager.Get(session, linkPtr, trackLinkOffset: trackLinkOffset);
        }
    }
}