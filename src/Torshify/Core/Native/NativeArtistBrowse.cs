using System;

using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativeArtistBrowse : NativeObject, IArtistBrowse
    {
        #region Fields

        private readonly IntPtr _artistToBrowse;
        private readonly ArtistBrowseType _browseType;

        private Spotify.ArtistBrowseCompleteCallback _browseCompleteCallback;
        private DelegateArray<ITrack> _tracks;
        private DelegateArray<ITrack> _topHitTracks;
        private DelegateArray<IAlbum> _albums;
        private DelegateArray<IArtist> _similarArtists;
        private DelegateArray<IImage> _portraits;

        #endregion Fields

        #region Constructors

        public NativeArtistBrowse(ISession session, IntPtr artistToBrowse, ArtistBrowseType browseType)
            : base(session, IntPtr.Zero)
        {
            _artistToBrowse = artistToBrowse;
            _browseType = browseType;
        }

        #endregion Constructors

        #region Events

        public event EventHandler Completed;

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
                    return Spotify.sp_artistbrowse_is_loaded(Handle);
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
                    return Spotify.sp_artistbrowse_error(Handle);
                }
            }
        }

        public IArtist Artist
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return ArtistManager.Get(Session, Spotify.sp_artistbrowse_artist(Handle));
                }
            }
        }

        public IArray<IImage> Portraits
        {
            get
            {
                AssertHandle();

                return _portraits;
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

        public IArray<ITrack> TopHitTracks
        {
            get
            {
                AssertHandle();

                return _topHitTracks;
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

        public IArray<IArtist> SimilarArtists
        {
            get
            {
                AssertHandle();

                return _similarArtists;
            }
        }

        public string Biography
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_artistbrowse_biography(Handle);
                }
            }
        }

        public TimeSpan BackendRequestDuration
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return TimeSpan.FromMilliseconds(Spotify.sp_artistbrowse_backend_request_duration(Handle));
                }
            }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            _browseCompleteCallback = OnBrowseCompleteCallback;
            _tracks = new DelegateArray<ITrack>(GetNumberOfTracks, GetTrackAtIndex);
            _topHitTracks = new DelegateArray<ITrack>(GetNumberOfTopHitTracks, GetTopHitTrackAtIndex);
            _albums = new DelegateArray<IAlbum>(GetNumberOfAlbums, GetAlbumAtIndex);
            _similarArtists = new DelegateArray<IArtist>(GetNumberOfSimilarArtists, GetSimilarArtistAtIndex);
            _portraits = new DelegateArray<IImage>(GetNumberOfPortraits, GetPortraitAtIndex);

            lock (Spotify.Mutex)
            {
                Handle = Spotify.sp_artistbrowse_create(
                            Session.GetHandle(),
                            _artistToBrowse,
                            _browseType,
                            _browseCompleteCallback,
                            IntPtr.Zero);
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

            // Dispose unmanaged
            if (!IsInvalid)
            {
                try
                {
                    lock (Spotify.Mutex)
                    {
                        Ensure(() => Spotify.sp_artistbrowse_release(Handle));
                    }

                    GC.KeepAlive(_browseCompleteCallback);
                }
                catch
                {
                }
            }

            base.Dispose(disposing);
        }

        #endregion Protected Methods

        #region Private Methods

        private IImage GetPortraitAtIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                IntPtr id = Spotify.sp_artistbrowse_portrait(Handle, index);
                string stringId = Spotify.ImageIdToString(id);
                return Session.GetImage(stringId);
            }
        }

        private int GetNumberOfPortraits()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_artistbrowse_num_portraits(Handle);
            }
        }

        private IArtist GetSimilarArtistAtIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return ArtistManager.Get(Session, Spotify.sp_artistbrowse_similar_artist(Handle, index));
            }
        }

        private int GetNumberOfSimilarArtists()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_artistbrowse_num_similar_artists(Handle);
            }
        }

        private IAlbum GetAlbumAtIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return AlbumManager.Get(Session, Spotify.sp_artistbrowse_album(Handle, index));
            }
        }

        private int GetNumberOfAlbums()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_artistbrowse_num_albums(Handle);
            }
        }

        private ITrack GetTrackAtIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return TrackManager.Get(Session, Spotify.sp_artistbrowse_track(Handle, index));
            }
        }

        private int GetNumberOfTracks()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_artistbrowse_num_tracks(Handle);
            }
        }

        private ITrack GetTopHitTrackAtIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return TrackManager.Get(Session, Spotify.sp_artistbrowse_tophit_track(Handle, index));
            }
        }

        private int GetNumberOfTopHitTracks()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_artistbrowse_num_tophit_tracks(Handle);
            }
        }

        private void OnBrowseCompleteCallback(IntPtr browseptr, IntPtr userdataptr)
        {
            if (browseptr != Handle)
            {
                return;
            }

            this.QueueThis(() => OnBrowseComplete(new UserDataEventArgs(EventArgs.Empty)));
        }

        private void OnBrowseComplete(EventArgs e)
        {
            IsComplete = true;

            Completed.RaiseEvent(this, e);
        }

        #endregion Private Methods
    }
}