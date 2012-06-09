namespace Torshify
{
    public static class ArtistExtensions
    {
        public static IArtistBrowse Browse(this IArtist artist, object userData = null)
        {
            return Browse(artist, ArtistBrowseType.Full, userData);
        }

        public static IArtistBrowse Browse(this IArtist artist, ArtistBrowseType browseType, object userData = null)
        {
            return artist.Session.Browse(artist, browseType, userData);
        }
    }
}