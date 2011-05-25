using System;
using System.Collections.Generic;

using log4net;

using Torshify.Server.Interfaces;

namespace Torshify.Server.Services
{
    public class Player : IPlayer, IStartable
    {
        #region Fields

        private static readonly ILog Logger = LogManager.GetLogger("Player");

        private readonly ISession _session;

        private readonly BassPlayer _bassPlayer;
        private Error _lastLoadStatus;
        private bool _isPlaying;
        private DateTime _trackPaused;
        private DateTime _trackStarted;

        #endregion Fields

        #region Constructors

        public Player(ISession session, IPlayerPlaylist playerPlaylist)
        {
            Playlist = playerPlaylist;
            Playlist.CurrentChanged += CurrentTrackChanged;

            _session = session;
            _session.MusicDeliver += OnMusicDeliver;
            _bassPlayer = new BassPlayer();
        }

        #endregion Constructors

        #region Events

        public event EventHandler IsPlayingChanged;

        #endregion Events

        #region Properties

        public IPlayerPlaylist Playlist
        {
            get;
            private set;
        }

        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            private set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    OnIsPlayingChanged();
                }
            }
        }

        #endregion Properties

        #region Public Methods

        public void Play()
        {
            if (_lastLoadStatus == Error.OK)
            {
                _session.PlayerPlay();
                IsPlaying = true;

                if (_trackPaused != DateTime.MinValue)
                {
                    _trackStarted = _trackStarted + DateTime.Now.Subtract(_trackPaused);
                    _trackPaused = DateTime.MinValue;
                }
                else
                {
                    _trackStarted = DateTime.Now;
                }
            }
        }

        public void Stop()
        {
            _session.PlayerUnload();
            IsPlaying = false;
        }

        public void Pause()
        {
            if (_lastLoadStatus == Error.OK)
            {
                _session.PlayerPause();
                IsPlaying = false;

                if (_trackPaused == DateTime.MinValue)
                {
                    _trackPaused = DateTime.Now;
                }
            }
        }

        public void Seek(TimeSpan timeSpan)
        {
            if (_isPlaying)
            {
                _session.PlayerSeek(timeSpan);
            }
        }

        public void Start()
        {
            Logger.Debug("Started");
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void OnIsPlayingChanged()
        {
            var handler = IsPlayingChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void CurrentTrackChanged(object sender, EventArgs e)
        {
            _lastLoadStatus = Playlist.Current.Load();

            if (Playlist.Current != null && IsPlaying)
            {
                Playlist.Current.Play();
            }
        }

        private void OnMusicDeliver(object sender, MusicDeliveryEventArgs e)
        {
            if (e.Samples.Length > 0)
            {
                e.ConsumedFrames = _bassPlayer.EnqueueSamples(e.Channels, e.Rate, e.Samples, e.Frames);
                IsPlaying = true;
            }
            else
            {
                e.ConsumedFrames = 0;
                IsPlaying = false;
            }
        }

        #endregion Private Methods
    }
}