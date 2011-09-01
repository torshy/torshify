using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Torshify.Core.Native
{
    internal partial class Spotify
    {
        /// <summary>
        /// Increase reference count on playlistconatiner object
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <returns></returns>
        [DllImport("spotify")]
        internal static extern void sp_playlistcontainer_add_ref(IntPtr pcPtr);

        /// <summary>
        /// Release reference count on playlistconatiner object
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        [DllImport("spotify")]
        internal static extern void sp_playlistcontainer_release(IntPtr pcPtr);

        /// <summary>
        /// Return true if the playlistcontainer is fully loaded
        /// 
        /// The container_loaded callback will be invoked when this flips to true
        /// </summary>
        /// <param name="pcPtr">Playlist container</param>
        /// <returns>True if container is loaded</returns>
        [DllImport("spotify")]
        internal static extern bool sp_playlistcontainer_is_loaded(IntPtr pcPtr);

        /// <summary>
        /// Register interest in changes to a playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="callbacksPtr">Callbacks, see sp_playlistcontainer_callbacks.</param>
        /// <param name="userdataPtr">Opaque value passed to callbacks.</param>
        [DllImport("spotify")]
        internal static extern void sp_playlistcontainer_add_callbacks(IntPtr pcPtr, ref NativePlaylistContainerCallbacks.PlaylistContainerCallbacks callbacksPtr, IntPtr userdataPtr);

        /// <summary>
        /// Unregister interest in changes to a playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="callbacksPtr">Callbacks, see sp_playlistcontainer_callbacks</param>
        /// <param name="userdataPtr">Opaque value passed to callbacks.</param>
        [DllImport("spotify")]
        internal static extern void sp_playlistcontainer_remove_callbacks(IntPtr pcPtr, ref NativePlaylistContainerCallbacks.PlaylistContainerCallbacks callbacksPtr, IntPtr userdataPtr);

        /// <summary>
        /// Return the number of playlists in the given playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <returns>Number of playlists, -1 if undefined.</returns>
        [DllImport("spotify")]
        internal static extern int sp_playlistcontainer_num_playlists(IntPtr pcPtr);

        /// <summary>
        /// Return a pointer to the playlist at a specific index.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index in playlist container. Should be in the interval [0, sp_playlistcontainer_num_playlists() - 1].</param>
        /// <returns>Number of playlists.</returns>
        [DllImport("spotify")]
        internal static extern IntPtr sp_playlistcontainer_playlist(IntPtr pcPtr, int index);

        /// <summary>
        /// Return the type of the playlist at a index.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index in playlist container. Should be in the interval [0, sp_playlistcontainer_num_playlists() - 1].</param>
        /// <returns>Type of the playlist.</returns>
        [DllImport("spotify")]
        internal static extern PlaylistType sp_playlistcontainer_playlist_type(IntPtr pcPtr, int index);

        /// <summary>
        /// Gets the name of the playlist folder.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">The playlist index.</param>
        /// <param name="buffer">Pointer to name-buffer.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <returns></returns>
        [DllImport("spotify")]
        internal static extern Error sp_playlistcontainer_playlist_folder_name(
            IntPtr pcPtr, 
            int index, 
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringBuilderMarshaler))]StringBuilder buffer, 
            int bufferSize);

        /// <summary>
        /// Return the folder id at index.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index in playlist container. Should be in the interval [0, sp_playlistcontainer_num_playlists() - 1].</param>
        /// <returns>The group ID.</returns>
        [DllImport("spotify")]
        internal static extern IntPtr sp_playlistcontainer_playlist_folder_id(IntPtr pcPtr, int index);

        /// <summary>
        /// Add an empty playlist at the end of the playlist container. The name must not consist of only spaces and it must be shorter than 256 characters.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="name">Name of new playlist.</param>
        /// <returns>Pointer to the new playlist. Can be null if the operation fails.</returns>
        [DllImport("spotify")]
        internal static extern IntPtr sp_playlistcontainer_add_new_playlist(IntPtr pcPtr, string name);

        /// <summary>
        /// Add an existing playlist at the end of the given playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="linkPtr">Link object pointing to a playlist.</param>
        /// <returns>Pointer to the new playlist. Will be null if the playlist already exists.</returns>
        [DllImport("spotify")]
        internal static extern IntPtr sp_playlistcontainer_add_playlist(IntPtr pcPtr, IntPtr linkPtr);

        /// <summary>
        /// Remove playlist at index from the given playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index of playlist to be removed.</param>
        /// <returns>Error code.</returns>
        [DllImport("spotify")]
        internal static extern Error sp_playlistcontainer_remove_playlist(IntPtr pcPtr, int index);

        /// <summary>
        /// Move a playlist in the playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index of playlist to be moved.</param>
        /// <param name="newPosition">New position for the playlist.</param>
        /// <param name="dryRun">Do not execute the move, only check if it possible</param>
        /// <returns>Error code.</returns>
        [DllImport("spotify")]
        internal static extern Error sp_playlistcontainer_move_playlist(IntPtr pcPtr, int index, int newPosition, bool dryRun);

        /// <summary>
        /// Returns a pointer to the user object of the owner.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <returns>The user object or null if unknown or none.</returns>
        [DllImport("spotify")]
        internal static extern IntPtr sp_playlistcontainer_owner(IntPtr pcPtr);
    }
}