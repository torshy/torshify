using System;
using System.Runtime.InteropServices;
using System.Linq;

namespace Torshify.Core.Native
{
    internal class NativePlaylistContainerCallbacks : IDisposable
    {
        #region Fields

        internal static readonly int CallbacksSize = Marshal.SizeOf(typeof (PlaylistContainerCallbacks));

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
                _callbacks.container_loaded = Marshal.GetFunctionPointerForDelegate(_containerLoaded);
                _callbacks.playlist_added = Marshal.GetFunctionPointerForDelegate(_playlistAdded);
                _callbacks.playlist_moved = Marshal.GetFunctionPointerForDelegate(_playlistMoved);
                _callbacks.playlist_removed = Marshal.GetFunctionPointerForDelegate(_playlistRemoved);   
 
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
            if (containerPointer != _container.Handle) return;

            _container.QueueThis<NativePlaylistContainer, EventArgs>(
                pc => pc.OnLoaded,
                _container,
                EventArgs.Empty);
        }

        private void OnPlaylistAddedCallback(IntPtr containerPointer, IntPtr playlistptr, int position, IntPtr userdataptr)
        {
            if (containerPointer != _container.Handle) return;

            _container.QueueThis<NativePlaylistContainer, PlaylistEventArgs>(
                pc => pc.OnPlaylistAdded,
                _container,
                new PlaylistEventArgs(null, position));
        }

        private void OnPlaylistRemovedCallback(IntPtr containerPointer, IntPtr playlistptr, int position, IntPtr userdataptr)
        {
            if (containerPointer != _container.Handle) return;

            _container.QueueThis<NativePlaylistContainer, PlaylistEventArgs>(
                pc => pc.OnPlaylistRemoved,
                _container,
                new PlaylistEventArgs(null, position));
        }

        private void OnPlaylistMovedCallback(IntPtr containerPointer, IntPtr playlistptr, int position, int newposition, IntPtr userdataptr)
        {
            if (containerPointer != _container.Handle) return;

            IContainerPlaylist playlist = _container.Playlists.FirstOrDefault(p=>p.GetHandle() == playlistptr);
            _container.QueueThis<NativePlaylistContainer, PlaylistMovedEventArgs>(
                pc => pc.OnPlaylistMoved,
                _container,
                new PlaylistMovedEventArgs(playlist, position, newposition));
        }

        #endregion Private Methods

        #region Nested Types

        [StructLayout(LayoutKind.Sequential)]
        internal struct PlaylistContainerCallbacks
        {
            #region Fields

            internal IntPtr playlist_added;
            internal IntPtr playlist_removed;
            internal IntPtr playlist_moved;
            internal IntPtr container_loaded;

            #endregion Fields
        }

        #endregion Nested Types
    }
}