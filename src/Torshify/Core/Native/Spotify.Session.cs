using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Torshify.Core.Native
{
    internal partial class Spotify
    {
        /// <summary>
        /// Return the libspotify build ID
        /// This might be useful to have available for display somewhere in your user interface.
        /// </summary>
        /// <returns></returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
        internal static extern string sp_build_id();

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
        internal static extern Error sp_session_release(IntPtr sessionPtr);

        /// <summary>
        /// Logs in the specified username/password combo. This initiates the download in the background. A callback is called when login is complete.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="username">The username to log in.</param>
        /// <param name="password">The password for the specified username.</param>
        /// <param name="rememberMe">If set, the username / password will be remembered by libspotify</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_session_login(IntPtr sessionPtr, string username, string password, [MarshalAs(UnmanagedType.I1)]bool rememberMe, string blob);

        /// <summary>
        /// Log in the remembered user if last user that logged in logged in with remember_me set.
        /// If no credentials are stored, this will return SP_ERROR_NO_CREDENTIALS.
        /// </summary>
        /// <param name="sessionPtr">Your session object</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_session_relogin(IntPtr sessionPtr);

        /// <summary>
        /// Get username of the user that will be logged in via sp_session_relogin()
        /// </summary>
        /// <param name="linkPtr">Your session object</param>
        /// <param name="bufferPtr">The buffer to hold the username</param>
        /// <param name="buffer_size">The max size of the buffer that will hold the username. The resulting string is guaranteed to always be null terminated if buffer_size > 0</param>
        /// <returns> The number of characters in the username. If value is greater or equal than \p buffer_size, output was truncated. If returned value is -1 no credentials are stored in libspotify.*/</returns>
        [DllImport("libspotify")]
        internal static extern int sp_session_remembered_user(
            IntPtr sessionPtr, 
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringBuilderMarshaler))]StringBuilder buffer,
            int bufferSize);

        /// <summary>
        /// Remove stored credentials in libspotify. If no credentials are currently stored, nothing will happen.
        /// </summary>
        /// <param name="sessionPtr">Your session object</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_session_forget_me(IntPtr sessionPtr);

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
        internal static extern Error sp_session_set_cache_size(IntPtr sessionPtr, uint size);

        /// <summary>
        /// Make the specified session process any pending events.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="nextTimeout">Stores the time (in milliseconds) until you should call this function again.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_session_process_events(IntPtr sessionPtr, out int nextTimeout);

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
        internal static extern Error sp_session_player_seek(IntPtr sessionPtr, int offset);

        /// <summary>
        /// PlayerPlay or pause the currently loaded track.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="play">If set to true, playback will occur. If set to false, the playback will be paused.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_session_player_play(IntPtr sessionPtr, bool play);

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
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>A playlist.</returns>
        /// <remarks>You need to release the playlist when you are done with it.</remarks>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_inbox_create(IntPtr sessionPtr);

        /// <summary>
        /// Returns the starred list for the current user.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>A playlist.</returns>
        /// <remarks>You need to release the playlist when you are done with it.</remarks>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_starred_create(IntPtr sessionPtr);

        /// <summary>
        /// Returns the starred list for a user.
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
        internal static extern IntPtr sp_session_publishedcontainer_for_user_create(IntPtr sessionPtr, string username);

        /// <summary>
        /// Set preferred bitrate for music streaming.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="bitrate">Preferred bitrate.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_session_preferred_bitrate(IntPtr sessionPtr, Bitrate bitrate);

        /// <summary>
        /// Set preferred bitrate for offline sync
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="bitrate">Preferred bitrate, see ::sp_bitrate for possible values.</param>
        /// <param name="allowResync">Set to true if libspotify should resynchronize already synchronized tracks. Usually you should set this to false.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_session_preferred_offline_bitrate(IntPtr sessionPtr, Bitrate bitrate, bool allowResync);

        /// <summary>
        /// Set to true if the connection is currently routed over a roamed connectivity
        /// 
        /// Used in conjunction with sp_session_set_connection_rules() to control
        /// how libspotify should behave in respect to network activity and offline synchronization.
        /// </summary>
        /// <param name="sessionPtr">Session object</param>
        /// <param name="connectionType">Connection type</param>
        [DllImport("libspotify")]
        internal static extern Error sp_session_set_connection_type(IntPtr sessionPtr, ConnectionType connectionType);

        /// <summary>
        /// Set rules for how libspotify connects to Spotify servers and synchronizes offline content.
        /// 
        /// Used in conjunction with sp_session_set_connection_type() to control
        /// how libspotify should behave in respect to network activity and offline synchronization.
        /// </summary>
        /// <param name="sessionPtr"></param>
        /// <param name="connectionRule"></param>
        [DllImport("libspotify")]
        internal static extern Error sp_session_set_connection_rules(IntPtr sessionPtr, ConnectionRule connectionRule);

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
        /// <returns>false if no synching is in progress (in which case the contents of status is undefined)</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_offline_sync_get_status(IntPtr sessionPtr, ref SpotifyOfflineSyncStatus offlineStatus);

        /// <summary>
        /// Return remaining time (in seconds) until the offline key store expires and the user is required to relogin
        /// </summary>
        /// <param name="sessionPtr"></param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern int sp_offline_time_left(IntPtr sessionPtr);

        /// <summary>
        /// Fetches the currently logged in user.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>The logged in user (or null if not logged in).</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_user(IntPtr sessionPtr);

        /// <summary>
        /// The userdata associated with the session.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>The userdata that was passed in on session creation.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_user_data(IntPtr sessionPtr);

        /// <summary>
        /// Get a string representing the user's login username
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
        internal static extern string sp_session_user_name(IntPtr user);

        /// <summary>
        /// Get currently logged in users country
        /// updated the offline_status_updated() callback will be invoked.
        /// </summary>
        /// <param name="sessionPtr">Session object</param>
        /// <returns>Country encoded in an integer 'SE' = 'S' << 8 | 'E'</returns>
        [DllImport("libspotify")]
        internal static extern int sp_session_user_country(IntPtr sessionPtr);

        /// <summary>
        /// Return status of volume normalization
        /// </summary>
        /// <param name="sessionPtr">Session object</param>
        /// <returns>true if volume normalization is enabled</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_session_get_volume_normalization(IntPtr sessionPtr);

        /// <summary>
        /// Set volume normalization
        /// </summary>
        /// <param name="sessionPtr">Session object</param>
        /// <param name="on">True if volume normalization should be enabled</param>
        [DllImport("libspotify")]
        internal static extern Error sp_session_set_volume_normalization(IntPtr sessionPtr, bool on);

        /// <summary>
        /// This will make libspotify write all data that is meant to be stored
        /// on disk to the disk immediately. libspotify does this periodically
        /// by itself and also on logout. So under normal conditions this
        /// should never need to be used.
        /// </summary>
        /// <param name="sessionPtr"></param>
        [DllImport("libspotify")]
        internal static extern Error sp_session_flush_caches(IntPtr sessionPtr);

        /// <summary>
        /// Set if private session is enabled. This disables sharing what the user is listening to
        /// to services such as Spotify Social, Facebook and LastFM. The private session will
        /// last for a time, and then libspotify will revert to the normal state. The private
        /// session is prolonged by user activity.
        /// </summary>
        /// <param name="sessionPtr"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern Error sp_session_set_private_session(IntPtr sessionPtr,  bool enabled);

        /// <summary>
        /// Return True if private session is enabled
        /// </summary>
        /// <param name="sessionPtr"></param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern bool sp_session_is_private_session(IntPtr sessionPtr);

        /// <summary>
        /// Set if scrobbling is enabled. This api allows setting local overrides of the global scrobbling settings.
        /// Changing the global settings are currently not supported.
        /// </summary>
        /// <param name="sessionPtr"></param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern Error sp_session_set_scrobbling(IntPtr sessionPtr, SocialProvider socialProvider, ScrobblingState scrobblingState);

        /// <summary>
        /// Return the scrobbling state. This makes it possible to find out if scrobbling is locally overrided or
        /// if the global setting is used
        /// </summary>
        /// <param name="sessionPtr"></param>
        /// <param name="socialProvider"></param>
        /// <param name="scrobblingState"></param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern bool sp_session_is_scrobbling(IntPtr sessionPtr, SocialProvider socialProvider, ScrobblingState scrobblingState);

        /// <summary>
        /// Return True if scrobbling settings should be shown to the user. Currently this setting is relevant
        /// only to the facebook provider.
        /// The returned value may be false if the user is not connected to facebook,
        /// or if the user has opted out from facebook social graph.
        /// </summary>
        /// <param name="sessionPtr"></param>
        /// <param name="socialProvider"></param>
        /// <param name="isPossible"></param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern Error sp_session_is_scrobbling_possible(IntPtr sessionPtr, SocialProvider socialProvider, out bool isPossible);

        /// <summary>
        /// Set the user's credentials with a social provider.
        /// Currently this is only relevant for LastFm
        /// Call sp_session_set_scrobbling to force an authentication attempt
        /// with the LastFm server. If authentication fails a scrobble_error callback will be sent.
        /// </summary>
        /// <param name="sessionPtr"></param>
        /// <param name="socialProvider"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern Error sp_session_set_social_credentials(IntPtr sessionPtr, SocialProvider socialProvider, string userName, string password);
    }
}