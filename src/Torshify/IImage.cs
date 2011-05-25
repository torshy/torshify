using System;

namespace Torshify
{
    public interface IImage : IDisposable
    {
        byte[] Data { get; }
        Error Error { get; }
        ImageFormat Format { get; }
        string ImageId { get; }
        bool IsLoaded { get; }
        event EventHandler Loaded;
    }
}