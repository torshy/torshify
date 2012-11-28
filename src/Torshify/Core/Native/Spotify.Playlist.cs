using System;
using System.Runtime.InteropServices;

namespace Torshify.Core.Native
{
    internal partial class Spotify
    {
        #region Delegates

        public delegate void DescriptionChangedCallback(IntPtr playlistPtr, IntPtr descPtr, IntPtr userdataPtr);

        public delegate void ImageChangedCallback(IntPtr playlistPtr, IntPtr imgIdPtr, IntPtr userdataPtr);

        public delegate void PlaylistMetadataUpdatedCallback(IntPtr playlistPtr, IntPtr userdataPtr);

        public delegate void PlaylistRenamedCallback(IntPtr playlistPtr, IntPtr userdataPtr);

        public delegate void PlaylistStateChangedCallback(IntPtr playlistPtr, IntPtr userdataPtr);

        public delegate void PlaylistUpdateInProgressCallback(IntPtr playlistPtr, bool done, IntPtr userdataPtr);

        public delegate void SubscribersChangedCallback(IntPtr playlistPtr, IntPtr userdataPtr);

        public delegate void TrackCreatedChangedCallback(IntPtr playlistPtr, int position, IntPtr userPtr, int when, IntPtr userdataPtr);

        public delegate void TracksAddedCallback(IntPtr playlistPtr, IntPtr tracksPtr, int numTracks, int position, IntPtr userdataPtr);

        public delegate void TrackSeenChangedCallback(IntPtr playlistPtr, int position, bool seen, IntPtr userdataPtr);

        public delegate void TracksMovedCallback(IntPtr playlistPtr, IntPtr trackIndicesPtr, int numTracks, int newPosition, IntPtr userdataPtr);

        public delegate void TracksRemovedCallback(IntPtr playlistPtr, IntPtr trackIndicesPtr, int numTracks, IntPtr userdataPtr);

        #endregion Delegates

        #region Internal Static Methods

        /// <summary>
        /// Get load status for the specified playlist. If it's false, you have to wait until playlist_state_changed happens,
        /// and check again if is_loaded has changed.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <returns>True if playlist is loaded, otherwise false.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_playlist_is_loaded(IntPtr playlistPtr);

        /// <summary>
        /// Register interest in the given playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="callbacksPtr">Callbacks, see sp_playlist_callbacks.</param>
        /// <param name="userdataPtr">Userdata to be passed to callbacks.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_playlist_add_callbacks(IntPtr playlistPtr, ref PlaylistCallbacks callbacksPtr, IntPtr userdataPtr);

        /// <summary>
        /// Unregister interest in the given playlist.
        /// The combination of (callbacks, userdata) is used to find the entry to be removed.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="callbacksPtr">Callbacks, see sp_playlist_callbacks.</param>
        /// <param name="userdataPtr">Userdata to be passed to callbacks.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_playlist_remove_callbacks(IntPtr playlistPtr, ref PlaylistCallbacks callbacksPtr, IntPtr userdataPtr);

        /// <summary>
        /// Return number of tracks in the given playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <returns>The number of tracks in the playlist.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_playlist_num_tracks(IntPtr playlistPtr);

        /// <summary>
        /// Return the track at the given index in a playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="index">Index into playlist container. Should be in the interval [0, sp_playlist_num_tracks() - 1].</param>
        /// <returns>The track at the given index.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlist_track(IntPtr playlistPtr, int index);

        /// <summary>
        /// Return when the given index was added to the playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="index">Index into playlist container. Should be in the interval [0, sp_playlist_num_tracks() - 1].</param>
        /// <returns>Time, Seconds since unix epoch.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_playlist_track_create_time(IntPtr playlistPtr, int index);

        /// <summary>
        /// Return user that added the given index in the playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="index">Index into playlist container. Should be in the interval [0, sp_playlist_num_tracks() - 1].</param>
        /// <returns>User object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlist_track_creator(IntPtr playlistPtr, int index);

        /// <summary>
        /// Return if a playlist entry is marked as seen or not.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="index">Index into playlist container. Should be in the interval [0, sp_playlist_num_tracks() - 1].</param>
        /// <returns>Seen state.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_playlist_track_seen(IntPtr playlistPtr, int index);

        /// <summary>
        /// Return name of given playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <returns>The name of the given playlist.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
        internal static extern string sp_playlist_name(IntPtr playlistPtr);

        /// <summary>
        /// Rename the given playlist The name must not consist of only spaces and it must be shorter than 256 characters.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="newName">New name for playlist.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_playlist_rename(IntPtr playlistPtr, string newName);

        /// <summary>
        /// Return a pointer to the user for the given playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <returns>User object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlist_owner(IntPtr playlistPtr);

        /// <summary>
        /// Return collaborative status for a playlist.
        /// A playlist in collaborative state can be modifed by all users, not only the user owning the list.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <returns>true if playlist is collaborative, otherwise false.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_playlist_is_collaborative(IntPtr playlistPtr);

        /// <summary>
        /// Set collaborative status for a playlist.
        /// A playlist in collaborative state can be modifed by all users, not only the user owning the list.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="collaborative">Wheater or not the playlist should be collaborative.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_playlist_set_collaborative(IntPtr playlistPtr, bool collaborative);

        /// <summary>
        /// Set autolinking state for a playlist.
        /// If a playlist is autolinked, unplayable tracks will be made playable by linking them to other Spotify tracks, where possible.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="link">The new value.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_playlist_set_autolink_tracks(IntPtr playlistPtr, bool link);

        /// <summary>
        /// Get description for a playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <returns>Description</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
        internal static extern string sp_playlist_get_description(IntPtr playlistPtr);

        /// <summary>
        /// Get image for a playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="imageId">[out] 20 byte image id.</param>
        /// <returns>True if playlist has an image, otherwise false.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_playlist_get_image(IntPtr playlistPtr, IntPtr imageId);

        /// <summary>
        /// Check if a playlist has pending changes.
        /// Pending changes are local changes that have not yet been acknowledged by the server.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <returns>A flag representing if there are pending changes or not.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_playlist_has_pending_changes(IntPtr playlistPtr);

        /// <summary>
        /// Add tracks to a playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="trackArrayPtr">Array of pointer to tracks.</param>
        /// <param name="numTracks">Count of <c>tracks</c> array.</param>
        /// <param name="position">Start position in playlist where to insert the tracks.</param>
        /// <param name="sessionPtr">Your session object.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_playlist_add_tracks(IntPtr playlistPtr, IntPtr trackArrayPtr, int numTracks, int position, IntPtr sessionPtr);

        /// <summary>
        /// Remove tracks from a playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="trackIndices">Array of pointer to track indices.
        /// A certain track index should be present at most once, e.g. [0, 1, 2] is valid indata, whereas [0, 1, 1] is invalid.</param>
        /// <param name="numTracks">Count of <c>trackIndices</c> array.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_playlist_remove_tracks(IntPtr playlistPtr, int[] trackIndices, int numTracks);

        /// <summary>
        /// Move tracks in playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="trackIndices">Array of pointer to track indices to be moved.
        /// A certain track index should be present at most once, e.g. [0, 1, 2] is valid indata, whereas [0, 1, 1] is invalid.</param>
        /// <param name="numTracks">Count of <c>trackIndices</c> array.</param>
        /// <param name="newPosition">New position for tracks.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_playlist_reorder_tracks(IntPtr playlistPtr, int[] trackIndices, int numTracks, int newPosition);

        /// <summary>
        /// Return number of subscribers for a given playlist
        /// </summary>
        /// <param name="playlistPtr">The playlist object.</param>
        /// <returns>Number of subscribers</returns>
        [DllImport("libspotify")]
        internal static extern int sp_playlist_num_subscribers(IntPtr playlistPtr);

        /// <summary>
        /// Return subscribers for a playlist
        /// </summary>
        /// <param name="playlistPtr">The playlist object.</param>
        /// <returns>sp_subscribers struct with array of canonical usernames. This object should be free'd using sp_playlist_subscribers_free()</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlist_subscribers(IntPtr playlistPtr);

        /// <summary>
        /// Free object returned from sp_playlist_subscribers()
        /// </summary>
        /// <param name="subscribersPtr">The Subscribers object.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_playlist_subscribers_free(IntPtr subscribersPtr);

        /// <summary>
        /// Ask library to update the subscription count for a playlist
        /// 
        /// When the subscription info has been fetched from the Spotify backend
        /// the playlist subscribers_changed() callback will be invoked.
        /// In that callback use sp_playlist_num_subscribers() and/or
        /// sp_playlist_subscribers() to get information about the subscribers.
        /// You can call those two functions anytime you want but the information
        /// might not be up to date in such cases
        /// </summary>
        /// <param name="sessionPtr">The session object</param>
        /// <param name="playlistPtr">Playlist object.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_playlist_update_subscribers(IntPtr sessionPtr, IntPtr playlistPtr);

        /// <summary>
        /// Load an already existing playlist without adding it to a playlistcontainer.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <param name="linkPtr">Link object referring to a playlist.</param>
        /// <returns>A playlist. The reference is owned by the caller and should be released with sp_playlist_release().</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlist_create(IntPtr sessionPtr, IntPtr linkPtr);

        /// <summary>
        /// Increase the reference count of a playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_playlist_add_ref(IntPtr playlistPtr);

        /// <summary>
        /// Decrease the reference count of a playlist.
        /// </summary>
        /// <param name="playlistPtr">The playlist object.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_playlist_release(IntPtr playlistPtr);

        /// <summary>
        /// Mark a playlist to be synchronized for offline playback
        /// </summary>
        /// <param name="sessionPtr">Session object</param>
        /// <param name="playlistPtr">Playlist object</param>
        /// <param name="offline">True if playlist should be offline, false otherwise</param>
        [DllImport("libspotify")]
        internal static extern Error sp_playlist_set_offline_mode(IntPtr sessionPtr, IntPtr playlistPtr, bool offline);

        /// <summary>
        /// Get offline status for a playlist
        /// 
        /// When in SP_PLAYLIST_OFFLINE_STATUS_DOWNLOADING mode the
        /// sp_playlist_get_offline_download_completed() method can be used to query
        /// progress of the download
        /// </summary>
        /// <param name="sessionPtr">Session object</param>
        /// <param name="playlistPtr">Playlist object</param>
        [DllImport("libspotify")]
        internal static extern PlaylistOfflineStatus sp_playlist_get_offline_status(IntPtr sessionPtr, IntPtr playlistPtr);

        /// <summary>
        /// Get download progress for an offline playlist
        /// </summary>
        /// <param name="sessionPtr">Session object</param>
        /// <param name="playlistPtr">Playlist object</param>
        /// <returns>Value from 0 - 100 that indicates amount of playlist that is downloaded</returns>
        [DllImport("libspotify")]
        internal static extern int sp_playlist_get_offline_download_completed(IntPtr sessionPtr, IntPtr playlistPtr);

        /// <summary>
        /// Return whether a playlist is loaded in RAM (as opposed to onl stored on disk)
        /// </summary>
        /// <param name="sessionPtr"> </param>
        /// <param name="playlistPtr">Playlist object.</param>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_playlist_is_in_ram(IntPtr sessionPtr, IntPtr playlistPtr);

        /// <summary>
        ///Set whether a playlist is loaded in RAM (as opposed to only stored on disk)
        /// </summary>
        /// <param name="sessionPtr"> </param>
        /// <param name="playlistPtr">Playlist object.</param>
        ///<param name="inRam"> </param>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern Error sp_playlist_set_in_ram(IntPtr sessionPtr, IntPtr playlistPtr, bool inRam);

        #endregion Internal Static Methods

        #region Nested Types

        [StructLayout(LayoutKind.Sequential)]
        public struct PlaylistCallbacks
        {
            internal IntPtr tracks_added;
            internal IntPtr tracks_removed;
            internal IntPtr tracks_moved;
            internal IntPtr playlist_renamed;
            internal IntPtr playlist_state_changed;
            internal IntPtr playlist_update_in_progress;
            internal IntPtr playlist_metadata_updated;
            internal IntPtr track_created_changed;
            internal IntPtr track_seen_changed;
            internal IntPtr description_changed;
            internal IntPtr image_changed;
            internal IntPtr track_message_changed;
            internal IntPtr subscribers_changed;
        }

        #endregion Nested Types
    }
}