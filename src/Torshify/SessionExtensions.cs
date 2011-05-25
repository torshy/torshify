using System;
using System.Threading;

namespace Torshify
{
    public static class SessionExtensions
    {
        public static IImage WaitForCompletion(this IImage image)
        {
            var reset = new ManualResetEvent(image.IsLoaded);
            EventHandler handler = (s, e) => reset.Set();
            image.Loaded += handler;
            reset.WaitOne();
            image.Loaded -= handler;
            return image;
        }

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
    }
}