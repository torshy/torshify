using System;
using System.Threading;
using System.Threading.Tasks;

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

        public static Task<ISearch> SearchAsync(
            this ISession session,
            string query,
            int trackOffset,
            int trackCount,
            int albumOffset,
            int albumCount,
            int artistOffset,
            int artistCount,
            object userData = null)
        {
            var tcs = new TaskCompletionSource<ISearch>(userData);

            var search = session.Search(
                query,
                trackOffset,
                trackCount,
                albumOffset,
                albumCount,
                artistOffset,
                artistOffset,
                userData);
            search.Completed += (sender, args) => tcs.SetResult(search);
            return tcs.Task;
        }

        public static Task<ISearch> SearchAsync(
            this ISession session, 
            int fromYear,
            int toYear,
            RadioGenre genre,
            object userData = null)
        {
            var tcs = new TaskCompletionSource<ISearch>();

            var search = session.Search(
                fromYear,
                toYear,
                genre,
                userData);
            search.Completed += (sender, args) => tcs.SetResult(search);
            return tcs.Task;
        }

        public static Task<IArtistBrowse> BrowseAsync(
            this ISession session, 
            IArtist artist, 
            ArtistBrowseType type,
            object userState = null)
        {
            var tcs = new TaskCompletionSource<IArtistBrowse>();

            var browse = session.Browse(artist, type, userState);
            browse.Completed += (sender, args) => tcs.SetResult(browse);
            return tcs.Task;
        }

        public static Task<IAlbumBrowse> BrowseAsync(
            this ISession session, 
            IAlbum album,
            object userState = null)
        {
            var tcs = new TaskCompletionSource<IAlbumBrowse>();

            var browse = session.Browse(album, userState);
            browse.Completed += (sender, args) => tcs.SetResult(browse);
            return tcs.Task;
        }

        public static Task<IToplistBrowse> BrowseAsync(
            this ISession session,
            ToplistType toplist,
            object state = null)
        {
            var tcs = new TaskCompletionSource<IToplistBrowse>();

            var browse = session.Browse(toplist, state);
            browse.Completed += (sender, args) => tcs.SetResult(browse);
            return tcs.Task;
        }

        public static Task<IToplistBrowse> BrowseAsync(
            this ISession session, 
            ToplistType type,
            int encodedCountryCode, 
            object userData = null)
        {
            var tcs = new TaskCompletionSource<IToplistBrowse>();

            var browse = session.Browse(type, encodedCountryCode, userData);
            browse.Completed += (sender, args) => tcs.SetResult(browse);
            return tcs.Task;
        }

        public static Task<IToplistBrowse> BrowseAsync(
            this ISession session, 
            ToplistType type, 
            string userName, 
            object userData = null)
        {
            var tcs = new TaskCompletionSource<IToplistBrowse>();

            var browse = session.Browse(type, userName, userData);
            browse.Completed += (sender, args) => tcs.SetResult(browse);
            return tcs.Task;
        }

        public static Task<IToplistBrowse> BrowseCurrentUserAsync(
            this ISession session, 
            ToplistType type, 
            object userData = null)
        {
            var tcs = new TaskCompletionSource<IToplistBrowse>();

            var browse = session.BrowseCurrentUser(type, userData);
            browse.Completed += (sender, args) => tcs.SetResult(browse);
            return tcs.Task;
        }
    }
}