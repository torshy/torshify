using System;

namespace Torshify
{
    public interface IInbox
    {
        event EventHandler Complete;

        Error Error { get; }
    }
}