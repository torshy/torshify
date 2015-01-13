using System;

using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativeSearch : NativeObject, ISearch
    {
        #region Fields

        private readonly int _albumCount;
        private readonly int _albumOffset;
        private readonly int _artistCount;
        private readonly int _artistOffset;
        private readonly int _playlistCount;
        private readonly int _playlistOffset;
        private readonly string _query;
        private readonly SearchType _searchType;
        private readonly int _trackCount;
        private readonly int _trackOffset;
        private readonly object _userData;

        private Lazy<DelegateArray<IAlbum>> _albumsLazyLoad;
        private Lazy<DelegateArray<IArtist>> _artistsLazyLoad;
        private NativeSearchCallbacks _callbacks;
        private Lazy<DelegateArray<IPlaylistSearchResult>> _playlistsLazyLoad;
        private Lazy<DelegateArray<ITrack>> _tracksLazyLoad;

        #endregion Fields

        #region Constructors

        public NativeSearch(
            ISession session,
            string query,
            int trackOffset,
            int trackCount,
            int albumOffset,
            int albumCount,
            int artistOffset,
            int artistCount,
            int playlistOffset,
            int playlistCount,
            SearchType searchType,
            object userData)
            : base(session, IntPtr.Zero)
        {
            _query = query;
            _trackOffset = trackOffset;
            _trackCount = trackCount;
            _albumOffset = albumOffset;
            _albumCount = albumCount;
            _artistOffset = artistOffset;
            _artistCount = artistCount;
            _playlistOffset = playlistOffset;
            _playlistCount = playlistCount;
            _searchType = searchType;
            _userData = userData;
        }

        #endregion Constructors

        #region Events

        public event EventHandler<SearchEventArgs> Completed;

        #endregion Events

        #region Properties

        public IArray<IAlbum> Albums
        {
            get
            {
                AssertHandle();

                return _albumsLazyLoad.Value;
            }
        }

        public IArray<IArtist> Artists
        {
            get
            {
                AssertHandle();

                return _artistsLazyLoad.Value;
            }
        }

        public IArray<IPlaylistSearchResult> Playlists
        {
            get
            {
                AssertHandle();

                return _playlistsLazyLoad.Value;
            }
        }

        public string DidYouMean
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_search_did_you_mean(Handle);
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
                    return Spotify.sp_search_error(Handle);
                }
            }
        }

        public string Query
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_search_query(Handle);
                }
            }
        }

        public int TotalAlbums
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_search_total_albums(Handle);
                }
            }
        }

        public int TotalArtists
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_search_total_artists(Handle);
                }
            }
        }

        public int TotalTracks
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_search_total_tracks(Handle);
                }
            }
        }

        public int TotalPlaylists
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_search_total_playlists(Handle);
                }
            }
        }

        public bool IsComplete
        {
            get;
            private set;
        }

        public IArray<ITrack> Tracks
        {
            get
            {
                AssertHandle();

                return _tracksLazyLoad.Value;
            }
        }

        protected NativeSearchCallbacks Callbacks
        {
            get
            {
                return _callbacks;
            }
            set
            {
                _callbacks = value;
            }
        }

        protected Lazy<DelegateArray<IArtist>> ArtistsLazyLoad
        {
            get
            {
                return _artistsLazyLoad;
            }
            set
            {
                _artistsLazyLoad = value;
            }
        }

        protected Lazy<DelegateArray<ITrack>> TracksLazyLoad
        {
            get
            {
                return _tracksLazyLoad;
            }
            set
            {
                _tracksLazyLoad = value;
            }
        }

        protected Lazy<DelegateArray<IAlbum>> AlbumsLazyLoad
        {
            get
            {
                return _albumsLazyLoad;
            }
            set
            {
                _albumsLazyLoad = value;
            }
        }

        #endregion Properties

        #region Methods

        public override void Initialize()
        {
            _callbacks = new NativeSearchCallbacks(this, _userData);
            _tracksLazyLoad = new Lazy<DelegateArray<ITrack>>(() => new DelegateArray<ITrack>(GetNumberOfTracks, GetTrackIndex));
            _albumsLazyLoad = new Lazy<DelegateArray<IAlbum>>(() => new DelegateArray<IAlbum>(GetNumberOfAlbums, GetAlbumIndex));
            _artistsLazyLoad = new Lazy<DelegateArray<IArtist>>(() => new DelegateArray<IArtist>(GetNumberOfArtists, GetArtistIndex));
            _playlistsLazyLoad = new Lazy<DelegateArray<IPlaylistSearchResult>>(() => new DelegateArray<IPlaylistSearchResult>(GetNumberOfPlaylists, GetPlaylistAtIndex));

            lock (Spotify.Mutex)
            {
                Handle = Spotify.sp_search_create(
                    Session.GetHandle(),
                    _query,
                    _trackOffset,
                    _trackCount,
                    _albumOffset,
                    _albumCount,
                    _artistOffset,
                    _artistCount,
                    _playlistOffset,
                    _playlistCount,
                    _searchType,
                    _callbacks.CallbackHandle,
                    _callbacks.UserDataHandle);
            }
        }

        internal void OnComplete(SearchEventArgs e)
        {
            IsComplete = true;

            Completed.RaiseEvent(this, e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed
            }

            if (!IsInvalid)
            {
                // Dispose unmanaged
                try
                {
                    if (_callbacks != null)
                    {
                        _callbacks.Dispose();
                        _callbacks = null;
                    }

                    lock (Spotify.Mutex)
                    {
                        Ensure(() => Spotify.sp_search_release(Handle));
                    }
                }
                catch
                {
                }
            }

            base.Dispose(disposing);
        }

        protected IArtist GetArtistIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return ArtistManager.Get(Session, Spotify.sp_search_artist(Handle, index));
            }
        }

        protected int GetNumberOfArtists()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_search_num_artists(Handle);
            }
        }

        protected IAlbum GetAlbumIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return AlbumManager.Get(Session, Spotify.sp_search_album(Handle, index));
            }
        }

        protected int GetNumberOfAlbums()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_search_num_albums(Handle);
            }
        }

        protected ITrack GetTrackIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return TrackManager.Get(Session, Spotify.sp_search_track(Handle, index));
            }
        }

        protected int GetNumberOfTracks()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_search_num_tracks(Handle);
            }
        }

        private IPlaylistSearchResult GetPlaylistAtIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return new NativePlaylistSearchResult
                {
                    Name = Spotify.sp_search_playlist_name(Handle, index),
                    ImageUri = Spotify.sp_search_playlist_image_uri(Handle, index),
                    Uri = Spotify.sp_search_playlist_uri(Handle, index)
                };
            }
        }

        private int GetNumberOfPlaylists()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_search_num_playlists(Handle);
            }
        }

        #endregion Methods
    }
}