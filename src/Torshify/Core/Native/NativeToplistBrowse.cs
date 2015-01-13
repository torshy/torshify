using System;
using System.Runtime.InteropServices;
using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativeToplistBrowse : NativeObject, IToplistBrowse
    {
        #region Fields

        private readonly ToplistType _toplistType;
        private readonly int _region;
        private readonly object _userData;
        private readonly string _userName;

        private DelegateArray<IArtist> _artists;
        private DelegateArray<IAlbum> _albums;
        private DelegateArray<ITrack> _tracks;
        private Spotify.ToplistBrowseCompleteCallback _browseComplete;
        private GCHandle _userDataHandle;

        #endregion Fields

        #region Constructor

        public NativeToplistBrowse(ISession session, ToplistType toplistType, int region, object userData = null)
            : base(session, IntPtr.Zero)
        {
            _toplistType = toplistType;
            _region = region;
            _userData = userData;
        }

        public NativeToplistBrowse(ISession session, ToplistType toplistType, object userData = null)
            : this(session, toplistType, (int)ToplistSpecialRegion.Everywhere, userData)
        {
        }

        public NativeToplistBrowse(ISession session, ToplistType toplistType, string userName, object userData = null)
            : this(session, toplistType, (int)ToplistSpecialRegion.User, userData)
        {
            _userName = userName;
        }

        #endregion Constructors

        #region Events

        public event EventHandler<UserDataEventArgs> Completed;

        #endregion Events

        #region Properties

        public bool IsComplete
        {
            get;
            private set;
        }

        public bool IsLoaded
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_toplistbrowse_is_loaded(Handle);
                }
            }
        }

        public Error Error
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_toplistbrowse_error(Handle);
                }
            }
        }

        public IArray<IArtist> Artists
        {
            get
            {
                AssertHandle();

                return _artists;
            }
        }

        public IArray<IAlbum> Albums
        {
            get
            {
                AssertHandle();

                return _albums;
            }
        }

        public IArray<ITrack> Tracks
        {
            get
            {
                AssertHandle();

                return _tracks;
            }
        }

        public TimeSpan BackendRequestDuration
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return TimeSpan.FromMilliseconds(Spotify.sp_toplistbrowse_backend_request_duration(Handle));
                }
            }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            _browseComplete = new Spotify.ToplistBrowseCompleteCallback(OnBrowseCompleteCallback);
            _tracks = new DelegateArray<ITrack>(GetNumberOfTracks, GetTrackAtIndex);
            _albums = new DelegateArray<IAlbum>(GetNumberOfAlbums, GetAlbumAtIndex);
            _artists = new DelegateArray<IArtist>(GetNumberOfArtists, GetArtistAtIndex);

            if (_userData != null)
            {
                _userDataHandle = GCHandle.Alloc(_userData);
            }

            lock (Spotify.Mutex)
            {
                Handle = Spotify.sp_toplistbrowse_create(
                            Session.GetHandle(),
                            _toplistType,
                            _region,
                            _userName,
                            _browseComplete,
                            _userDataHandle.IsAllocated ? GCHandle.ToIntPtr(_userDataHandle) : IntPtr.Zero);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            // Dispose managed
            if (disposing)
            {
            }

            if (!IsInvalid)
            {
                try
                {
                    lock (Spotify.Mutex)
                    {
                        Spotify.sp_toplistbrowse_release(Handle);
                    }
                }
                catch
                {
                }
            }

            base.Dispose(disposing);
        }

        #endregion Protected Methods

        #region Private Methods

        private int GetNumberOfArtists()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_toplistbrowse_num_artists(Handle);
            }
        }

        private IArtist GetArtistAtIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return ArtistManager.Get(Session, Spotify.sp_toplistbrowse_artist(Handle, index));
            }
        }

        private int GetNumberOfAlbums()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_toplistbrowse_num_albums(Handle);
            }
        }

        private IAlbum GetAlbumAtIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return AlbumManager.Get(Session, Spotify.sp_toplistbrowse_album(Handle, index));
            }
        }

        private int GetNumberOfTracks()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_toplistbrowse_num_tracks(Handle);
            }
        }

        private ITrack GetTrackAtIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return TrackManager.Get(Session, Spotify.sp_toplistbrowse_track(Handle, index));
            }
        }

        private void OnBrowseCompleteCallback(IntPtr toplisthandle, IntPtr userdataptr)
        {
            if (toplisthandle != Handle)
            {
                return;
            }

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

            this.QueueThis(() => OnBrowseComplete(new UserDataEventArgs(userData)));
        }

        private void OnBrowseComplete(UserDataEventArgs args)
        {
            IsComplete = true;

            Completed.RaiseEvent(this, args);
        }

        #endregion Private Methods
    }
}