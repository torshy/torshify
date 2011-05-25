namespace Torshify
{
    public enum RelationType
    {
        Unknown = 0,          /// Not yet known
        None = 1,             /// No relation
        Unidirectional = 2,   /// The currently logged in user is following this user
        Bidirectional = 3     /// Bidirectional friendship established            
    }
}