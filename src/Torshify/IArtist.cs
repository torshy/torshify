namespace Torshify
{
    public interface IArtist : ISessionObject
    {
        #region Properties

        bool IsLoaded
        {
            get;
        }

        string Name
        {
            get;
        }

        #endregion Properties
    }
}