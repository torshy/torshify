using System;
using System.Linq.Expressions;

namespace Torshify.Core
{
    internal class DelegateInvoker
    {
        #region Fields

        private readonly object[] _args;
        private readonly Delegate _handler;

        #endregion Fields

        #region Constructors

        internal DelegateInvoker(Delegate handler, params object[] args)
        {
            _handler = handler;
            _args = args;
        }

        #endregion Constructors

        #region Public Static Methods

        public static Delegate CreateDelegate<T, TEventArgs>(Expression<Func<T, Action<TEventArgs>>> expr, T s)
            where TEventArgs : EventArgs
        {
            return expr.Compile().Invoke(s);
        }

        public static DelegateInvoker CreateInvoker<T, TEventArgs>(Expression<Func<T, Action<TEventArgs>>> expr, T s, params object[] args)
            where TEventArgs : EventArgs
        {
            return new DelegateInvoker(CreateDelegate(expr, s), args);
        }

        #endregion Public Static Methods

        #region Internal Methods

        internal void Execute()
        {
            if(_handler != null)
            {
                _handler.DynamicInvoke(_args);
            }
        }

        #endregion Internal Methods
    }
}