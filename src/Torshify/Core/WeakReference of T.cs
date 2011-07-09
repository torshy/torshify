using System;
using System.Runtime.InteropServices;

namespace Torshify.Core
{
    internal sealed class WeakReference<T> : IDisposable
        where T : class
    {
        #region Fields

        private GCHandle _handle;

        #endregion Fields

        #region Constructors

        public WeakReference(T obj)
        {
            _handle = GCHandle.Alloc(obj, GCHandleType.Weak);
        }

        ~WeakReference()
        {
            _handle.Free();
        }

        #endregion Constructors

        #region Properties

        public bool IsAlive
        {
            get { return _handle.Target != null; }
        }

        public T Target
        {
            get { return _handle.IsAllocated ? _handle.Target as T : null; }
        }

        #endregion Properties

        #region Methods

        public static bool operator !=(WeakReference<T> a, WeakReference<T> b)
        {
            return !(a == b);
        }

        public static bool operator ==(WeakReference<T> a, WeakReference<T> b)
        {
            return a.Equals(b);
        }

        public void Dispose()
        {
            _handle.Free();
            GC.SuppressFinalize(this);
        }

        public override bool Equals(object obj)
        {
            return obj != null && _handle.Equals(((WeakReference<T>)obj)._handle);
        }

        public override int GetHashCode()
        {
            return _handle.GetHashCode();
        }

        #endregion Methods
    }
}