using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativeSession : NativeObject, ISession
    {
        #region Fields

        private readonly byte[] _applicationKey;
        private readonly string _cacheLocation;
        private readonly string _settingsLocation;
        private readonly string _userAgent;

        private IPlaylistContainer _playlistContainer;
        private IPlaylist _starredPlaylist;
        private Thread _mainThread;
        private Thread _eventThread;
        private AutoResetEvent _mainThreadNotification;
        private AutoResetEvent _eventThreadNotification;
        private Queue<DelegateInvoker> _eventQueue;
        private NativeSessionCallbacks _callbacks;
        private IArray<IUser> _friends;

        #endregion Fields

        #region Constructors

        public NativeSession(byte[] applicationKey, string cacheLocation, string settingsLocation, string userAgent)
            : base(null, IntPtr.Zero)
        {
            _applicationKey = applicationKey;
            _cacheLocation = cacheLocation;
            _settingsLocation = settingsLocation;
            _userAgent = userAgent;
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

        public IArray<IUser> Friends
        {
            get
            {
                if (ConnectionState != ConnectionState.LoggedIn)
                {
                    return null;
                }

                AssertHandle();

                return _friends;
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

        public override ISession Session
        {
            get
            {
                return this;
            }
        }

        #endregion Properties

        #region Public Methods

        public void Login(string userName, string password)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Error result = Spotify.sp_session_login(Handle, userName, password);

                if (result != Error.OK)
                {
                    throw new Exception(result.GetMessage());
                }
            }
        }

        public void Logout()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Error result = Spotify.sp_session_logout(Handle);

                if (result != Error.OK)
                {
                    throw new Exception(result.GetMessage());
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

        public void PlayerPause()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Spotify.sp_session_player_play(Handle, false);
            }
        }

        public void PlayerPlay()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Spotify.sp_session_player_play(Handle, true);
            }
        }

        public void PlayerSeek(TimeSpan offset)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Spotify.sp_session_player_seek(Handle, (int)offset.TotalMilliseconds);
            }
        }

        public void PlayerUnload()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Spotify.sp_session_player_unload(Handle);
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
                userData);

            search.Initialize();
            return search;
        }

        public ISearch Search(int fromYear, int toYear, RadioGenre genre, object userData = null)
        {
            AssertHandle();

            var search = new NativeRadioSearch(
                this,
                fromYear,
                toYear,
                genre,
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
            if (!(artist is INativeObject))
            {
                throw new ArgumentException("Invalid type");
            }

            AssertHandle();

            var browse = new NativeArtistBrowse(this, artist);
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
                Spotify.sp_session_preferred_bitrate(Handle, bitrate);
            }

            return this;
        }

        public ISession SetPreferredOfflineBitrate(Bitrate bitrate, bool resync)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Spotify.sp_session_preferred_offline_bitrate(Handle, bitrate, resync);
            }

            return this;
        }

        public ISession SetConnectionType(ConnectionType connectionType)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Spotify.sp_session_set_connection_type(Handle, connectionType);
            }

            return this;
        }

        public ISession SetConnectionRules(ConnectionRule connectionRule)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Spotify.sp_session_set_connection_rules(Handle, connectionRule);
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

        public override void Initialize()
        {
            _callbacks = new NativeSessionCallbacks(this);

            var sessionConfig = new Spotify.SpotifySessionConfig
            {
                ApiVersion = Spotify.SPOTIFY_API_VERSION,
                CacheLocation = _cacheLocation,
                SettingsLocation = _settingsLocation,
                UserAgent = _userAgent,
                CompressPlaylists = false,
                DontSaveMetadataForPlaylists = false,
                InitiallyUnloadPlaylists = false,
                ApplicationKey = Marshal.AllocHGlobal(_applicationKey.Length),
                ApplicationKeySize = _applicationKey.Length,
                Callbacks = _callbacks.CallbackHandle
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
                        throw new Exception(res.GetMessage());
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

            _friends = new DelegateArray<IUser>(GetNumberOfFriends, GetFriendAtIndex);

            _mainThreadNotification = new AutoResetEvent(false);
            _mainThread = new Thread(MainThreadLoop);
            _mainThread.Name = "MainLoop";
            _mainThread.IsBackground = true;
            _mainThread.Start();

            _eventQueue = new Queue<DelegateInvoker>(2000);

            _eventThreadNotification = new AutoResetEvent(false);
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
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    delegateInvoker.Execute();
                }
                catch (Exception ex)
                {
                    OnException(new SessionEventArgs(ex.ToString()));
                }
            });
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
                    _eventThreadNotification.Set();

                    if (_callbacks != null)
                    {
                        _callbacks.Dispose();
                        _callbacks = null;
                    }

                    PlaylistTrackManager.RemoveAll(this);
                    TrackManager.RemoveAll(this);

                    LinkManager.RemoveAll(this);
                    UserManager.RemoveAll(this);

                    PlaylistContainerManager.RemoveAll(this);
                    PlaylistManager.RemoveAll(this);
                    ContainerPlaylistManager.RemoveAll(this);

                    SessionManager.Remove(Handle);

                    lock (Spotify.Mutex)
                    {
                        if (ConnectionState == ConnectionState.LoggedIn)
                        {
                            try
                            {
                                Spotify.sp_session_logout(Handle);
                            }
                            catch
                            {
                            }
                        }

                        Spotify.sp_session_release(Handle);
                        Handle = IntPtr.Zero;
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

        private IUser GetFriendAtIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return UserManager.Get(this, Spotify.sp_session_friend(Handle, index));
            }
        }

        private int GetNumberOfFriends()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_session_num_friends(Handle);
            }
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
                                throw new Exception();
                            }

                            Spotify.sp_session_process_events(Handle, out waitTime);
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
            var localList = new List<DelegateInvoker>();

            while (!IsInvalid)
            {
                _eventThreadNotification.WaitOne();
                lock (_eventQueue)
                {
                    while (_eventQueue.Count > 0)
                    {
                        localList.Add(_eventQueue.Dequeue());
                    }
                }

                foreach (var workerItem in localList)
                {
                    try
                    {
                        workerItem.Execute();
                    }
                    catch (Exception ex)
                    {
                        OnException(new SessionEventArgs(ex.ToString()));
                    }
                }

                localList.Clear();
            }

            Debug.WriteLine("Event loop exiting");
        }

        #endregion Private Methods
    }
}