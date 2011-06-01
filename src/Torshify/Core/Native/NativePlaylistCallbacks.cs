using System;
using System.Runtime.InteropServices;

using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativePlaylistCallbacks : IDisposable
    {
        #region Fields

        private readonly NativePlaylist _playlist;
        private readonly bool _registerCallbacks;

        private Spotify.PlaylistCallbacks _callbacks;
        private Spotify.TracksAddedCallback _tracksAdded;
        private Spotify.TracksRemovedCallback _tracksRemoved;
        private Spotify.TracksMovedCallback _tracksMoved;
        private Spotify.PlaylistRenamedCallback _playlistRenamed;
        private Spotify.PlaylistStateChangedCallback _playlistStateChanged;
        private Spotify.PlaylistUpdateInProgressCallback _playlistUpdateInProgress;
        private Spotify.PlaylistMetadataUpdatedCallback _playlistMetadataUpdated;
        private Spotify.TrackCreatedChangedCallback _trackCreatedChanged;
        private Spotify.TrackSeenChangedCallback _trackSeenChanged;
        private Spotify.DescriptionChangedCallback _descriptionChanged;
        private Spotify.ImageChangedCallback _imageChanged;
        private Spotify.SubscribersChangedCallback _subscribersChanged;

        #endregion Fields

        #region Constructors

        public NativePlaylistCallbacks(NativePlaylist playlist, bool registerCallbacks = true)
        {
            _registerCallbacks = registerCallbacks;
            _playlist = playlist;

            if (registerCallbacks)
            {
                _tracksAdded = OnTracksAddedCallback;
                _tracksRemoved = OnTracksRemovedCallback;
                _tracksMoved = OnTracksMovedCallback;
                _playlistRenamed = OnRenamedCallback;
                _playlistStateChanged = OnStateChangedCallback;
                _playlistUpdateInProgress = OnUpdateInProgressCallback;
                _playlistMetadataUpdated = OnMetadataUpdatedCallback;
                _trackCreatedChanged = OnTrackCreatedChangedCallback;
                _trackSeenChanged = OnTrackSeenChangedCallback;
                _descriptionChanged = OnDescriptionChangedCallback;
                _imageChanged = OnImageChangedCallback;
                _subscribersChanged = OnSubscribersChangedCallback;

                _callbacks = new Spotify.PlaylistCallbacks
                {
                    tracks_added = Marshal.GetFunctionPointerForDelegate(_tracksAdded),
                    tracks_removed = Marshal.GetFunctionPointerForDelegate(_tracksRemoved),
                    tracks_moved = Marshal.GetFunctionPointerForDelegate(_tracksMoved),
                    playlist_renamed = Marshal.GetFunctionPointerForDelegate(_playlistRenamed),
                    playlist_state_changed = Marshal.GetFunctionPointerForDelegate(_playlistStateChanged),
                    playlist_update_in_progress = Marshal.GetFunctionPointerForDelegate(_playlistUpdateInProgress),
                    playlist_metadata_updated = Marshal.GetFunctionPointerForDelegate(_playlistMetadataUpdated),
                    track_created_changed = Marshal.GetFunctionPointerForDelegate(_trackCreatedChanged),
                    track_seen_changed = Marshal.GetFunctionPointerForDelegate(_trackSeenChanged),
                    description_changed = Marshal.GetFunctionPointerForDelegate(_descriptionChanged),
                    image_changed = Marshal.GetFunctionPointerForDelegate(_imageChanged),
                    subscribers_changed = Marshal.GetFunctionPointerForDelegate(_subscribersChanged)
                };

                lock (Spotify.Mutex)
                {
                    Spotify.sp_playlist_add_callbacks(_playlist.Handle, ref _callbacks, IntPtr.Zero);
                }
            }
        }

        #endregion Constructors

        #region Public Methods

        public void Dispose()
        {
            if (_registerCallbacks)
            {
                try
                {
                    lock (Spotify.Mutex)
                    {
                        Spotify.sp_playlist_remove_callbacks(_playlist.Handle, ref _callbacks, IntPtr.Zero);
                    }
                }
                catch
                {
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void OnImageChangedCallback(IntPtr playlistPtr, IntPtr imgidptr, IntPtr userdataptr)
        {
            if (playlistPtr != _playlist.Handle)
            {
                return;
            }

            string imageId = Spotify.ImageIdToString(imgidptr);

            _playlist.QueueThis<NativePlaylist, ImageEventArgs>(
                    pc => pc.OnImageChanged,
                    _playlist,
                    new ImageEventArgs(imageId));
        }

        private void OnDescriptionChangedCallback(IntPtr playlistPtr, IntPtr descptr, IntPtr userdataptr)
        {
            if (playlistPtr != _playlist.Handle)
            {
                return;
            }

            string description = Spotify.GetString(descptr, string.Empty);

            _playlist.QueueThis<NativePlaylist, DescriptionEventArgs>(
                    pc => pc.OnDescriptionChanged,
                    _playlist,
                    new DescriptionEventArgs(description));
        }

        private void OnTrackSeenChangedCallback(IntPtr playlistPtr, int position, bool seen, IntPtr userdataptr)
        {
            if (playlistPtr != _playlist.Handle)
            {
                return;
            }

            ITrack track = TrackManager.Get(_playlist.Session, Spotify.sp_playlist_track(playlistPtr, position));

            _playlist.QueueThis<NativePlaylist, TrackSeenEventArgs>(
                    pc => pc.OnTrackSeenChanged,
                    _playlist,
                    new TrackSeenEventArgs(track, seen));
        }

        private void OnTrackCreatedChangedCallback(IntPtr playlistPtr, int position, IntPtr userptr, int when, IntPtr userdataptr)
        {
            if (playlistPtr != _playlist.Handle)
            {
                return;
            }

            ITrack track = TrackManager.Get(_playlist.Session, Spotify.sp_playlist_track(playlistPtr, position));
            DateTime dtWhen = new DateTime(TimeSpan.FromSeconds(when).Ticks, DateTimeKind.Utc);

            _playlist.QueueThis<NativePlaylist, TrackCreatedChangedEventArgs>(
                    pc => pc.OnTrackCreatedChanged,
                    _playlist,
                    new TrackCreatedChangedEventArgs(track, dtWhen));
        }

        private void OnMetadataUpdatedCallback(IntPtr playlistPtr, IntPtr userdataptr)
        {
            if (playlistPtr != _playlist.Handle)
            {
                return;
            }

            _playlist.QueueThis<NativePlaylist, EventArgs>(
                    pc => pc.OnMetadataUpdated,
                    _playlist,
                    EventArgs.Empty);
        }

        private void OnUpdateInProgressCallback(IntPtr playlistPtr, bool done, IntPtr userdataptr)
        {
            if (playlistPtr != _playlist.Handle)
            {
                return;
            }

            _playlist.QueueThis<NativePlaylist, PlaylistUpdateEventArgs>(
                    pc => pc.OnUpdateInProgress,
                    _playlist,
                    new PlaylistUpdateEventArgs(done));
        }

        private void OnStateChangedCallback(IntPtr playlistPtr, IntPtr userdataptr)
        {
            if (playlistPtr != _playlist.Handle)
            {
                return;
            }

            _playlist.QueueThis<NativePlaylist, EventArgs>(
                    pc => pc.OnStateChanged,
                    _playlist,
                    EventArgs.Empty);
        }

        private void OnRenamedCallback(IntPtr playlistPtr, IntPtr userdataptr)
        {
            if (playlistPtr != _playlist.Handle)
            {
                return;
            }

            _playlist.QueueThis<NativePlaylist, EventArgs>(
                    pc => pc.OnRenamed,
                    _playlist,
                    EventArgs.Empty);
        }

        private void OnTracksMovedCallback(IntPtr playlistPtr, IntPtr trackIndicesPtr, int numTracks, int newPosition, IntPtr userdataptr)
        {
            if (playlistPtr != _playlist.Handle)
            {
                return;
            }

            int[] trackIndices = new int[numTracks];
            Marshal.Copy(trackIndicesPtr, trackIndices, 0, numTracks);

            _playlist.QueueThis<NativePlaylist, TracksMovedEventArgs>(
                    pc => pc.OnTracksMoved,
                    _playlist,
                    new TracksMovedEventArgs(trackIndices, newPosition));
        }

        private void OnTracksRemovedCallback(IntPtr playlistPtr, IntPtr trackIndicesPtr, int numTracks, IntPtr userdataptr)
        {
            if (playlistPtr != _playlist.Handle)
            {
                return;
            }

            int[] trackIndices = new int[numTracks];
            Marshal.Copy(trackIndicesPtr, trackIndices, 0, numTracks);

            _playlist.QueueThis<NativePlaylist, TracksRemovedEventArgs>(
                    pc => pc.OnTracksRemoved,
                    _playlist,
                    new TracksRemovedEventArgs(trackIndices));
        }

        private void OnTracksAddedCallback(IntPtr playlistPtr, IntPtr tracksPtr, int numTracks, int position, IntPtr userdataptr)
        {
            if (playlistPtr != _playlist.Handle)
            {
                return;
            }

            var trackIndices = new int[numTracks];
            var tracks = new ITrack[numTracks];
            var trackPtrs = new IntPtr[numTracks];
            Marshal.Copy(tracksPtr, trackPtrs, 0, numTracks);

            for (int i = 0; i < numTracks; i++)
            {
                trackIndices[i] = position + i;
                tracks[i] = TrackManager.Get(_playlist.Session, trackPtrs[i]);
            }

            _playlist.QueueThis<NativePlaylist, TracksAddedEventArgs>(
                    pc => pc.OnTracksAdded,
                    _playlist,
                    new TracksAddedEventArgs(trackIndices, tracks));
        }

        private void OnSubscribersChangedCallback(IntPtr playlistPtr, IntPtr userdataptr)
        {
            if (playlistPtr != _playlist.Handle)
            {
                return;
            }

            _playlist.QueueThis<NativePlaylist, EventArgs>(
                    pc => pc.OnSubscribersChanged,
                    _playlist,
                    EventArgs.Empty);
        }

        #endregion Private Methods
    }
}