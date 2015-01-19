using System;
using System.Runtime.InteropServices;

using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativeAlbumBrowse : NativeObject, IAlbumBrowse
    {
        #region Fields

        private readonly IAlbum _albumToBrowse;
        private readonly object _userData;

        private Spotify.AlbumBrowseCompleteCallback _browseComplete;
        private DelegateArray<ITrack> _tracks;
        private DelegateArray<string> _copyrights;
        private GCHandle _userDataHandle;

        #endregion Fields

        #region Constructors

        public NativeAlbumBrowse(ISession session, IAlbum albumToBrowse, object userData = null)
            : base(session, IntPtr.Zero)
        {
            _albumToBrowse = albumToBrowse;
            _userData = userData;
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

        public IAlbum Album
        {
            get { return _albumToBrowse; }
        }

        public IArtist Artist
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return ArtistManager.Get(Session, Spotify.sp_albumbrowse_artist(Handle));
                }
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
                    return TimeSpan.FromMilliseconds(Spotify.sp_albumbrowse_backend_request_duration(Handle));
                }
            }
        }

        public IArray<string> Copyrights
        {
            get
            {
                AssertHandle();

                return _copyrights;
            }
        }

        public Error Error
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_albumbrowse_error(Handle);
                }
            }
        }

        public string Review
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_albumbrowse_review(Handle);
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
                    return Spotify.sp_albumbrowse_is_loaded(Handle);
                }
            }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            _browseComplete = new Spotify.AlbumBrowseCompleteCallback(OnBrowseCompleteCallback);
            _tracks = new DelegateArray<ITrack>(GetNumberOfTracks, GetTrackAtIndex);
            _copyrights = new DelegateArray<string>(GetNumberOfCopyrights, GetCopyrightAtIndex);

            if (_userData != null)
            {
                _userDataHandle = GCHandle.Alloc(_userData);
            }

            lock (Spotify.Mutex)
            {
                Handle = Spotify.sp_albumbrowse_create(
                            Session.GetHandle(),
                            _albumToBrowse.GetHandle(),
                            _browseComplete,
                             _userDataHandle.IsAllocated ? GCHandle.ToIntPtr(_userDataHandle) : IntPtr.Zero);
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
                        Ensure(() => Spotify.sp_albumbrowse_release(Handle));
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

        private string GetCopyrightAtIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_albumbrowse_copyright(Handle, index);
            }
        }

        private int GetNumberOfCopyrights()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_albumbrowse_num_copyrights(Handle);
            }
        }

        private ITrack GetTrackAtIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return TrackManager.Get(Session, Spotify.sp_albumbrowse_track(Handle, index));
            }
        }

        private int GetNumberOfTracks()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_albumbrowse_num_tracks(Handle);
            }
        }

        private void OnBrowseCompleteCallback(IntPtr albumbrowseptr, IntPtr userdataptr)
        {
            if (albumbrowseptr != Handle)
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

        private void OnBrowseComplete(UserDataEventArgs e)
        {
            IsComplete = true;

            Completed.RaiseEvent(this, e);
        }

        #endregion Private Methods
    }
}