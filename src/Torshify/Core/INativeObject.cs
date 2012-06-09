using System;

namespace Torshify.Core
{
    internal interface INativeObject : ISessionObject
    {
        IntPtr Handle { get; }
    }
}