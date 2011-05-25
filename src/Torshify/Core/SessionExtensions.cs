using System;
using System.Linq.Expressions;

using Torshify.Core.Native;

namespace Torshify.Core
{
    internal static class SessionExtensions
    {
        public static void QueueThis<T, TEventArgs>(this NativeSession session, Expression<Func<T, Action<TEventArgs>>> expr, T s, params object[] args)
            where TEventArgs : EventArgs
        {
            session.Queue(DelegateInvoker.CreateInvoker(expr, s, args));
        }

        public static void QueueThis<T, TEventArgs>(this INativeObject nativeObject, Expression<Func<T, Action<TEventArgs>>> expr, T s, params object[] args)
            where TEventArgs : EventArgs
        {
            NativeSession nativeSession = nativeObject.Session as NativeSession;
            nativeSession.QueueThis(expr, s, args);
        }

        public static void QueueThis<T, TEventArgs>(this IManagedObject managedObject, Expression<Func<T, Action<TEventArgs>>> expr, T s, params object[] args)
            where TEventArgs : EventArgs
        {
            NativeSession nativeSession = managedObject.NativeObject.Session as NativeSession;
            nativeSession.QueueThis(expr, s, args);
        }

        public static IntPtr GetHandle(this ISession session)
        {
            INativeObject nativeObject = session as INativeObject;
            IManagedObject managedObject = session as IManagedObject;

            if (managedObject != null)
            {
                return managedObject.NativeObject.Handle;
            }

            if (nativeObject != null)
            {
                return nativeObject.Handle;
            }

            return IntPtr.Zero;
        }

        public static IntPtr GetHandle(this ISessionObject sessionObject)
        {
            INativeObject nativeObject = sessionObject as INativeObject;
            IManagedObject managedObject = sessionObject as IManagedObject;

            if (managedObject != null)
            {
                return managedObject.NativeObject.Handle;
            }

            if (nativeObject != null)
            {
                return nativeObject.Handle;
            }

            return IntPtr.Zero;
        }
    }
}