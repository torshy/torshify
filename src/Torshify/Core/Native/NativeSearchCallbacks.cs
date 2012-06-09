using System;
using System.Runtime.InteropServices;

namespace Torshify.Core.Native
{
    internal class NativeSearchCallbacks : IDisposable
    {
        #region Fields

        private readonly NativeSearch _search;

        private SearchCompleteCallback _searchComplete;
        private IntPtr _callbackHandle;
        private GCHandle _userDataHandle;

        #endregion Fields

        #region Constructors

        public NativeSearchCallbacks(NativeSearch search, object userData)
        {
            _search = search;
            _searchComplete = OnSearchCompleteCallback;
            _callbackHandle = Marshal.GetFunctionPointerForDelegate(_searchComplete);

            if (userData != null)
            {
                _userDataHandle = GCHandle.Alloc(userData);
            }
        }

        #endregion Constructors

        #region Delegates

        private delegate void SearchCompleteCallback(IntPtr searchPtr, IntPtr userdataPtr);

        #endregion Delegates

        #region Properties

        public IntPtr UserDataHandle
        {
            get { return _userDataHandle.IsAllocated ? GCHandle.ToIntPtr(_userDataHandle) : IntPtr.Zero; }
        }

        public IntPtr CallbackHandle
        {
            get { return _callbackHandle; }
        }

        #endregion Properties

        #region Public Methods

        public void Dispose()
        {
            try
            {
                if (_userDataHandle.IsAllocated)
                {
                    _userDataHandle.Free();
                }
            }
            catch
            {
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void OnSearchCompleteCallback(IntPtr searchptr, IntPtr userdataptr)
        {
            object userData = null;

            if (userdataptr != IntPtr.Zero)
            {
                GCHandle handle = GCHandle.FromIntPtr(userdataptr);
                userData = handle.Target;
                handle.Free();

                if (_userDataHandle.IsAllocated)
                {
                    _userDataHandle.Free();
                }
            }

            _search.QueueThis(() => _search.OnComplete(new SearchEventArgs(userData)));
        }

        #endregion Private Methods
    }
}