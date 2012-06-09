using System;

namespace Torshify
{
    public class TorshifyException : Exception
    {
        #region Constructors

        public TorshifyException(string message, Error error)
            : base(message)
        {
            Error = error;
        }

        #endregion Constructors

        #region Properties

        public Error Error
        {
            get; private set;
        }

        #endregion Properties
    }
}