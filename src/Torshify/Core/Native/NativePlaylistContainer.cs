using System;
using System.Diagnostics;
using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativePlaylistContainer : NativeObject, IPlaylistContainer
    {
        #region Fields

        private Lazy<DelegateList<IContainerPlaylist>> _playlists;
        private NativePlaylistContainerCallbacks _callbacks;

        #endregion Fields

        #region Constructors

        public NativePlaylistContainer(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Events

        public event EventHandler Loaded;

        public event EventHandler<PlaylistEventArgs> PlaylistAdded;

        public event EventHandler<PlaylistMovedEventArgs> PlaylistMoved;

        public event EventHandler<PlaylistEventArgs> PlaylistRemoved;

        #endregion Events

        #region Properties

        public IUser Owner
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return UserManager.Get(Session, Spotify.sp_playlistcontainer_owner(Handle));
                }
            }
        }

        public bool IsLoaded
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_playlistcontainer_is_loaded(Handle);
                }
            }
        }

        public IEditableArray<IContainerPlaylist> Playlists
        {
            get
            {
                AssertHandle();

                return _playlists.Value;
            }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            lock (Spotify.Mutex)
            {
                Spotify.sp_playlistcontainer_add_ref(Handle);
            }

            _callbacks = new NativePlaylistContainerCallbacks(this);
            _playlists = new Lazy<DelegateList<IContainerPlaylist>>(() => new DelegateList<IContainerPlaylist>(
                GetContainerLength,
                GetPlaylistAtIndex,
                AddPlaylist,
                RemovePlaylist,
                () => false,
                MovePlaylists));
        }

        #endregion Public Methods

        #region Internal Methods

        internal virtual void OnLoaded(EventArgs e)
        {
            Loaded.RaiseEvent(this, e);
        }

        internal virtual void OnPlaylistAdded(PlaylistEventArgs e)
        {
            PlaylistAdded.RaiseEvent(this, e);
        }

        internal virtual void OnPlaylistMoved(PlaylistMovedEventArgs e)
        {
            PlaylistMoved.RaiseEvent(this, e);
        }

        internal virtual void OnPlaylistRemoved(PlaylistEventArgs e)
        {
            PlaylistRemoved.RaiseEvent(this, e);
        }

        #endregion Internal Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            // Dispose managed
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
                        Spotify.sp_playlistcontainer_release(Handle);
                    }
                }
                catch
                {
                }
                finally
                {
                    PlaylistContainerManager.Remove(Handle);
                    Handle = IntPtr.Zero;
                    Debug.WriteLine("Playlist container disposed");
                }
            }

            base.Dispose(disposing);
        }

        #endregion Protected Methods

        #region Private Methods

        private void RemovePlaylist(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Spotify.sp_playlistcontainer_remove_playlist(Handle, index);
            }
        }

        private void AddPlaylist(IContainerPlaylist playlist, int index)
        {
            AssertHandle();
            IntPtr playlistPtr;
            int newIndex;

            lock (Spotify.Mutex)
            {
                playlistPtr = Spotify.sp_playlistcontainer_add_new_playlist(Handle, playlist.Name);
                newIndex = Spotify.sp_playlistcontainer_num_playlists(Handle);
            }

            if (playlistPtr != IntPtr.Zero)
            {
                lock (Spotify.Mutex)
                {
                    Spotify.sp_playlistcontainer_move_playlist(Handle, newIndex, index, false);
                }
            }
        }

        private int GetContainerLength()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_playlistcontainer_num_playlists(Handle);
            }
        }

        private IContainerPlaylist GetPlaylistAtIndex(int index)
        {
            lock (Spotify.Mutex)
            {
                return ContainerPlaylistManager.Get(
                    Session,
                    this,
                    Spotify.sp_playlistcontainer_playlist(Handle, index),
                    Spotify.sp_playlistcontainer_playlist_folder_id(Handle, index),
                    Spotify.sp_playlistcontainer_playlist_type(Handle, index));
            }
        }

        private void MovePlaylists(int oldIndex, int newIndex)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                Spotify.sp_playlistcontainer_move_playlist(Handle, oldIndex, newIndex, false);
            }
        }

        #endregion Private Methods
    }
}