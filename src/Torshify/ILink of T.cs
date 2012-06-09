namespace Torshify
{
    public interface ILink<out T> : ILink
    {
        new T Object { get; }
    }
}