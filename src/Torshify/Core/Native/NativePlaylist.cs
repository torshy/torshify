using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativePlaylist : NativeObject, IPlaylist
    {
        #region Fields

        private NativePlaylistCallbacks _callbacks;
        private Lazy<NativePlaylistTrackList> _tracks;
        private List<string> _subscribers;

        #endregion Fields

        #region Constructors

        public NativePlaylist(ISession session, IntPtr handle)
            : base(session, handle)
        {
            _subscribers = new List<string>();
        }

        #endregion Constructors

        #region Events

        public event EventHandler<DescriptionEventArgs> DescriptionChanged;

        public event EventHandler<ImageEventArgs> ImageChanged;

        public event EventHandler MetadataUpdated;

        public event EventHandler Renamed;

        public event EventHandler StateChanged;

        public event EventHandler SubscribersChanged;

        public event EventHandler<TrackCreatedChangedEventArgs> TrackCreatedChanged;

        public event EventHandler<TracksAddedEventArgs> TracksAdded;

        public event EventHandler<TrackSeenEventArgs> TrackSeenChanged;

        public event EventHandler<TracksMovedEventArgs> TracksMoved;

        public event EventHandler<TracksRemovedEventArgs> TracksRemoved;

        public event EventHandler<PlaylistUpdateEventArgs> UpdateInProgress;

        #endregion Events

        #region Properties

        public bool IsLoaded
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_playlist_is_loaded(Handle);
                }
            }
        }

        public string Description
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_playlist_get_description(Handle);
                }
            }
        }

        public string ImageId
        {
            get
            {
                AssertHandle();

                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * 20);
                bool ans;
                lock (Spotify.Mutex)
                {
                    ans = Spotify.sp_playlist_get_image(Handle, ptr);
                }

                try
                {
                    if (ans)
                    {
                        return Spotify.ImageIdToString(ptr);
                    }
                    else
                    {
                        return null;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        public bool IsCollaborative
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_playlist_is_collaborative(Handle);
                }
            }
            set
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    Spotify.sp_playlist_set_collaborative(Handle, value);
                }
            }
        }

        public virtual string Name
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_playlist_name(Handle);
                }
            }
            set
            {
                AssertHandle();

                if (value.Length > 255)
                {
                    throw new ArgumentException("value can't be longer than 255 chars.");
                }

                lock (Spotify.Mutex)
                {
                    Spotify.sp_playlist_rename(Handle, value);
                }
            }
        }

        public bool PendingChanges
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_playlist_has_pending_changes(Handle);
                }
            }
        }

        public IPlaylistTrackList Tracks
        {
            get
            {
                AssertHandle();

                return _tracks.Value;
            }
        }

        public PlaylistOfflineStatus OfflineStatus
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_playlist_get_offline_status(Session.GetHandle(), Handle);
                }
            }
        }

        public ReadOnlyCollection<string> Subscribers
        {
            get
            {
                return _subscribers.AsReadOnly();
            }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            _callbacks = new NativePlaylistCallbacks(this);
            _tracks = new Lazy<NativePlaylistTrackList>(() => new NativePlaylistTrackList(
                GetNumberOfTracks,
                GetTrackIndex,
                AddTrack,
                AddNewTrack,
                RemoveTrack,
                () => false));

            lock (Spotify.Mutex)
            {
                Spotify.sp_playlist_add_ref(Handle);
                Spotify.sp_playlist_update_subscribers(Session.GetHandle(), Handle);
            }
        }

        public void AutoLinkTracks(bool autoLink)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Spotify.sp_playlist_set_autolink_tracks(Handle, autoLink);
            }
        }

        public void SetOfflineMode(bool offline)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Spotify.sp_playlist_set_offline_mode(Session.GetHandle(), Handle, offline);
            }
        }

        public int GetOfflineDownloadCompleted()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_playlist_get_offline_download_completed(Session.GetHandle(), Handle);
            }
        }

        #endregion Public Methods

        #region Internal Methods

        internal void OnTracksMoved(TracksMovedEventArgs e)
        {
            TracksMoved.RaiseEvent(this, e);
        }

        internal void OnTracksRemoved(TracksRemovedEventArgs e)
        {
            TracksRemoved.RaiseEvent(this, e);
        }

        internal void OnTracksAdded(TracksAddedEventArgs e)
        {
            TracksAdded.RaiseEvent(this, e);
        }

        internal void OnUpdateInProgress(PlaylistUpdateEventArgs e)
        {
            UpdateInProgress.RaiseEvent(this, e);
        }

        internal void OnStateChanged(EventArgs e)
        {
            StateChanged.RaiseEvent(this, e);
        }

        internal void OnRenamed(EventArgs e)
        {
            Renamed.RaiseEvent(this, e);
        }

        internal void OnDescriptionChanged(DescriptionEventArgs e)
        {
            DescriptionChanged.RaiseEvent(this, e);
        }

        internal void OnTrackSeenChanged(TrackSeenEventArgs e)
        {
            TrackSeenChanged.RaiseEvent(this, e);
        }

        internal void OnTrackCreatedChanged(TrackCreatedChangedEventArgs e)
        {
            TrackCreatedChanged.RaiseEvent(this, e);
        }

        internal void OnMetadataUpdated(EventArgs e)
        {
            MetadataUpdated.RaiseEvent(this, e);
        }

        internal void OnImageChanged(ImageEventArgs e)
        {
            ImageChanged.RaiseEvent(this, e);
        }

        internal void OnSubscribersChanged(EventArgs e)
        {
            GetSubscribers();
            SubscribersChanged.RaiseEvent(this, e);
        }

        #endregion Internal Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            if (!IsInvalid)
            {
                if (_callbacks != null)
                {
                    _callbacks.Dispose();
                    _callbacks = null;
                }

                try
                {
                    lock (Spotify.Mutex)
                    {
                        Spotify.sp_playlist_release(Handle);
                    }
                }
                catch
                {
                }
                finally
                {
                    PlaylistTrackManager.RemoveAll(this);
                    PlaylistManager.Remove(Handle);
                    Handle = IntPtr.Zero;
                    Debug.WriteLine("Playlist disposed");
                }
            }

            base.Dispose(disposing);
        }

        #endregion Protected Methods

        #region Private Methods

        private void GetSubscribers()
        {
            lock (Spotify.Mutex)
            {
                // NOTE: Something isn't correct with the sp_playlist_subscribers marshalling. The 'count' field is not correct at all, but the array of user pointers are.
                var subscribersPtr = Spotify.sp_playlist_subscribers(Handle);
                var numberOfSubscribers = Spotify.sp_playlist_num_subscribers(Handle);

                var arrayPtr = IntPtr.Add(subscribersPtr, sizeof(uint));
                var arrayPtrs = new IntPtr[Math.Min(numberOfSubscribers, 500)];
                Marshal.Copy(arrayPtr, arrayPtrs, 0, arrayPtrs.Length);

                _subscribers.Clear();

                for (int i = 0; i < arrayPtrs.Length; i++)
                {
                    string userName = Spotify.GetString(arrayPtrs[i], string.Empty);

                    if (!string.IsNullOrEmpty(userName))
                    {
                        _subscribers.Add(userName);
                    }
                }

                // TODO : Throws AccessViolationException.
                // Spotify.sp_playlist_subscribers_free(subscribersPtr);
            }
        }

        private void RemoveTrack(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Spotify.sp_playlist_remove_tracks(Handle, new[] { index }, 1);
            }
        }

        private void AddTrack(IPlaylistTrack track, int index)
        {
            AddNewTrack(track, index);
        }

        private void AddNewTrack(ITrack track, int index)
        {
            IntPtr[] ptrArray = new[] { track.GetHandle() };
            IntPtr trackArrayPtr = Marshal.AllocHGlobal(Marshal.SizeOf(ptrArray));
            Marshal.Copy(ptrArray, 0, trackArrayPtr, 1);

            AssertHandle();

            lock (Spotify.Mutex)
            {
                Spotify.sp_playlist_add_tracks(Handle, trackArrayPtr, 1, index, Session.GetHandle());
            }

            Marshal.FreeHGlobal(trackArrayPtr);
        }

        private IPlaylistTrack GetTrackIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return PlaylistTrackManager.Get(Session, this, Spotify.sp_playlist_track(Handle, index), index);
            }
        }

        private int GetNumberOfTracks()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_playlist_num_tracks(Handle);
            }
        }

        #endregion Private Methods
    }
}