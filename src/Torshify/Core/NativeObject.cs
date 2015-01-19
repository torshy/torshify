using System;

namespace Torshify.Core
{
    internal abstract class NativeObject : INativeObject
    {
        #region Fields

        private readonly ISession _session;

        private IntPtr _handle;

        #endregion Fields

        #region Constructors

        protected NativeObject(ISession session, IntPtr handle)
        {
            _session = session;
            _handle = handle;
        }

        ~NativeObject()
        {
            Dispose(false);
        }

        #endregion Constructors

        #region Properties

        public virtual IntPtr Handle
        {
            get { return _handle; }
            protected set { _handle = value; }
        }

        public virtual ISession Session
        {
            get { return _session; }
        }

        protected bool IsInvalid
        {
            get { return Handle == IntPtr.Zero; }
        }

        #endregion Properties

        #region Public Methods

        public abstract void Initialize();

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj == null)
            {
                return false;
            }

            return GetHashCode() == obj.GetHashCode();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // get rid of managed resources
            }

            // get rid of unmanaged resources

            Handle = IntPtr.Zero;
        }

        protected void AssertHandle()
        {
            if (IsInvalid)
            {
                throw new InvalidOperationException("Handle is invalid");
            }
        }

        public void Ensure(Action action)
        {
            if ((Session is NativeObject) && !(Session as NativeObject).IsInvalid)
            {
                if (action != null)
                {
                    action();
                }
            }
        }

        #endregion Protected Methods
    }
}