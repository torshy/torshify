using System;
using System.Collections.Generic;
using System.ServiceModel;

using Microsoft.Practices.ServiceLocation;

using Torshify.Server.Contracts;
using Torshify.Server.Extensions;
using Torshify.Server.Interfaces;

namespace Torshify.Server.Services
{
    public class PlayerServiceCallbacksHandler
    {
        #region Fields

        private static readonly PlayerServiceCallbacksHandler _instance = new PlayerServiceCallbacksHandler();

        private readonly List<IPlayerCallbacks> _callbacks;
        private readonly IPlayer _player;
        private readonly ISession _session;

        private object _lockObject = new object();

        #endregion Fields

        #region Constructors

        public PlayerServiceCallbacksHandler()
        {
            _callbacks = new List<IPlayerCallbacks>();
            _player = ServiceLocator.Current.Resolve<IPlayer>();
            _player.IsPlayingChanged += IsPlayingChanged;
            _player.Playlist.RepeatChanged += RepeatPlayChanged;
            _player.Playlist.ShuffleChanged += ShufflePlayChanged;
            _player.Playlist.CurrentChanged += CurrentTrackChanged;

            _session = ServiceLocator.Current.Resolve<ISession>();
            _session.PlayTokenLost += PlayTokenLost;
            _session.EndOfTrack += EndOfTrack;
        }

        #endregion Constructors

        #region Properties

        public static PlayerServiceCallbacksHandler Instance
        {
            get { return _instance; }
        }

        #endregion Properties

        #region Public Methods

        public void Register(IPlayerCallbacks callbacks)
        {
            lock (_lockObject)
            {
                _callbacks.Add(callbacks);
            }
        }

        public void Unregister(IPlayerCallbacks callbacks)
        {
            lock (_lockObject)
            {
                _callbacks.Remove(callbacks);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void CurrentTrackChanged(object sender, EventArgs e)
        {
        }

        private void ShufflePlayChanged(object sender, EventArgs e)
        {
        }

        private void RepeatPlayChanged(object sender, EventArgs e)
        {
        }

        private void IsPlayingChanged(object sender, EventArgs e)
        {
            Execute(c => c.OnIsPlayingChanged(_player.IsPlaying));
        }

        private void EndOfTrack(object sender, SessionEventArgs e)
        {
        }

        private void PlayTokenLost(object sender, SessionEventArgs e)
        {
            Execute(c => c.OnLostPlayToken());
        }

        private void Execute(Action<IPlayerCallbacks> action)
        {
            var faultedClients = new List<IPlayerCallbacks>();

            lock (_lockObject)
            {
                foreach (var client in _callbacks)
                {
                    try
                    {
                        action(client);
                    }
                    catch (CommunicationException)
                    {
                        ((ICommunicationObject)client).Abort();
                        faultedClients.Add(client);
                    }
                }
            }

            foreach (var faultedClient in faultedClients)
            {
                Unregister(faultedClient);
            }
        }

        #endregion Private Methods
    }
}