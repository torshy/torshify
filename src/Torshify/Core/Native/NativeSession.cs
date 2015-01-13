using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativeSession : NativeObject, ISession
    {
        #region Fields

        private readonly byte[] _applicationKey;
        private readonly SessionOptions _options;

        private IPlaylistContainer _playlistContainer;
        private IPlaylist _starredPlaylist;
        private Thread _mainThread;
        private Thread _eventThread;
        private AutoResetEvent _mainThreadNotification;
        private Queue<DelegateInvoker> _eventQueue;
        private NativeSessionCallbacks _callbacks;
        private readonly object _eventQueueLock = new object();

        #endregion Fields

        #region Constructors

        public NativeSession(byte[] applicationKey, SessionOptions options)
            : base(null, IntPtr.Zero)
        {
            _applicationKey = applicationKey;
            _options = options;
        }

        #endregion Constructors

        #region Events

        public event EventHandler<SessionEventArgs> ConnectionError;

        public event EventHandler<SessionEventArgs> EndOfTrack;

        public event EventHandler<SessionEventArgs> Exception;

        public event EventHandler<SessionEventArgs> LoginComplete;

        public event EventHandler<SessionEventArgs> LogMessage;

        public event EventHandler<SessionEventArgs> LogoutComplete;

        public event EventHandler<SessionEventArgs> MessageToUser;

        public event EventHandler<SessionEventArgs> MetadataUpdated;

        public event EventHandler<MusicDeliveryEventArgs> MusicDeliver;

        public event EventHandler<SessionEventArgs> PlayTokenLost;

        public event EventHandler<SessionEventArgs> StartPlayback;

        public event EventHandler<SessionEventArgs> StopPlayback;

        public event EventHandler<SessionEventArgs> StreamingError;

        public event EventHandler<SessionEventArgs> UserinfoUpdated;

        public event EventHandler<SessionEventArgs> OfflineStatusUpdated;

        public event EventHandler<SessionEventArgs> OfflineError;

        public event EventHandler<CredentialsBlobEventArgs> CredentialsBlobUpdated;

        public event EventHandler<SessionEventArgs> ConnectionStateUpdated;

        public event EventHandler<SessionEventArgs> ScrobbleError;

        public event EventHandler<PrivateSessionModeChangedEventArgs> PrivateSessionModeChanged;

        #endregion Events

        #region Properties

        public IPlaylistContainer PlaylistContainer
        {
            get
            {
                ConnectionState connectionState = ConnectionState;

                if (connectionState == ConnectionState.Disconnected || connectionState == ConnectionState.LoggedOut)
                {
                    return null;
                }

                AssertHandle();

                return _playlistContainer;
            }
        }

        public IPlaylist Starred
        {
            get
            {
                if (ConnectionState != ConnectionState.LoggedIn)
                {
                    return null;
                }

                AssertHandle();

                if (_starredPlaylist == null)
                {
                    lock (Spotify.Mutex)
                    {
                        _starredPlaylist = PlaylistManager.Get(this, Spotify.sp_session_starred_create(Handle));
                    }
                }

                return _starredPlaylist;
            }
        }

        public ConnectionState ConnectionState
        {
            get
            {
                if (!IsInvalid)
                {
                    try
                    {
                        lock (Spotify.Mutex)
                        {
                            return Spotify.sp_session_connectionstate(Handle);
                        }
                    }
                    catch
                    {
                        return ConnectionState.Undefined;
                    }
                }

                return ConnectionState.Undefined;
            }
        }
        
        public IUser LoggedInUser
        {
            get
            {
                if (ConnectionState != ConnectionState.LoggedIn)
                {
                    return null;
                }

                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return UserManager.Get(this, Spotify.sp_session_user(Handle));
                }
            }
        }

        public int LoggedInUserCountry
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_session_user_country(Handle);
                }
            }
        }

        public bool IsVolumeNormalizationEnabled
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_session_get_volume_normalization(Handle);
                }
            }
            set
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    Error error = Spotify.sp_session_set_volume_normalization(Handle, value);

                    if (error != Error.OK)
                    {
                        throw new TorshifyException(error.GetMessage(), error);
                    }
                }
            }
        }

        public bool IsPrivateSessionEnabled
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_session_is_private_session(Handle);
                }
            }
            set
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    Error error = Spotify.sp_session_set_private_session(Handle, value);

                    if (error != Error.OK)
                    {
                        throw new TorshifyException(error.GetMessage(), error);
                    }
                }
            }
        }

        public override ISession Session
        {
            get
            {
                return this;
            }
        }

        #endregion Properties

        #region Public Methods

        public void Login(string userName, string password, bool rememberMe = false)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Error error = Spotify.sp_session_login(Handle, userName, password, rememberMe, null);

                if (error != Error.OK)
                {
                    throw new TorshifyException(error.GetMessage(), error);
                }
            }
        }

        public void LoginWithBlob(string userName, string blob)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Error error = Spotify.sp_session_login(Handle, userName, null, false, blob);

                if (error != Error.OK)
                {
                    throw new TorshifyException(error.GetMessage(), error);
                }
            }
        }

        public void Relogin()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                var error = Spotify.sp_session_relogin(Handle);

                if (error != Error.OK)
                {
                    throw new TorshifyException(error.GetMessage(), error);
                }
            }
        }

        public void ForgetStoredLogin()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Error error = Spotify.sp_session_forget_me(Handle);

                if (error != Error.OK)
                {
                    throw new TorshifyException(error.GetMessage(), error);
                }
            }
        }

        public string GetRememberedUser()
        {
            AssertHandle();

            int bufferSize = Spotify.STRINGBUFFER_SIZE;

            try
            {
                int userNameLength;
                StringBuilder builder = new StringBuilder(bufferSize);

                lock (Spotify.Mutex)
                {
                    userNameLength = Spotify.sp_session_remembered_user(Handle, builder, bufferSize);
                }

                if (userNameLength == -1)
                {
                    return string.Empty;
                }

                return builder.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public void Logout()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Error error = Spotify.sp_session_logout(Handle);

                if (error != Error.OK)
                {
                    throw new TorshifyException(error.GetMessage(), error);
                }
            }
        }

        public Error PlayerLoad(ITrack track)
        {
            AssertHandle();

            INativeObject nativeObject = track as INativeObject;

            if (nativeObject == null)
            {
                throw new ArgumentException("Invalid argument");
            }

            lock (Spotify.Mutex)
            {
                return Spotify.sp_session_player_load(Handle, nativeObject.Handle);
            }
        }

        public Error PlayerPrefetch(ITrack track)
        {
            AssertHandle();

            INativeObject nativeObject = track as INativeObject;

            if (nativeObject == null)
            {
                throw new ArgumentException("Invalid argument");
            }

            lock (Spotify.Mutex)
            {
                return Spotify.sp_session_player_prefetch(Handle, nativeObject.GetHandle());
            }
        }

        public Error PlayerPause()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_session_player_play(Handle, false);
            }
        }

        public Error PlayerPlay()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_session_player_play(Handle, true);
            }
        }

        public Error PlayerSeek(TimeSpan offset)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_session_player_seek(Handle, (int)offset.TotalMilliseconds);
            }
        }

        public Error PlayerUnload()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_session_player_unload(Handle);
            }
        }

        public ISearch Search(
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
            object userData = null)
        {
            AssertHandle();

            var search = new NativeSearch(
                this,
                query,
                trackOffset,
                trackCount,
                albumOffset,
                albumCount,
                artistOffset,
                artistCount,
                playlistOffset,
                playlistCount,
                searchType,
                userData);

            search.Initialize();
            return search;
        }

        public IAlbumBrowse Browse(IAlbum album, object userData = null)
        {
            if (!(album is INativeObject))
            {
                throw new ArgumentException("Invalid type");
            }

            AssertHandle();

            var browse = new NativeAlbumBrowse(this, album, userData);
            browse.Initialize();
            return browse;
        }

        public IArtistBrowse Browse(IArtist artist, object userData = null)
        {
            return Browse(artist, ArtistBrowseType.Full, userData);
        }

        public IArtistBrowse Browse(IArtist artist, ArtistBrowseType browseType, object userData = null)
        {
            if (!(artist is INativeObject))
            {
                throw new ArgumentException("Invalid type");
            }

            AssertHandle();

            var browse = new NativeArtistBrowse(this, artist.GetHandle(), browseType);
            browse.Initialize();
            return browse;
        }

        public IToplistBrowse Browse(ToplistType type, int encodedCountryCode, object userData = null)
        {
            AssertHandle();

            var browse = new NativeToplistBrowse(this, type, encodedCountryCode, userData);
            browse.Initialize();
            return browse;
        }

        public IToplistBrowse Browse(ToplistType type, object userData = null)
        {
            return Browse(type, (int)ToplistSpecialRegion.Everywhere, userData);
        }

        public IToplistBrowse Browse(ToplistType type, string userName, object userData = null)
        {
            AssertHandle();

            var browse = new NativeToplistBrowse(this, type, userName, userData);
            browse.Initialize();
            return browse;
        }

        public IToplistBrowse BrowseCurrentUser(ToplistType type, object userData = null)
        {
            return Browse(type, null, userData);
        }

        public IImage GetImage(string id)
        {
            AssertHandle();

            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            if (id.Length != 40)
            {
                throw new ArgumentException("invalid id", "id");
            }

            var image = new NativeImage(this, id);
            image.Initialize();
            return image;
        }

        public ISession SetPreferredBitrate(Bitrate bitrate)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Error error = Spotify.sp_session_preferred_bitrate(Handle, bitrate);

                if (error != Error.OK)
                {
                    throw new TorshifyException(error.GetMessage(), error);
                }
            }

            return this;
        }

        public ISession SetPreferredOfflineBitrate(Bitrate bitrate, bool resync)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Error error = Spotify.sp_session_preferred_offline_bitrate(Handle, bitrate, resync);
                
                if (error != Error.OK)
                {
                    throw new TorshifyException(error.GetMessage(), error);
                }
            }

            return this;
        }

        public ISession SetConnectionType(ConnectionType connectionType)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Error error = Spotify.sp_session_set_connection_type(Handle, connectionType);

                if (error != Error.OK)
                {
                    throw new TorshifyException(error.GetMessage(), error);
                }
            }

            return this;
        }

        public ISession SetConnectionRules(ConnectionRule connectionRule)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Error error = Spotify.sp_session_set_connection_rules(Handle, connectionRule);

                if (error != Error.OK)
                {
                    throw new TorshifyException(error.GetMessage(), error);
                }
            }

            return this;
        }

        public ISession SetCacheSize(uint megabytes)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Error error = Spotify.sp_session_set_cache_size(Handle, megabytes);

                if (error != Error.OK)
                {
                    throw new TorshifyException(error.GetMessage(), error);
                }
            }

            return this;
        }

        public int GetNumberOfOfflineTracksRemainingToSync()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_offline_tracks_to_sync(Handle);
            }
        }

        public int GetNumberOfOfflinePlaylists()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_offline_num_playlists(Handle);
            }
        }

        public OfflineSyncStatus GetOfflineSyncStatus()
        {
            AssertHandle();

            var syncStatus = new OfflineSyncStatus();

            lock (Spotify.Mutex)
            {
                Spotify.SpotifyOfflineSyncStatus offlineSyncStatus = new Spotify.SpotifyOfflineSyncStatus();
                Spotify.sp_offline_sync_get_status(Handle, ref offlineSyncStatus);

                syncStatus.CopiedBytes = offlineSyncStatus.CopiedBytes;
                syncStatus.CopiedTracks = offlineSyncStatus.CopiedTracks;
                syncStatus.DoneBytes = offlineSyncStatus.DoneBytes;
                syncStatus.DoneTracks = offlineSyncStatus.DoneTracks;
                syncStatus.ErrorTracks = offlineSyncStatus.ErrorTracks;
                syncStatus.IsSyncing = offlineSyncStatus.Syncing;
                syncStatus.QueuedBytes = offlineSyncStatus.QueuedBytes;
                syncStatus.QueuedTracks = offlineSyncStatus.QueuedTracks;
            }

            return syncStatus;
        }

        public IPlaylist GetStarredForUser(string canonicalUserName)
        {
            if (ConnectionState != ConnectionState.LoggedIn)
            {
                return null;
            }

            AssertHandle();

            lock (Spotify.Mutex)
            {
                IntPtr starredPtr = Spotify.sp_session_starred_for_user_create(Handle, canonicalUserName);
                return PlaylistManager.Get(this, starredPtr);
            }
        }

        public IPlaylistContainer GetPlaylistContainerForUser(string canonicalUsername)
        {
            if (ConnectionState != ConnectionState.LoggedIn)
            {
                return null;
            }

            AssertHandle();

            lock (Spotify.Mutex)
            {
                IntPtr containerPtr = Spotify.sp_session_publishedcontainer_for_user_create(Handle, canonicalUsername);
                return PlaylistContainerManager.Get(this, containerPtr);
            }
        }

        public Error FlushCaches()
        {
            lock (Spotify.Mutex)
            {
                return Spotify.sp_session_flush_caches(Handle);
            }
        }

        public override void Initialize()
        {
            _callbacks = new NativeSessionCallbacks(this);

            if (!string.IsNullOrEmpty(_options.SettingsLocation))
            {
                Directory.CreateDirectory(_options.SettingsLocation);
            }

            var sessionConfig = new Spotify.SpotifySessionConfig
            {
                ApiVersion = Spotify.SPOTIFY_API_VERSION,
                CacheLocation = _options.CacheLocation,
                SettingsLocation = _options.SettingsLocation,
                UserAgent = _options.UserAgent,
                CompressPlaylists = _options.CompressPlaylists,
                DontSaveMetadataForPlaylists = _options.DontSavePlaylistMetadata,
                InitiallyUnloadPlaylists = _options.InitiallyUnloadPlaylists,
                ApplicationKey = Marshal.AllocHGlobal(_applicationKey.Length),
                ApplicationKeySize = _applicationKey.Length,
                Callbacks = _callbacks.CallbackHandle,
                DeviceID = _options.DeviceID,
                TraceFile = _options.TraceFileLocation,
                Proxy = _options.Proxy,
                ProxyUsername = _options.ProxyUsername,
                ProxyPassword = _options.ProxyPassword
            };

            try
            {
                Marshal.Copy(_applicationKey, 0, sessionConfig.ApplicationKey, _applicationKey.Length);

                lock (Spotify.Mutex)
                {
                    IntPtr sessionPtr;
                    Error res = Spotify.sp_session_create(ref sessionConfig, out sessionPtr);

                    if (res != Error.OK)
                    {
                        throw new TorshifyException(res.GetMessage(), res);
                    }

                    Handle = sessionPtr;
                }
            }
            finally
            {
                if (sessionConfig.ApplicationKey != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(sessionConfig.ApplicationKey);
                    sessionConfig.ApplicationKey = IntPtr.Zero;
                }
            }

            _mainThreadNotification = new AutoResetEvent(false);
            _mainThread = new Thread(MainThreadLoop);
            _mainThread.Name = "MainLoop";
            _mainThread.IsBackground = true;
            _mainThread.Start();

            _eventQueue = new Queue<DelegateInvoker>();

            _eventThread = new Thread(EventThreadLoop);
            _eventThread.Name = "EventLoop";
            _eventThread.IsBackground = true;
            _eventThread.Start();

            AppDomain.CurrentDomain.ProcessExit += OnHostProcessExit;
        }

        #endregion Public Methods

        #region Internal Methods

        internal void Queue(DelegateInvoker delegateInvoker)
        {
            lock (_eventQueueLock)
            {
                _eventQueue.Enqueue(delegateInvoker);
                Monitor.Pulse(_eventQueueLock);
            }
        }

        internal void OnNotifyMainThread()
        {
            _mainThreadNotification.Set();
        }

        internal void OnException(SessionEventArgs e)
        {
            Exception.RaiseEvent(this, e);
        }

        internal void OnLoginComplete(SessionEventArgs e)
        {
            if (e.Status == Error.OK)
            {
                _playlistContainer = PlaylistContainerManager.Get(this, Spotify.sp_session_playlistcontainer(Handle));
            }

            LoginComplete.RaiseEvent(this, e);
        }

        internal void OnLogoutComplete(SessionEventArgs e)
        {
            LogoutComplete.RaiseEvent(this, e);
        }

        internal void OnLogMessage(SessionEventArgs e)
        {
            LogMessage.RaiseEvent(this, e);
        }

        internal void OnConnectionError(SessionEventArgs e)
        {
            ConnectionError.RaiseEvent(this, e);
        }

        internal void OnEndOfTrack(SessionEventArgs e)
        {
            EndOfTrack.RaiseEvent(this, e);
        }

        internal void OnMessageToUser(SessionEventArgs e)
        {
            MessageToUser.RaiseEvent(this, e);
        }

        internal void OnMetadataUpdated(SessionEventArgs e)
        {
            MetadataUpdated.RaiseEvent(this, e);
        }

        internal void OnMusicDeliver(MusicDeliveryEventArgs e)
        {
            MusicDeliver.RaiseEvent(this, e);
        }

        internal void OnPlayTokenLost(SessionEventArgs e)
        {
            PlayTokenLost.RaiseEvent(this, e);
        }

        internal void OnStartPlayback(SessionEventArgs e)
        {
            StartPlayback.RaiseEvent(this, e);
        }

        internal void OnStopPlayback(SessionEventArgs e)
        {
            StopPlayback.RaiseEvent(this, e);
        }

        internal void OnStreamingError(SessionEventArgs e)
        {
            StreamingError.RaiseEvent(this, e);
        }

        internal void OnUserinfoUpdated(SessionEventArgs e)
        {
            UserinfoUpdated.RaiseEvent(this, e);
        }

        internal void OnOfflineStatusUpdated(SessionEventArgs e)
        {
            OfflineStatusUpdated.RaiseEvent(this, e);
        }

        internal void OnOfflineError(SessionEventArgs e)
        {
            OfflineError.RaiseEvent(this, e);
        }

        internal void OnCredentialsBlobUpdated(CredentialsBlobEventArgs e)
        {
            CredentialsBlobUpdated.RaiseEvent(this, e);
        }

        internal void OnConnectionStateUpdated(SessionEventArgs e)
        {
            ConnectionStateUpdated.RaiseEvent(this, e);
        }

        internal void OnScrobbleError(SessionEventArgs e)
        {
            ScrobbleError.RaiseEvent(this, e);
        }

        internal void OnPrivateSessionModeChanged(PrivateSessionModeChangedEventArgs e)
        {
            PrivateSessionModeChanged.RaiseEvent(this, e);
        }

        #endregion Internal Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed
            }

            // Dispose unmanaged
            if (!IsInvalid)
            {
                try
                {
                    _mainThreadNotification.Set();

                    lock (_eventQueueLock)
                    {
                        Monitor.Pulse(_eventQueueLock);
                    }

                    if (_callbacks != null)
                    {
                        _callbacks.Dispose();
                        _callbacks = null;
                    }

                    PlaylistTrackManager.RemoveAll(this);
                    TrackManager.DisposeAll(this);

                    LinkManager.RemoveAll(this);
                    UserManager.RemoveAll(this);

                    PlaylistContainerManager.RemoveAll(this);
                    PlaylistManager.RemoveAll(this);
                    ContainerPlaylistManager.RemoveAll(this);
                    ArtistManager.RemoveAll();
                    AlbumManager.RemoveAll();

                    SessionManager.Remove(Handle);

                    lock (Spotify.Mutex)
                    {
                        Error error = Error.OK;

                        if (ConnectionState == ConnectionState.LoggedIn)
                        {
                            error = Spotify.sp_session_logout(Handle);
                            Debug.WriteLineIf(error != Error.OK, error.GetMessage());
                        }

                        Ensure(() => error = Spotify.sp_session_release(Handle));
                        Debug.WriteLineIf(error != Error.OK, error.GetMessage());
                    }
                }
                catch
                {
                    // Ignore
                }
                finally
                {
                    Debug.WriteLine("Session disposed");
                }
            }

            base.Dispose(disposing);
        }

        #endregion Protected Methods

        #region Private Methods

        private void OnHostProcessExit(object sender, EventArgs e)
        {
            Dispose();
        }

        private void MainThreadLoop()
        {
            int waitTime = 0;

            while (!IsInvalid && waitTime >= 0)
            {
                _mainThreadNotification.WaitOne(waitTime, false);

                do
                {
                    lock (Spotify.Mutex)
                    {
                        try
                        {
                            if (IsInvalid)
                            {
                                break;
                            }

                            Error error = Spotify.sp_session_process_events(Handle, out waitTime);

                            if (error != Error.OK)
                            {
                                Debug.WriteLine(Spotify.sp_error_message(error));
                            }
                        }
                        catch
                        {
                            waitTime = 1000;
                        }
                    }
                }
                while (waitTime == 0);
            }

            Debug.WriteLine("Main loop exiting.");
        }

        private void EventThreadLoop()
        {
            while (!IsInvalid)
            {
                DelegateInvoker invoker = null;

                lock (_eventQueueLock)
                {
                    if (_eventQueue.Count == 0)
                    {
                        Monitor.Wait(_eventQueueLock);
                    }

                    if (_eventQueue.Count > 0)
                    {
                        invoker = _eventQueue.Dequeue();
                    }
                }

                if (invoker != null)
                {
                    try
                    {
                        invoker.Execute();
                    }
                    catch (Exception ex)
                    {
                        OnException(new SessionEventArgs(ex.ToString()));
                    }
                }
            }

            Debug.WriteLine("Event loop exiting");
        }

        #endregion Private Methods
    }
}