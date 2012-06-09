using System;

namespace Torshify.Core.Native
{
    internal class ErroneousAlbum : NativeObject, IAlbum
    {
        #region Constructors

        public ErroneousAlbum(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Properties

        public bool IsLoaded
        {
            get { return false; }
        }

        public IArtist Artist
        {
            get { return new ErroneousArtist(Session, Handle); }
        }

        public string CoverId
        {
            get { return string.Empty; }
        }

        public bool IsAvailable
        {
            get { return false; }
        }

        public string Name
        {
            get { return string.Empty; }
        }

        public AlbumType Type
        {
            get { return AlbumType.Unknown; }
        }

        public int Year
        {
            get { return DateTime.MinValue.Year; }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
        }

        #endregion Public Methods
    }
}