using System;

namespace Torshify
{
    public interface ISessionObject : IDisposable
    {
        ISession Session { get; }
    }
}