namespace Torshify
{
    public enum RelationType
    {
        /// <summary>
        /// Not yet known
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// No relation
        /// </summary>
        None = 1,

        /// <summary>
        /// The currently logged in user is following this user
        /// </summary>
        Unidirectional = 2,

        /// <summary>
        /// Bidirectional friendship established
        /// </summary>
        Bidirectional = 3
    }
}