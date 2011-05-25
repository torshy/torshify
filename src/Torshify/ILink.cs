using System;

namespace Torshify
{
    public interface ILink 
    {
        LinkType Type { get; }
        object Object { get; }
    }

    public interface ILink<out T> : ILink, IDisposable
    {
        new T Object { get; }
    }
}
