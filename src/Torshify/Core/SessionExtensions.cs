using System;

using Torshify.Core.Native;

namespace Torshify.Core
{
    internal static class SessionExtensions
    {
        public static void QueueThis(this INativeObject nativeObject, Action action)
        {
            QueueThis(nativeObject, new DelegateInvoker(action));
        }

        public static void QueueThis(this INativeObject nativeObject, DelegateInvoker invoker)
        {
            NativeSession nativeSession = (NativeSession)nativeObject.Session;
            nativeSession.Queue(invoker);
        }

        public static IntPtr GetHandle(this ISession session)
        {
            INativeObject nativeObject = session as INativeObject;

            if (nativeObject != null)
            {
                return nativeObject.Handle;
            }

            return IntPtr.Zero;
        }

        public static IntPtr GetHandle(this ISessionObject sessionObject)
        {
            INativeObject nativeObject = sessionObject as INativeObject;

            if (nativeObject != null)
            {
                return nativeObject.Handle;
            }

            return IntPtr.Zero;
        }
    }
}