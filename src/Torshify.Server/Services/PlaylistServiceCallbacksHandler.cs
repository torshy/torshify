using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.ServiceLocation;

using Torshify.Server.Contracts;
using Torshify.Server.Extensions;

namespace Torshify.Server.Services
{
    public class PlaylistServiceCallbacksHandler
    {
        #region Fields

        private static readonly PlaylistServiceCallbacksHandler _instance = new PlaylistServiceCallbacksHandler();

        private List<IPlaylistCallbacks> _callbacks;
        private object _lockObject = new object();
        private bool _isInitialized;

        #endregion Fields

        #region Constructors

        public PlaylistServiceCallbacksHandler()
        {
            _callbacks = new List<IPlaylistCallbacks>();
        }

        #endregion Constructors

        #region Properties

        public static PlaylistServiceCallbacksHandler Instance
        {
            get { return _instance; }
        }

        #endregion Properties

        #region Public Methods

        public void Register(IPlaylistCallbacks callback)
        {
            lock (_lockObject)
            {
                if (!_isInitialized)
                {
                    var session = ServiceLocator.Current.Resolve<ISession>();

                    session.PlaylistContainer.PlaylistAdded += PlaylistAdded;
                    session.PlaylistContainer.PlaylistRemoved += PlaylistRemoved;
                    session.PlaylistContainer.PlaylistMoved += PlaylistMoved;

                    foreach (var playlist in session.PlaylistContainer.Playlists)
                    {
                        HookPlaylist(playlist);
                    }

                    _isInitialized = true;
                }

                _callbacks.Add(callback);
            }
        }

        public void Unregister(IPlaylistCallbacks callback)
        {
            lock (_lockObject)
            {
                _callbacks.Remove(callback);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void HookPlaylist(IPlaylist playlist)
        {
            playlist.TracksAdded += TracksAdded;
            playlist.TracksMoved += TracksMoved;
            playlist.TracksRemoved += TracksRemoved;
            playlist.Renamed += Renamed;
        }

        private void UnhookPlaylist(IPlaylist playlist)
        {
            playlist.TracksAdded -= TracksAdded;
            playlist.TracksMoved -= TracksMoved;
            playlist.TracksRemoved -= TracksRemoved;
            playlist.Renamed -= Renamed;
        }

        private void PlaylistMoved(object sender, PlaylistMovedEventArgs e)
        {
            lock(_lockObject)
            {
                foreach (var callback in _callbacks)
                {
                    callback.OnPlaylistMoved(e.OldIndex, e.NewIndex);
                }
            }
        }

        private void PlaylistRemoved(object sender, PlaylistEventArgs e)
        {
            UnhookPlaylist(e.Playlist);

            lock (_lockObject)
            {
                foreach (var callback in _callbacks)
                {
                    callback.OnPlaylistRemoved(e.Position, e.Playlist.GetHashCode());
                }
            }
        }

        private void PlaylistAdded(object sender, PlaylistEventArgs e)
        {
            HookPlaylist(e.Playlist);

            lock (_lockObject)
            {
                foreach (var callback in _callbacks)
                {
                    callback.OnPlaylistAdded(e.Position, e.Playlist.GetHashCode());
                }
            }
        }

        private void TracksRemoved(object sender, TracksRemovedEventArgs e)
        {
            IPlaylist playlist = (IPlaylist) sender;

            lock (_lockObject)
            {
                foreach (var callback in _callbacks)
                {
                    callback.OnTracksRemoved(playlist.GetHashCode(), e.TrackIndices);
                }
            }
        }

        private void TracksMoved(object sender, TracksMovedEventArgs e)
        {
            IPlaylist playlist = (IPlaylist)sender;

            lock (_lockObject)
            {
                foreach (var callback in _callbacks)
                {
                    callback.OnTracksMoved(playlist.GetHashCode(), e.TrackIndices, e.NewPosition);
                }
            }
        }

        private void TracksAdded(object sender, TracksAddedEventArgs e)
        {
            IPlaylist playlist = (IPlaylist)sender;

            lock (_lockObject)
            {
                var tracks = e.Tracks.Select(t => t.ToDto()).ToArray();

                foreach (var callback in _callbacks)
                {
                    callback.OnTracksAdded(playlist.GetHashCode(), e.TrackIndices, tracks);
                }
            }
        }

        private void Renamed(object sender, EventArgs e)
        {
            IPlaylist playlist = (IPlaylist)sender;

            lock (_lockObject)
            {
                foreach (var callback in _callbacks)
                {
                    callback.OnRenamed(playlist.GetHashCode());
                }
            }
        }

        #endregion Private Methods
    }
}