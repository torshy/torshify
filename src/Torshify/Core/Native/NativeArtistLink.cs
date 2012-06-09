using System;
using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativeArtistLink : NativeLink, ILink<IArtist>
    {
        #region Fields

        private Lazy<IArtist> _artist;

        #endregion Fields

        #region Constructors

        public NativeArtistLink(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Properties

        public override object Object
        {
            get { return _artist.Value; }
        }

        IArtist ILink<IArtist>.Object
        {
            get { return (IArtist)Object; }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            _artist = new Lazy<IArtist>(
                () =>
                {
                    AssertHandle();

                    lock (Spotify.Mutex)
                    {
                        return ArtistManager.Get(Session, Spotify.sp_link_as_artist(Handle));
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