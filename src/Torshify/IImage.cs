using System;

namespace Torshify
{
    public interface IImage : IDisposable
    {
        #region Events

        event EventHandler Loaded;

        #endregion Events

        #region Properties

        byte[] Data
        {
            get;
        }

        Error Error
        {
            get;
        }

        ImageFormat Format
        {
            get;
        }

        string ImageId
        {
            get;
        }

        bool IsLoaded
        {
            get;
        }

        #endregion Properties
    }
}