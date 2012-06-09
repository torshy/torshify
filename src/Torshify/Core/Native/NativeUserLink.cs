using System;
using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativeUserLink : NativeLink, ILink<IUser>
    {
        #region Fields

        private Lazy<IUser> _artist;

        #endregion Fields

        #region Constructors

        public NativeUserLink(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Properties

        public override object Object
        {
            get { return _artist.Value; }
        }

        IUser ILink<IUser>.Object
        {
            get { return (IUser)Object; }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            _artist = new Lazy<IUser>(() =>
                                            {
                                                AssertHandle();

                                                lock (Spotify.Mutex)
                                                {
                                                    return UserManager.Get(Session, Spotify.sp_link_as_user(Handle));
                                                }
                                            });
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }

        #endregion Protected Methods
    }
}