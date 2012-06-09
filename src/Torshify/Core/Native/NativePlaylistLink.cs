using System;
using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativePlaylistLink : NativeLink, ILink<IPlaylist>
    {
        #region Fields

        private Lazy<IPlaylist> _playlist;

        #endregion Fields

        #region Constructors

        public NativePlaylistLink(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Properties

        public override object Object
        {
            get { return _playlist.Value; }
        }

        IPlaylist ILink<IPlaylist>.Object
        {
            get { return (IPlaylist)Object; }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            _playlist = new Lazy<IPlaylist>(() =>
                                                {
                                                    AssertHandle();

                                                    lock (Spotify.Mutex)
                                                    {
                                                        return PlaylistManager.Get(Session, Spotify.sp_playlist_create(Session.GetHandle(), Handle));
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