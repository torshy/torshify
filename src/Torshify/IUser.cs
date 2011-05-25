namespace Torshify
{
    public interface IUser : ISessionObject
    {
        string CanonicalName { get; }
        string DisplayName { get; }
        bool IsLoaded { get; }
        string FullName { get; }
        string Picture { get; }
        RelationType Relation { get; }
    }
}