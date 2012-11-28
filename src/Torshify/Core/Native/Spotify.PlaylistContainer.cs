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
        [DllImport("libspotify")]
        internal static extern Error sp_playlistcontainer_add_ref(IntPtr pcPtr);

        /// <summary>
        /// Release reference count on playlistconatiner object
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_playlistcontainer_release(IntPtr pcPtr);

        /// <summary>
        /// Return true if the playlistcontainer is fully loaded
        /// 
        /// The container_loaded callback will be invoked when this flips to true
        /// </summary>
        /// <param name="pcPtr">Playlist container</param>
        /// <returns>True if container is loaded</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_playlistcontainer_is_loaded(IntPtr pcPtr);

        /// <summary>
        /// Register interest in changes to a playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="callbacksPtr">Callbacks, see sp_playlistcontainer_callbacks.</param>
        /// <param name="userdataPtr">Opaque value passed to callbacks.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_playlistcontainer_add_callbacks(IntPtr pcPtr, ref NativePlaylistContainerCallbacks.PlaylistContainerCallbacks callbacksPtr, IntPtr userdataPtr);

        /// <summary>
        /// Unregister interest in changes to a playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="callbacksPtr">Callbacks, see sp_playlistcontainer_callbacks</param>
        /// <param name="userdataPtr">Opaque value passed to callbacks.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_playlistcontainer_remove_callbacks(IntPtr pcPtr, ref NativePlaylistContainerCallbacks.PlaylistContainerCallbacks callbacksPtr, IntPtr userdataPtr);

        /// <summary>
        /// Return the number of playlists in the given playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <returns>Number of playlists, -1 if undefined.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_playlistcontainer_num_playlists(IntPtr pcPtr);

        /// <summary>
        /// Return a pointer to the playlist at a specific index.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index in playlist container. Should be in the interval [0, sp_playlistcontainer_num_playlists() - 1].</param>
        /// <returns>Number of playlists.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlistcontainer_playlist(IntPtr pcPtr, int index);

        /// <summary>
        /// Return the type of the playlist at a index.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index in playlist container. Should be in the interval [0, sp_playlistcontainer_num_playlists() - 1].</param>
        /// <returns>Type of the playlist.</returns>
        [DllImport("libspotify")]
        internal static extern PlaylistType sp_playlistcontainer_playlist_type(IntPtr pcPtr, int index);

        /// <summary>
        /// Gets the name of the playlist folder.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">The playlist index.</param>
        /// <param name="buffer">Pointer to name-buffer.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern Error sp_playlistcontainer_playlist_folder_name(
            IntPtr pcPtr, 
            int index, 
            IntPtr buffer, 
            int bufferSize);

        /// <summary>
        /// Return the folder id at index.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index in playlist container. Should be in the interval [0, sp_playlistcontainer_num_playlists() - 1].</param>
        /// <returns>The group ID.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlistcontainer_playlist_folder_id(IntPtr pcPtr, int index);

        /// <summary>
        /// Add an empty playlist at the end of the playlist container. The name must not consist of only spaces and it must be shorter than 256 characters.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="name">Name of new playlist.</param>
        /// <returns>Pointer to the new playlist. Can be null if the operation fails.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlistcontainer_add_new_playlist(IntPtr pcPtr, string name);

        /// <summary>
        /// Add a playlist folder
        /// 
        /// This operation will actually create two playlists. One of type SP_PLAYLIST_TYPE_START_FOLDER and immediately following a SP_PLAYLIST_TYPE_END_FOLDER one.
        /// 
        /// To remove a playlist folder both of these must be deleted or the list will be left in an inconsistant state.
        /// 
        /// There is no way to rename a playlist folder. Instead you need to remove the folder and recreate it again.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="name">Name of new playlist.</param>
        /// <returns>Pointer to the new playlist. Can be null if the operation fails.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_playlistcontainer_add_folder(IntPtr pcPtr, int index, string name);

        /// <summary>
        /// Add an existing playlist at the end of the given playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="linkPtr">Link object pointing to a playlist.</param>
        /// <returns>Pointer to the new playlist. Will be null if the playlist already exists.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlistcontainer_add_playlist(IntPtr pcPtr, IntPtr linkPtr);

        /// <summary>
        /// Remove playlist at index from the given playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index of playlist to be removed.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_playlistcontainer_remove_playlist(IntPtr pcPtr, int index);

        /// <summary>
        /// Move a playlist in the playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index of playlist to be moved.</param>
        /// <param name="newPosition">New position for the playlist.</param>
        /// <param name="dryRun">Do not execute the move, only check if it possible</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_playlistcontainer_move_playlist(IntPtr pcPtr, int index, int newPosition, bool dryRun);

        /// <summary>
        /// Returns a pointer to the user object of the owner.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <returns>The user object or null if unknown or none.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlistcontainer_owner(IntPtr pcPtr);

        /// <summary>
        /// Get the number of new tracks in a playlist since the corresponding
        /// function sp_playlistcontainer_clear_unseen_tracks() was called. The
        /// function always returns the number of new tracks, and fills the
        /// tracks array with the new tracks, but not more than specified in
        /// num_tracks. The function will return a negative value on failure.
        /// </summary>
        /// <param name="pcPtr"></param>
        /// <param name="playlistPtr"></param>
        /// <param name="tracks"></param>
        /// <param name="maxNumberOfTracks"></param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern int sp_playlistcontainer_get_unseen_tracks(IntPtr pcPtr, IntPtr playlistPtr, IntPtr tracks, int maxNumberOfTracks);

        /// <summary>
        /// Clears a playlist from unseen tracks, so that next call to sp_playlistcontainer_get_unseen_tracks() will return 0 until a new track is added to the \p playslist.
        /// </summary>
        /// <param name="pcPtr"></param>
        /// <param name="playlistPtr"></param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern int sp_playlistcontainer_clear_unseen_tracks(IntPtr pcPtr, IntPtr playlistPtr);
    }
}