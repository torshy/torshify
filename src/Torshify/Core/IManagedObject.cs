using System;

namespace Torshify.Core
{
    internal interface IManagedObject : IDisposable
    {
        INativeObject NativeObject { get; }
    }
}