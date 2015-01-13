using System;

namespace Torshify.Core.Native
{
    internal class NativeUser : NativeObject, IUser
    {
        #region Constructors

        public NativeUser(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Properties

        public string CanonicalName
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_user_canonical_name(Handle);
                }
            }
        }

        public string DisplayName
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_user_display_name(Handle);
                }
            }
        }

        public bool IsLoaded
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_user_is_loaded(Handle);
                }
            }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            lock (Spotify.Mutex)
            {
                Spotify.sp_user_add_ref(Handle);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed
            }

            if (!IsInvalid)
            {
                try
                {
                    lock (Spotify.Mutex)
                    {
                        Ensure(() => Spotify.sp_user_release(Handle));
                    }
                }
                catch
                {
                }
            }

            base.Dispose(disposing);
        }

        #endregion Protected Methods
    }
}