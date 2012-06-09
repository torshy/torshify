namespace Torshify
{
    public static class AlbumExtensions
    {
        public static IAlbumBrowse Browse(this IAlbum album, object userData = null)
        {
            return album.Session.Browse(album, userData);
        }
    }
}