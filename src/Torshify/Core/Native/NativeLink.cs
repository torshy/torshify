using System;
using System.Runtime.InteropServices;
using System.Text;
using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal abstract class NativeLink : NativeObject, ILink
    {
        #region Constructors

        protected NativeLink(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Properties

        public LinkType Type
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_link_type(Handle);
                }
            }
        }

        public abstract object Object
        {
            get;
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            lock (Spotify.Mutex)
            {
                Spotify.sp_link_add_ref(Handle);
            }
        }

        public string GetStringLink()
        {
            return ToString();
        }

        public override string ToString()
        {
            if (IsInvalid)
            {
                return string.Empty;
            }

            IntPtr bufferPtr = IntPtr.Zero;
            try
            {
                int size = Spotify.STRINGBUFFER_SIZE;
                bufferPtr = Marshal.AllocHGlobal(size);

                lock (Spotify.Mutex)
                {
                    Spotify.sp_link_as_string(Handle, bufferPtr, size);
                }

                return Spotify.GetString(bufferPtr, string.Empty);
            }
            finally
            {
                try
                {
                    Marshal.FreeHGlobal(bufferPtr);
                }
                catch
                {
                }
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            // Dipose managed
            if (disposing)
            {
            }

            if (!IsInvalid)
            {
                // Dipose unmanaged
                try
                {
                    lock (Spotify.Mutex)
                    {
                        Ensure(() => Spotify.sp_link_release(Handle));
                    }
                }
                catch
                {
                }
                finally
                {
                    LinkManager.Remove(Handle);
                }

#if DEBUG_VERBOSE
                Console.WriteLine("Link disposed");
#endif
            }

            base.Dispose(disposing);
        }

        #endregion Protected Methods
    }
}