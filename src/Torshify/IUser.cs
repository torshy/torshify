namespace Torshify
{
    public interface IUser : ISessionObject
    {
        #region Properties

        string CanonicalName
        {
            get;
        }

        string DisplayName
        {
            get;
        }

        string FullName
        {
            get;
        }

        bool IsLoaded
        {
            get;
        }

        string Picture
        {
            get;
        }

        RelationType Relation
        {
            get;
        }

        #endregion Properties
    }
}