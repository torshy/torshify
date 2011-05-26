using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativePlaylist : NativeObject, IPlaylist
    {
        #region Fields

        private NativePlaylistCallbacks _callbacks;
        private Lazy<DelegateList<IPlaylistTrack>> _tracks;

        #endregion Fields

        #region Constructors

        public NativePlaylist(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Events

        public event EventHandler<DescriptionEventArgs> DescriptionChanged;

        public event EventHandler<ImageEventArgs> ImageChanged;

        public event EventHandler MetadataUpdated;

        public event EventHandler Renamed;

        public event EventHandler StateChanged;

        public event EventHandler<TrackCreatedChangedEventArgs> TrackCreatedChanged;

        public event EventHandler<TracksAddedEventArgs> TracksAdded;

        public event EventHandler<TrackSeenEventArgs> TrackSeenChanged;

        public event EventHandler<TracksMovedEventArgs> TracksMoved;

        public event EventHandler<TracksRemovedEventArgs> TracksRemoved;

        public event EventHandler<PlaylistUpdateEventArgs> UpdateInProgress;

        public event EventHandler SubscribersChanged;

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
                    return Spotify.GetString(Spotify.sp_playlist_get_description(Handle), String.Empty);
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
                    return Spotify.GetString(Spotify.sp_playlist_name(Handle), String.Empty);
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

        public IEditableArray<IPlaylistTrack> Tracks
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

                lock(Spotify.Mutex)
                {
                    return Spotify.sp_playlist_get_offline_status(Session.GetHandle(), Handle);
                }
            }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            lock (Spotify.Mutex)
            {
                Spotify.sp_playlist_add_ref(Handle);
                Spotify.sp_playlist_update_subscribers(Session.GetHandle(), Handle);
            }

            _callbacks = new NativePlaylistCallbacks(this);
            _tracks = new Lazy<DelegateList<IPlaylistTrack>>(() => new DelegateList<IPlaylistTrack>(
                GetNumberOfTracks,
                GetTrackIndex,
                AddTrack,
                RemoveTrack,
                () => false));
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

            lock(Spotify.Mutex)
            {
                Spotify.sp_playlist_set_offline_mode(Session.GetHandle(), Handle, offline);
            }
        }

        public int GetOfflineDownloadCompleted()
        {
            AssertHandle();

            lock(Spotify.Mutex)
            {
                return Spotify.sp_playlist_get_offline_download_completed(Session.GetHandle(), Handle);
            }
        }

        #endregion Public Methods

        #region Internal Methods

        internal void OnTracksMoved(TracksMovedEventArgs e)
        {
            //SpotifyPlaylistTrackHandler.Update(this, e.TrackIndices, e.NewPosition);
            TracksMoved.RaiseEvent(this, e);
        }

        internal void OnTracksRemoved(TracksRemovedEventArgs e)
        {
            //SpotifyPlaylistTrackHandler.Update(this, aditions: e.TrackIndices);
            TracksRemoved.RaiseEvent(this, e);
        }

        internal void OnTracksAdded(TracksAddedEventArgs e)
        {
            //SpotifyPlaylistTrackHandler.Update(this, aditions: e.TrackIndices);
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
            IntPtr[] ptrArray = new IntPtr[1];
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