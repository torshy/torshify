using System;

namespace Torshify.Core
{
    internal abstract class ManagedObject : IManagedObject
    {
        #region Fields

        private readonly INativeObject _nativeObject;

        #endregion Fields

        #region Constructors

        protected ManagedObject(INativeObject nativeObject)
        {
            _nativeObject = nativeObject;
        }

        ~ManagedObject()
        {
            Console.WriteLine("Finalize managed object");
            Dispose(false);
        }

        #endregion Constructors

        #region Properties

        public virtual INativeObject NativeObject
        {
            get { return _nativeObject; }
        }

        #endregion Properties

        #region Public Methods

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
        }

        #endregion Protected Methods
    }
}