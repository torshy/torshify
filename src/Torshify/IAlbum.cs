namespace Torshify
{
    public interface IAlbum : ISessionObject
    {
        #region Properties

        IArtist Artist
        {
            get;
        }

        string CoverId
        {
            get;
        }

        bool IsAvailable
        {
            get;
        }

        bool IsLoaded
        {
            get;
        }

        string Name
        {
            get;
        }

        AlbumType Type
        {
            get;
        }

        int Year
        {
            get;
        }

        #endregion Properties
    }
}