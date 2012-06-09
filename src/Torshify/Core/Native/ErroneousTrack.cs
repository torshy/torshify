using System;

namespace Torshify.Core.Native
{
    internal class ErroneousTrack : NativeObject, IPlaylistTrack
    {
        #region Fields

        private readonly Error _error;
        private readonly IPlaylist _playlist;

        private IArray<IArtist> _artists;

        #endregion Fields

        #region Constructors

        public ErroneousTrack(ISession session, IntPtr handle, Error error, IPlaylist playlist = null)
            : base(session, handle)
        {
            _error = error;
            _playlist = playlist;
            _artists = new DelegateArray<IArtist>(() => 0, index => null);
        }

        #endregion Constructors

        #region Properties

        public IAlbum Album
        {
            get
            {
                return new ErroneousAlbum(Session, Handle);
            }
        }

        public IArray<IArtist> Artists
        {
            get { return _artists; }
        }

        public int Disc
        {
            get { return 0; }
        }

        public TimeSpan Duration
        {
            get { return TimeSpan.Zero; }
        }

        public Error Error
        {
            get { return _error; }
        }

        public int Index
        {
            get { return 0; }
        }

        public TrackAvailablity Availability
        {
            get { return TrackAvailablity.Unavailable; }
        }

        public TrackOfflineStatus OfflineStatus
        {
            get { return TrackOfflineStatus.No; }
        }

        public bool IsStarred
        {
            get { return false; }
            set { }
        }

        public bool IsLocal
        {
            get { return false; }
        }

        public bool IsAutolinked
        {
            get { return false; }
        }

        public string Name
        {
            get { return Error.GetMessage(); }
        }

        public int Popularity
        {
            get { return 0; }
        }

        public ITrack AutolinkedTrack
        {
            get { return null; }
        }

        public bool IsLoaded
        {
            get { return false; }
        }

        public DateTime CreateTime
        {
            get { return DateTime.MinValue; }
        }

        public bool Seen
        {
            get { return false; }
        }

        public IPlaylist Playlist
        {
            get { return _playlist; }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
        }

        #endregion Public Methods
    }
}