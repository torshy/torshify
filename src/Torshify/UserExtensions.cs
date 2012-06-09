namespace Torshify
{
    public static class UserExtensions
    {
        public static IToplistBrowse Browse(this IUser user, ToplistType toplistType, object userData = null)
        {
            return user.Session.Browse(toplistType, user.CanonicalName, userData);
        }
    }
}