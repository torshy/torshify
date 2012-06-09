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

        public static bool WaitUntilLoaded(this IPlaylistContainer source, int millisecondsTimeout = 10000)
        {
            var reset = new ManualResetEvent(source.IsLoaded);
            EventHandler handler = (s, e) => reset.Set();
            source.Loaded += handler;
            bool result = reset.WaitOne(millisecondsTimeout);
            source.Loaded -= handler;
            return result;
        }

        public static bool WaitUntilLoaded(this IPlaylist source, int millisecondsTimeout = 10000)
        {
            var reset = new ManualResetEvent(source.IsLoaded);
            EventHandler handler = (s, e) =>
                                   {
                                       IPlaylist p = (IPlaylist)s;
                                       if (p.IsLoaded)
                                       {
                                           reset.Set();
                                       }
                                   };
            source.StateChanged += handler;
            bool result = reset.WaitOne(millisecondsTimeout);
            source.StateChanged -= handler;
            return result;
        }

        public static bool WaitUntilLoaded(this ITrack source, int millisecondsTimeout = 10000)
        {
            return WaitUntilLoaded(() => source.IsLoaded, millisecondsTimeout);
        }

        public static bool WaitUntilLoaded(this IAlbum source, int millisecondsTimeout = 10000)
        {
            return WaitUntilLoaded(() => source.IsLoaded, millisecondsTimeout);
        }

        public static bool WaitUntilLoaded(this IArtist source, int millisecondsTimeout = 10000)
        {
            return WaitUntilLoaded(() => source.IsLoaded, millisecondsTimeout);
        }

        public static bool WaitUntilLoaded(this IUser source, int millisecondsTimeout = 10000)
        {
            return WaitUntilLoaded(() => source.IsLoaded, millisecondsTimeout);
        }

        public static bool WaitUntilLoaded(this IImage source, int millisecondsTimeout = 10000)
        {
            var reset = new ManualResetEvent(source.IsLoaded);
            EventHandler handler = (s, e) => reset.Set();
            source.Loaded += handler;
            bool result = reset.WaitOne(millisecondsTimeout);
            source.Loaded -= handler;
            return result;
        }

        private static bool WaitUntilLoaded(Func<bool> isLoaded, int millisecondsTimeout)
        {
            var timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);
            var startTime = DateTime.UtcNow;
            var endTime = startTime.Add(timeout);
            var complete = isLoaded();

            while (!complete && DateTime.UtcNow < endTime)
            {
                complete = isLoaded();
                Thread.Yield();
            }

            return complete;
        }
    }
}