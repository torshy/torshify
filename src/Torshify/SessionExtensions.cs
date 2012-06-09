using System.Threading.Tasks;

namespace Torshify
{
    public static class SessionExtensions
    {
        public static Task<ISearch> SearchAsync(
            this ISession session,
            string query,
            int trackOffset,
            int trackCount,
            int albumOffset,
            int albumCount,
            int artistOffset,
            int artistCount,
            int playlistOffset,
            int playlistCount,
            SearchType searchType,
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
                artistCount,
                playlistOffset,
                playlistCount,
                searchType,
                userData);

            if (search.IsComplete)
            {
                tcs.SetResult(search);
            }

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

            if (browse.IsComplete)
            {
                tcs.SetResult(browse);
            }

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

            if (browse.IsComplete)
            {
                tcs.SetResult(browse);
            }

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

            if (browse.IsComplete)
            {
                tcs.SetResult(browse);
            }

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

            if (browse.IsComplete)
            {
                tcs.SetResult(browse);
            }

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

            if (browse.IsComplete)
            {
                tcs.SetResult(browse);
            }

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

            if (browse.IsComplete)
            {
                tcs.SetResult(browse);
            }

            browse.Completed += (sender, args) => tcs.SetResult(browse);
            return tcs.Task;
        }

        public static Task<IImage> GetImageAsync(
            this ISession session,
            string id)
        {
            var tcs = new TaskCompletionSource<IImage>();

            var image = session.GetImage(id);
            if (image.IsLoaded)
            {
                tcs.SetResult(image);
            }
            else
            {
                image.Loaded += (sender, args) => tcs.SetResult(image);
            }

            return tcs.Task;
        }
    }
}