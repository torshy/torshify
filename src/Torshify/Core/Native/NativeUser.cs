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

        public string FullName
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_user_full_name(Handle);
                }
            }
        }

        public string Picture
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_user_picture(Handle);
                }
            }
        }

        public RelationType Relation
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_user_relation_type(Session.GetHandle(), Handle);
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
                        Spotify.sp_user_release(Handle);
                    }
                }
                catch
                {
                }
                finally
                {
                    Handle = IntPtr.Zero;
                }
            }

            base.Dispose(disposing);
        }

        #endregion Protected Methods
    }
}