using System;
using System.Runtime.InteropServices;
using Torshify.Core;

namespace Torshify.Core.Native
{
    internal class NativeSearchCallbacks : IDisposable
    {
        private readonly NativeSearch _search;

        private delegate void SearchCompleteCallback(IntPtr searchPtr, IntPtr userdataPtr);

        private SearchCompleteCallback _searchComplete;
        private IntPtr _callbackHandle;
        private GCHandle _userDataHandle;

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

        public IntPtr UserDataHandle
        {
            get { return _userDataHandle.IsAllocated ? GCHandle.ToIntPtr(_userDataHandle) : IntPtr.Zero; }
        }

        public IntPtr CallbackHandle
        {
            get { return _callbackHandle; }
        }

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

            _search.QueueThis<NativeSearch, SearchEventArgs>(
                s=>s.OnComplete,
                _search,
                new SearchEventArgs(userData));
        }

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
    }
}