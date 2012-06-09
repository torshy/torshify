using System;

namespace Torshify.Core
{
    internal class DelegateInvoker
    {
        #region Fields

        private readonly Action _invoker;

        #endregion Fields

        #region Constructors

        internal DelegateInvoker(Action invoker)
        {
            _invoker = invoker;
        }

        #endregion Constructors

        #region Methods

        internal virtual void Execute()
        {
            if (_invoker != null)
            {
                _invoker();
            }
        }

        #endregion Methods
    }
}