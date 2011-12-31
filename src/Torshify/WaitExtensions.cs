using System;
using System.Threading;

namespace Torshify
{
    public static class WaitExtensions
    {
        public static ISearch WaitForCompletion(this ISearch search)
        {
            var reset = new ManualResetEvent(search.IsComplete);
            EventHandler<SearchEventArgs> handler = (s, e) => reset.Set();
            search.Completed += handler;
            reset.WaitOne();
            search.Completed -= handler;
            return search;
        }

        public static IAlbumBrowse WaitForCompletion(this IAlbumBrowse browse)
        {
            var reset = new ManualResetEvent(browse.IsComplete);
            EventHandler<UserDataEventArgs> handler = (s, e) => reset.Set();
            browse.Completed += handler;
            reset.WaitOne();
            browse.Completed -= handler;
            return browse;
        }

        public static IArtistBrowse WaitForCompletion(this IArtistBrowse browse)
        {
            var reset = new ManualResetEvent(browse.IsComplete);
            EventHandler handler = (s, e) => reset.Set();
            browse.Completed += handler;
            reset.WaitOne();
            browse.Completed -= handler;
            return browse;
        }

        public static IToplistBrowse WaitForCompletion(this IToplistBrowse browse)
        {
            var reset = new ManualResetEvent(browse.IsComplete);
            EventHandler<UserDataEventArgs> handler = (s, e) => reset.Set();
            browse.Completed += handler;
            reset.WaitOne();
            browse.Completed -= handler;
            return browse;
        }

        public static IPlaylistContainer WaitUntilLoaded(this IPlaylistContainer source, int millisecondsTimeout = 10000)
        {
            var reset = new ManualResetEvent(source.IsLoaded);
            EventHandler handler = (s, e) => reset.Set();
            source.Loaded += handler;
            reset.WaitOne(millisecondsTimeout);
            source.Loaded -= handler;
            return source;
        }

        public static IPlaylist WaitUntilLoaded(this IPlaylist source, int millisecondsTimeout = 10000)
        {
            WaitUntilLoaded(() => source.IsLoaded, millisecondsTimeout);
            return source;
        }

        public static ITrack WaitUntilLoaded(this ITrack source, int millisecondsTimeout = 10000)
        {
            WaitUntilLoaded(() => source.IsLoaded, millisecondsTimeout);
            return source;
        }

        public static IAlbum WaitUntilLoaded(this IAlbum source, int millisecondsTimeout = 10000)
        {
            WaitUntilLoaded(() => source.IsLoaded, millisecondsTimeout);
            return source;
        }

        public static IArtist WaitUntilLoaded(this IArtist source, int millisecondsTimeout = 10000)
        {
            WaitUntilLoaded(() => source.IsLoaded, millisecondsTimeout);
            return source;
        }

        public static IUser WaitUntilLoaded(this IUser source, int millisecondsTimeout = 10000)
        {
            WaitUntilLoaded(() => source.IsLoaded, millisecondsTimeout);
            return source;
        }

        public static IImage WaitUntilLoaded(this IImage source, int millisecondsTimeout = 10000)
        {
            var reset = new ManualResetEvent(source.IsLoaded);
            EventHandler handler = (s, e) => reset.Set();
            source.Loaded += handler;
            reset.WaitOne(millisecondsTimeout);
            source.Loaded -= handler;
            return source;
        }

        private static void WaitUntilLoaded(Func<bool> isLoaded, int millisecondsTimeout)
        {
            var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);
            var startTime = DateTime.UtcNow;
            var endTime = startTime.Add(timeout);

            while (!isLoaded() && DateTime.UtcNow < endTime)
            {
                Thread.Yield();
            }
        }
    }
}