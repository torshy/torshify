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

        string PortraitId
        {
            get;
        }

        #endregion Properties
    }
}