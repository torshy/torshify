using System;

using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativeAlbumLink : NativeLink, ILink<IAlbum>
    {
        #region Fields

        private Lazy<IAlbum> _album;

        #endregion Fields

        #region Constructors

        public NativeAlbumLink(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Properties

        public override object Object
        {
            get { return _album.Value; }
        }

        IAlbum ILink<IAlbum>.Object
        {
            get { return (IAlbum)Object; }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            _album = new Lazy<IAlbum>(
                () =>
                {
                    AssertHandle();

                    lock (Spotify.Mutex)
                    {
                        return AlbumManager.Get(Session, Spotify.sp_link_as_album(Handle));
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