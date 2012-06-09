using System;
using System.Runtime.InteropServices;

using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativePlaylistContainerCallbacks : IDisposable
    {
        #region Fields

        internal static readonly int CallbacksSize = Marshal.SizeOf(typeof(PlaylistContainerCallbacks));

        private readonly NativePlaylistContainer _container;

        private ContainerLoadedCallback _containerLoaded;
        private PlaylistAddedCallback _playlistAdded;
        private PlaylistMovedCallback _playlistMoved;
        private PlaylistRemovedCallback _playlistRemoved;
        private PlaylistContainerCallbacks _callbacks;

        #endregion Fields

        #region Constructors

        public NativePlaylistContainerCallbacks(NativePlaylistContainer container)
        {
            _container = container;
            _playlistAdded = OnPlaylistAddedCallback;
            _playlistRemoved = OnPlaylistRemovedCallback;
            _playlistMoved = OnPlaylistMovedCallback;
            _containerLoaded = OnLoadedCallback;

            lock (Spotify.Mutex)
            {
                _callbacks = new PlaylistContainerCallbacks();
                _callbacks.ContainerLoaded = Marshal.GetFunctionPointerForDelegate(_containerLoaded);
                _callbacks.PlaylistAdded = Marshal.GetFunctionPointerForDelegate(_playlistAdded);
                _callbacks.PlaylistMoved = Marshal.GetFunctionPointerForDelegate(_playlistMoved);
                _callbacks.PlaylistRemoved = Marshal.GetFunctionPointerForDelegate(_playlistRemoved);

                Spotify.sp_playlistcontainer_add_callbacks(container.Handle, ref _callbacks, IntPtr.Zero);
            }
        }

        #endregion Constructors

        #region Delegates

        internal delegate void ContainerLoadedCallback(IntPtr pcPtr, IntPtr userdataPtr);

        internal delegate void PlaylistAddedCallback(IntPtr pcPtr, IntPtr playlistPtr, int position, IntPtr userdataPtr);

        internal delegate void PlaylistMovedCallback(IntPtr pcPtr, IntPtr playlistPtr, int position, int newPosition, IntPtr userdataPtr);

        internal delegate void PlaylistRemovedCallback(IntPtr pcPtr, IntPtr playlistPtr, int position, IntPtr userdataPtr);

        #endregion Delegates

        #region Public Methods

        public void Dispose()
        {
            try
            {
                lock (Spotify.Mutex)
                {
                    Spotify.sp_playlistcontainer_remove_callbacks(_container.Handle, ref _callbacks, IntPtr.Zero);
                }
            }
            catch
            {
                // Ignore
            }
            finally
            {
                GC.KeepAlive(_callbacks);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void OnLoadedCallback(IntPtr containerPointer, IntPtr userdataptr)
        {
            if (containerPointer != _container.Handle)
            {
                return;
            }

            _container.QueueThis(() => _container.OnLoaded(EventArgs.Empty));
        }

        private void OnPlaylistAddedCallback(IntPtr containerPointer, IntPtr playlistptr, int position, IntPtr userdataptr)
        {
            if (containerPointer != _container.Handle)
            {
                return;
            }

            var containerPlaylist = ContainerPlaylistManager.Get(
                _container.Session,
                _container,
                playlistptr,
                Spotify.sp_playlistcontainer_playlist_folder_id(containerPointer, position),
                Spotify.sp_playlistcontainer_playlist_type(containerPointer, position));

            var args = new PlaylistEventArgs(containerPlaylist, position);

            _container.QueueThis(() => _container.OnPlaylistAdded(args));
        }

        private void OnPlaylistRemovedCallback(IntPtr containerPointer, IntPtr playlistptr, int position, IntPtr userdataptr)
        {
            if (containerPointer != _container.Handle)
            {
                return;
            }

            var containerPlaylist = ContainerPlaylistManager.Get(
                _container.Session,
                _container,
                playlistptr,
                Spotify.sp_playlistcontainer_playlist_folder_id(containerPointer, position),
                Spotify.sp_playlistcontainer_playlist_type(containerPointer, position));

            var args = new PlaylistEventArgs(containerPlaylist, position);

            _container.QueueThis(() => _container.OnPlaylistRemoved(args));
        }

        private void OnPlaylistMovedCallback(IntPtr containerPointer, IntPtr playlistptr, int position, int newposition, IntPtr userdataptr)
        {
            if (containerPointer != _container.Handle)
            {
                return;
            }

            // HACK : The indices are wrong when moving playlists up
            if (position < newposition)
            {
                newposition--;
            }

            var containerPlaylist = ContainerPlaylistManager.Get(
                _container.Session,
                _container,
                playlistptr,
                Spotify.sp_playlistcontainer_playlist_folder_id(containerPointer, position),
                Spotify.sp_playlistcontainer_playlist_type(containerPointer, position));

            var args = new PlaylistMovedEventArgs(containerPlaylist, position, newposition);

            _container.QueueThis(() => _container.OnPlaylistMoved(args));
        }

        #endregion Private Methods

        #region Nested Types

        [StructLayout(LayoutKind.Sequential)]
        internal struct PlaylistContainerCallbacks
        {
            internal IntPtr PlaylistAdded;
            internal IntPtr PlaylistRemoved;
            internal IntPtr PlaylistMoved;
            internal IntPtr ContainerLoaded;
        }

        #endregion Nested Types
    }
}