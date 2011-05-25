namespace Torshify
{
    public static class ArtistExtensions
    {
        public static IArtistBrowse Browse(this IArtist artist, object userData = null)
        {
            return artist.Session.Browse(artist, userData);
        }
    }
}