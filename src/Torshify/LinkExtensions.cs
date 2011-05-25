using System;
using Torshify.Core;
using Torshify.Core.Managers;
using Torshify.Core.Native;

namespace Torshify
{
    public static class LinkExtensions
    {
        public static string ToLink(this IArtist artist)
        {
            return CreateLink(artist, Spotify.sp_link_create_from_artist);
        }

        public static string ToLink(this ITrack track)
        {
            var wrapper = track as INativeObject;

            if (wrapper == null)
            {
                return string.Empty;
            }

            IntPtr linkPtr;

            lock (Spotify.Mutex)
            {
                linkPtr = Spotify.sp_link_create_from_track(wrapper.Handle, 0);
            }

            using (var link = (ILink<ITrackAndOffset>)LinkManager.Get(wrapper.Session, linkPtr))
            {
                return link.ToString();
            }
        }

        public static string ToLink(this ITrack track, TimeSpan offset)
        {
            var wrapper = track as INativeObject;

            if (wrapper == null)
            {
                return string.Empty;
            }

            IntPtr linkPtr;

            lock (Spotify.Mutex)
            {
                linkPtr = Spotify.sp_link_create_from_track(wrapper.Handle, (int) offset.TotalMilliseconds);
            }

            using (var link = (ILink<ITrackAndOffset>)LinkManager.Get(wrapper.Session, linkPtr))
            {
                return link.ToString();
            }
        }

        public static string ToLink(this IAlbum album)
        {
            return CreateLink(album, Spotify.sp_link_create_from_album);
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

            IntPtr linkPtr;

            lock (Spotify.Mutex)
            {
                linkPtr = create(wrapper.Handle);
            }

            using (var link = (ILink<T>)LinkManager.Get(wrapper.Session, linkPtr))
            {
                return link.ToString();
            }
        }
    }
}