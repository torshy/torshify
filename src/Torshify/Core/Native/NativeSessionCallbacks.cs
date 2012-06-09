using System;
using System.Runtime.InteropServices;

namespace Torshify.Core.Native
{
    internal class NativeSessionCallbacks : IDisposable
    {
        #region Fields

        internal static readonly int CallbacksSize = Marshal.SizeOf(typeof(Spotify.SpotifySessionCallbacks));

        private readonly Spotify.SpotifySessionCallbacks _callbacks;
        private readonly NativeSession _session;
        private readonly ConnectionErrorDelegate _connectionError;
        private readonly EndOfTrackDelegate _endOfTrack;
        private readonly GetAudioBufferStatsDelegate _getAudioBufferStats;
        private readonly LoggedInDelegate _loggedIn;
        private readonly LoggedOutDelegate _loggedOut;
        private readonly LogMessageDelegate _logMessage;
        private readonly MessageToUserDelegate _messageToUser;
        private readonly MetadataUpdatedDelegate _metadataUpdated;
        private readonly MusicDeliveryDelegate _musicDelivery;
        private readonly NotifyMainThreadDelegate _notifyMainThread;
        private readonly PlayTokenLostDelegate _playTokenLost;
        private readonly StartPlaybackDelegate _startPlayback;
        private readonly StopPlaybackDelegate _stopPlayback;
        private readonly StreamingErrorDelegate _streamingError;
        private readonly UserinfoUpdatedDelegate _userinfoUpdated;
        private readonly OfflineStatusUpdated _offlineStatusUpdated;
        private readonly OfflineError _offlineError;
        private readonly CredentialsBlobUpdated _credentialsBlobUpdated;
        private readonly ConnectionStateUpdatedDelegate _connectionStateUpdated;
        private readonly ScrobbleErrorDelegate _scrobbleError;
        private readonly PrivateSessionModeChangedDelegate _privateSessionModeChanged;


        private IntPtr _callbacksHandle;

        #endregion Fields

        #region Constructors

        public NativeSessionCallbacks(NativeSession session)
        {
            _session = session;

            _connectionError = ConnectionErrorCallback;
            _endOfTrack = EndOfTrackCallback;
            _getAudioBufferStats = GetAudioBufferStatsCallback;
            _loggedIn = LoggedInCallback;
            _loggedOut = LoggedOutCallback;
            _logMessage = LogMessageCallback;
            _messageToUser = MessageToUserCallback;
            _metadataUpdated = MetadataUpdatedCallback;
            _musicDelivery = MusicDeliveryCallback;
            _notifyMainThread = NotifyMainThreadCallback;
            _playTokenLost = PlayTokenLostCallback;
            _startPlayback = StartPlaybackCallback;
            _stopPlayback = StopPlaybackCallback;
            _streamingError = StreamingErrorCallback;
            _userinfoUpdated = UserinfoUpdatedCallback;
            _offlineStatusUpdated = OfflineStatusUpdatedCallback;
            _offlineError = OfflineErrorCallback;
            _credentialsBlobUpdated = CredentialsBlobUpdatedCallback;
            _connectionStateUpdated = ConnectionStateUpdatedCallback;
            _scrobbleError = ScrobbleErrorCallback;
            _privateSessionModeChanged = PrivateSessionModeChangedCallback;
            
            _callbacks = new Spotify.SpotifySessionCallbacks();
            _callbacks.LoggedIn = Marshal.GetFunctionPointerForDelegate(_loggedIn);
            _callbacks.LoggedOut = Marshal.GetFunctionPointerForDelegate(_loggedOut);
            _callbacks.MetadataUpdated = Marshal.GetFunctionPointerForDelegate(_metadataUpdated);
            _callbacks.ConnectionError = Marshal.GetFunctionPointerForDelegate(_connectionError);
            _callbacks.MessageToUser = Marshal.GetFunctionPointerForDelegate(_messageToUser);
            _callbacks.NotifyMainThread = Marshal.GetFunctionPointerForDelegate(_notifyMainThread);
            _callbacks.MusicDelivery = Marshal.GetFunctionPointerForDelegate(_musicDelivery);
            _callbacks.PlayTokenLost = Marshal.GetFunctionPointerForDelegate(_playTokenLost);
            _callbacks.LogMessage = Marshal.GetFunctionPointerForDelegate(_logMessage);
            _callbacks.EndOfTrack = Marshal.GetFunctionPointerForDelegate(_endOfTrack);
            _callbacks.StreamingError = Marshal.GetFunctionPointerForDelegate(_streamingError);
            _callbacks.UserinfoUpdated = Marshal.GetFunctionPointerForDelegate(_userinfoUpdated);
            _callbacks.StartPlayback = Marshal.GetFunctionPointerForDelegate(_startPlayback);
            _callbacks.StopPlayback = Marshal.GetFunctionPointerForDelegate(_stopPlayback);
            _callbacks.GetAudioBufferStats = Marshal.GetFunctionPointerForDelegate(_getAudioBufferStats);
            _callbacks.OfflineStatusUpdated = Marshal.GetFunctionPointerForDelegate(_offlineStatusUpdated);
            _callbacks.OfflineError = Marshal.GetFunctionPointerForDelegate(_offlineError);
            _callbacks.CredentialsBlobUpdated = Marshal.GetFunctionPointerForDelegate(_credentialsBlobUpdated);
            _callbacks.ConnectionStateUpdated = Marshal.GetFunctionPointerForDelegate(_connectionStateUpdated);
            _callbacks.ScrobbleError = Marshal.GetFunctionPointerForDelegate(_scrobbleError);
            _callbacks.PrivateSessionModeChanged = Marshal.GetFunctionPointerForDelegate(_privateSessionModeChanged);

            _callbacksHandle = Marshal.AllocHGlobal(CallbacksSize);
            Marshal.StructureToPtr(_callbacks, _callbacksHandle, true);
        }

        #endregion Constructors

        #region Delegates

        private delegate void ConnectionErrorDelegate(IntPtr sessionPtr, Error error);

        private delegate void EndOfTrackDelegate(IntPtr sessionPtr);

        private delegate void GetAudioBufferStatsDelegate(IntPtr sessionPtr, IntPtr statsPtr);

        private delegate void LoggedInDelegate(IntPtr sessionPtr, Error error);

        private delegate void LoggedOutDelegate(IntPtr sessionPtr);

        private delegate void LogMessageDelegate(IntPtr sessionPtr, string data);

        private delegate void MessageToUserDelegate(IntPtr sessionPtr, string message);

        private delegate void MetadataUpdatedDelegate(IntPtr sessionPtr);

        private delegate int MusicDeliveryDelegate(IntPtr sessionPtr, IntPtr formatPtr, IntPtr framesPtr, int numFrames);

        private delegate void NotifyMainThreadDelegate(IntPtr sessionPtr);

        private delegate void PlayTokenLostDelegate(IntPtr sessionPtr);

        private delegate void StartPlaybackDelegate(IntPtr sessionPtr);

        private delegate void StopPlaybackDelegate(IntPtr sessionPtr);

        private delegate void StreamingErrorDelegate(IntPtr sessionPtr, Error error);

        private delegate void UserinfoUpdatedDelegate(IntPtr sessionPtr);

        private delegate void OfflineStatusUpdated(IntPtr sessionPtr);

        private delegate void OfflineError(IntPtr sessionPtr, Error error);

        private delegate void CredentialsBlobUpdated(IntPtr sessionPtr, string blob);

        private delegate void ConnectionStateUpdatedDelegate(IntPtr sessionPtr);

        private delegate void ScrobbleErrorDelegate(IntPtr sessionPtr, Error error);

        private delegate void PrivateSessionModeChangedDelegate(IntPtr sessionPtr, bool isPrivate);

        #endregion Delegates

        #region Properties

        public IntPtr CallbackHandle
        {
            get { return _callbacksHandle; }
        }

        #endregion Properties

        #region Public Methods

        public void Dispose()
        {
            try
            {
                Marshal.FreeHGlobal(_callbacksHandle);
            }
            catch
            {
                // Ignore
            }
            finally
            {
                _callbacksHandle = IntPtr.Zero;
                GC.KeepAlive(_callbacks);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void ConnectionErrorCallback(IntPtr sessionPtr, Error error)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnConnectionError(new SessionEventArgs(error))));
        }

        private void EndOfTrackCallback(IntPtr sessionPtr)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnEndOfTrack(new SessionEventArgs("End of track"))));
        }

        private void GetAudioBufferStatsCallback(IntPtr sessionPtr, IntPtr statsPtr)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }
        }

        private void LoggedInCallback(IntPtr sessionPtr, Error error)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnLoginComplete(new SessionEventArgs(error))));
        }

        private void LoggedOutCallback(IntPtr sessionPtr)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnLogoutComplete(new SessionEventArgs("Logged out"))));
        }

        private void LogMessageCallback(IntPtr sessionPtr, string data)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnLogMessage(new SessionEventArgs(data))));
        }

        private void MessageToUserCallback(IntPtr sessionPtr, string message)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnMessageToUser(new SessionEventArgs(message))));
        }

        private void MetadataUpdatedCallback(IntPtr sessionPtr)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnMetadataUpdated(new SessionEventArgs("Metadata updated"))));
        }

        private int MusicDeliveryCallback(IntPtr sessionPtr, IntPtr formatPtr, IntPtr framesPtr, int numFrames)
        {
            if (sessionPtr != _session.Handle)
            {
                return 0;
            }

            byte[] samplesBytes = new byte[0];
            Spotify.SpotifyAudioFormat format = (Spotify.SpotifyAudioFormat)Marshal.PtrToStructure(formatPtr, typeof(Spotify.SpotifyAudioFormat));

            if (numFrames > 0)
            {
                samplesBytes = new byte[numFrames * format.Channels * 2];
                Marshal.Copy(framesPtr, samplesBytes, 0, samplesBytes.Length);
            }

            var args = new MusicDeliveryEventArgs(format.Channels, format.SampleRate, samplesBytes, numFrames);
            _session.OnMusicDeliver(args);

            return args.ConsumedFrames;
        }

        private void NotifyMainThreadCallback(IntPtr sessionPtr)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.OnNotifyMainThread();
        }

        private void PlayTokenLostCallback(IntPtr sessionPtr)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnPlayTokenLost(new SessionEventArgs("Player token lost"))));
        }

        private void StartPlaybackCallback(IntPtr sessionPtr)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnStartPlayback(new SessionEventArgs(Error.OK))));
        }

        private void StopPlaybackCallback(IntPtr sessionPtr)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnStopPlayback(new SessionEventArgs(Error.OK))));
        }

        private void StreamingErrorCallback(IntPtr sessionPtr, Error error)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnStreamingError(new SessionEventArgs(error))));
        }

        private void UserinfoUpdatedCallback(IntPtr sessionPtr)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnUserinfoUpdated(new SessionEventArgs(Error.OK))));
        }

        private void OfflineStatusUpdatedCallback(IntPtr sessionPtr)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

           _session.Queue(new DelegateInvoker(() => _session.OnOfflineStatusUpdated(new SessionEventArgs(Error.OK))));
        }

        private void OfflineErrorCallback(IntPtr sessionPtr, Error error)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnOfflineError(new SessionEventArgs(error))));
        }

        private void CredentialsBlobUpdatedCallback(IntPtr sessionPtr, string blob)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnCredentialsBlobUpdated(new CredentialsBlobEventArgs(blob))));
        }

        private void ConnectionStateUpdatedCallback(IntPtr sessionPtr)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnConnectionStateUpdated(new SessionEventArgs(Error.OK))));
        }

        private void ScrobbleErrorCallback(IntPtr sessionPtr, Error error)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnScrobbleError(new SessionEventArgs(error))));
        }

        private void PrivateSessionModeChangedCallback(IntPtr sessionPtr, bool isPrivate)
        {
            if (sessionPtr != _session.Handle)
            {
                return;
            }

            _session.Queue(new DelegateInvoker(() => _session.OnPrivateSessionModeChanged(new PrivateSessionModeChangedEventArgs(isPrivate))));
        }

        #endregion Private Methods
    }
}