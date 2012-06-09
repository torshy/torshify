using System;

namespace Torshify
{
    public interface ILink : IDisposable
    {
        LinkType Type
        {
            get;
        }

        object Object
        {
            get;
        }

        string GetStringLink();
    }
}
