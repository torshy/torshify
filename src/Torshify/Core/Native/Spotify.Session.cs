using System;
using System.Runtime.InteropServices;

namespace Torshify.Core.Native
{
    internal partial class Spotify
    {
        /// <summary>
        /// Initialize a session. The session returned will be initialized, but you will need to log in before you can perform any other operation.
        /// </summary>
        /// <param name="config">The configuration to use for the session.</param>
        /// <param name="sessionPtr">If successful, a new session - otherwise null.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        public static extern Error sp_session_create(ref SpotifySessionConfig config, out IntPtr sessionPtr);

        /// <summary>
        /// Release the session. This will clean up all data and connections associated with the session.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        [DllImport("libspotify")]
        internal static extern void sp_session_release(IntPtr sessionPtr);

        /// <summary>
        /// Logs in the specified username/password combo. This initiates the download in the background. A callback is called when login is complete.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="username">The username to log in.</param>
        /// <param name="password">The password for the specified username.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_session_login(IntPtr sessionPtr, string username, string password);

        /// <summary>
        /// Logs out the currently logged in user.
        /// </summary>
        /// <remarks>
        /// Always call this before terminating the application and libspotify is currently logged in. Otherwise, the settings and cache may be lost.
        /// </remarks>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_session_logout(IntPtr sessionPtr);

        /// <summary>
        /// Gets the connection state of the specified session.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>The connection state.</returns>
        [DllImport("libspotify")]
        internal static extern ConnectionState sp_session_connectionstate(IntPtr sessionPtr);

        /// <summary>
        /// Set maximum cache size.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="size">Maximum cache size in megabytes. Setting it to 0 (the default) will let libspotify automatically resize the cache (10% of disk free space).</param>
        [DllImport("libspotify")]
        internal static extern void sp_session_set_cache_size(IntPtr sessionPtr, uint size);

        /// <summary>
        /// Make the specified session process any pending events.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="next_timeout">Stores the time (in milliseconds) until you should call this function again.</param>
        [DllImport("libspotify")]
        internal static extern void sp_session_process_events(IntPtr sessionPtr, out int next_timeout);

        /// <summary>
        /// Loads the specified track.
        /// After successfully loading the track, you have the option of running sp_session_player_play() directly,
        /// or using sp_session_player_seek() first. When this call returns, the track will have been loaded,
        /// unless an error occurred.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="track">Track object from playlist or search.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_session_player_load(IntPtr sessionPtr, IntPtr track);

        /// <summary>
        /// Seek to position in the currently loaded track.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="offset">Track position, in milliseconds.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern void sp_session_player_seek(IntPtr sessionPtr, int offset);

        /// <summary>
        /// PlayerPlay or pause the currently loaded track.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="play">If set to true, playback will occur. If set to false, the playback will be paused.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern void sp_session_player_play(IntPtr sessionPtr, bool play);

        /// <summary>
        /// Stops the currently playing track.
        /// This frees some resources held by libspotify to identify the currently playing track.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_session_player_unload(IntPtr sessionPtr);

        /// <summary>
        /// Prefetch a track.
        /// Instruct libspotify to start loading of a track into its cache. This could be done by an application just before the current track ends.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="track">The track to be prefetched.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_session_player_prefetch(IntPtr sessionPtr, IntPtr track);

        /// <summary>
        /// Returns the playlist container for the currently logged in user.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>Playlist container object, NULL if not logged in.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_playlistcontainer(IntPtr sessionPtr);

        /// <summary>
        /// Returns an inbox playlist for the currently logged in user.
        /// <seealso cref="libspotify.sp_playlist_release"/>
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>A playlist.</returns>
        /// <remarks>You need to release the playlist when you are done with it.</remarks>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_inbox_create(IntPtr sessionPtr);

        /// <summary>
        /// Returns the starred list for the current user.
        /// <seealso cref="libspotify.sp_playlist_release"/>
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>A playlist.</returns>
        /// <remarks>You need to release the playlist when you are done with it.</remarks>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_starred_create(IntPtr sessionPtr);

        /// <summary>
        /// Returns the starred list for a user.
        /// <seealso cref="libspotify.sp_playlist_release"/>
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="username">Canonical username.</param>
        /// <returns>A playlist.</returns>
        /// <remarks>You need to release the playlist when you are done with it.</remarks>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_starred_for_user_create(IntPtr sessionPtr, string username);

        /// <summary>
        /// Return the published container for a given canonical_username, or the currently logged in user if canonical_username is null.
        /// The container can be released when you're done with it, using <c>sp_session_publishedcontainer_fo_user_release()</c>, or it will be released when calling <c>sp_session_logout()</c>.
        /// Subsequent requests for a published container will return the same object, unless it has been released previously.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="username">Canonical username.</param>
        /// <returns>Playlist container object, null if not logged in or not found. </returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_publishedcontainer_for_user(IntPtr sessionPtr, string username);

        /// <summary>
        /// Releases the playlistcontainer for canonical_username.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="username">Canonical username.</param>
        [DllImport("libspotify")]
        internal static extern void sp_session_publishedcontainer_for_user_release(IntPtr sessionPtr, string username);

        /// <summary>
        /// Set preferred bitrate for music streaming.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="bitrate">Preferred bitrate.</param>
        [DllImport("libspotify")]
        internal static extern void sp_session_preferred_bitrate(IntPtr sessionPtr, Bitrate bitrate);

        /// <summary>
        /// Return number of friends in the currently logged in users friends list.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>Number of users in friends. Each user can be extracted using the <c>sp_session_friend()</c>
        /// method. The number of users in the list will not be updated nor change order between calls to <c>sp_session_process_events()</c></returns>
        [DllImport("libspotify")]
        internal static extern int sp_session_num_friends(IntPtr sessionPtr);

        /// <summary>
        /// Retrun the given user from the currently logged in users list of friends.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="index">Index in the list.</param>
        /// <returns>A user. The object is owned by the session so the caller should not release it.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_friend(IntPtr sessionPtr, int index);

        /// <summary>
        /// Set preferred bitrate for offline sync
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="bitrate">Preferred bitrate, see ::sp_bitrate for possible values.</param>
        /// <param name="allowResync">Set to true if libspotify should resynchronize already synchronized tracks. Usually you should set this to false.</param>
        [DllImport("libspotify")]
        internal static extern void sp_session_preferred_offline_bitrate(IntPtr sessionPtr, Bitrate bitrate, bool allowResync);

        /// <summary>
        /// Set to true if the connection is currently routed over a roamed connectivity
        /// 
        /// Used in conjunction with sp_session_set_connection_rules() to control
        /// how libspotify should behave in respect to network activity and offline synchronization.
        /// </summary>
        /// <param name="sessionPtr">Session object</param>
        /// <param name="connectionType">Connection type</param>
        [DllImport("libspotify")]
        internal static extern void sp_session_set_connection_type(IntPtr sessionPtr, ConnectionType connectionType);

        /// <summary>
        /// Set rules for how libspotify connects to Spotify servers and synchronizes offline content.
        /// 
        /// Used in conjunction with sp_session_set_connection_type() to control
        /// how libspotify should behave in respect to network activity and offline synchronization.
        /// </summary>
        /// <param name="sessionPtr"></param>
        /// <param name="connectionRule"></param>
        [DllImport("libspotify")]
        internal static extern void sp_session_set_connection_rules(IntPtr sessionPtr, ConnectionRule connectionRule);

        /// <summary>
        /// Get total number of tracks that needs download before everything
        /// from all playlists that is marked for offline is fully synchronized
        /// </summary>
        /// <param name="sessionPtr">Session object</param>
        /// <returns>Number of tracks</returns>
        [DllImport("libspotify")]
        internal static extern int sp_offline_tracks_to_sync(IntPtr sessionPtr);

        /// <summary>
        /// Return number of playlisys that is marked for offline synchronization
        /// 
        /// 
        /// </summary>
        /// <param name="sessionPtr">Session object</param>
        /// <returns>Number of playlists</returns>
        [DllImport("libspotify")]
        internal static extern int sp_offline_num_playlists(IntPtr sessionPtr);

        /// <summary>
        /// Return offline synchronization status. When the internal status is
        /// updated the offline_status_updated() callback will be invoked.
        /// </summary>
        /// <param name="sessionPtr">Session object</param>
        /// <param name="connectionRule">Status object that will be filled with info</param>
        [DllImport("libspotify")]
        internal static extern void sp_offline_sync_get_status(IntPtr sessionPtr, out PlaylistOfflineStatus connectionRule);

        /// <summary>
        /// Get currently logged in users country
        /// updated the offline_status_updated() callback will be invoked.
        /// </summary>
        /// <param name="sessionPtr">Session object</param>
        /// <returns>Country encoded in an integer 'SE' = 'S' << 8 | 'E'</returns>
        [DllImport("libspotify")]
        internal static extern int sp_session_user_country(IntPtr sessionPtr);
    }
}