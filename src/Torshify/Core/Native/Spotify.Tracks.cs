using System;
using System.Runtime.InteropServices;

namespace Torshify.Core.Native
{
    internal partial class Spotify
    {
        /// <summary>
        /// Get load status for the specified track. If the track is not loaded yet, all other functions operating on the track return default values.
        /// </summary>
        /// <param name="albumPtr">The track whose load status you are interested in.</param>
        /// <returns>True if track is loaded, otherwise false.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_track_is_loaded(IntPtr trackPtr);

        /// <summary>
        /// Return an error code associated with a track. For example if it could not load.
        /// </summary>
        /// <param name="trackPtr"></param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_track_error(IntPtr trackPtr);

        /// <summary>
        /// Return availability for a track
        /// </summary>
        /// <param name="sessionPtr"></param>
        /// <param name="trackPtr"></param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern TrackAvailablity sp_track_get_availability(IntPtr sessionPtr, IntPtr trackPtr);

        /// <summary>
        /// Return offline status for a track. sp_session_callbacks::metadata_updated() will be invoked when
        /// offline status of a track changes
        /// </summary>
        /// <param name="trackPtr"></param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern TrackOfflineStatus sp_track_offline_get_status(IntPtr trackPtr);

        /// <summary>
        /// Return true if the track is a placeholder. Placeholder tracks are used
        /// to store other objects than tracks in the playlist. Currently this is
        ///  used in the inbox to store artists, albums and playlists.
        /// 
        /// Use sp_link_create_from_track() to get a link object that points
        /// to the real object this "track" points to.
        /// 
        /// Contrary to most functions the track does not have to be
        /// loaded for this function to return correct value
        /// </summary>
        /// <param name="trackPtr"></param>
        /// <returns>True if track is a placeholder</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_track_is_placeholder(IntPtr trackPtr);

        /// <summary>
        /// Return true if the track is a local file.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="trackPtr">The track.</param>
        /// <remarks>The track must be loaded or this function will always return false.
        /// <seealso cref="Spotify.sp_track_is_loaded"/>
        /// </remarks>
        /// <returns>True if track is a local file, otherwise false.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_track_is_local(IntPtr sessionPtr, IntPtr trackPtr);

        /// <summary>
        /// Return true if the track is autolinked to another track.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="albumPtr">The track.</param>
        /// <remarks>The track must be loaded or this function will always return false.
        /// </remarks>
        /// <returns>True if track is autolinked, otherwise false.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_track_is_autolinked(IntPtr sessionPtr, IntPtr trackPtr);

        /// <summary>
        /// Return true if the track is starred by the currently logged in user.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="albumPtr">The track.</param>
        /// <remarks>The track must be loaded or this function will always return false.
        /// </remarks>
        /// <returns>True if track is a starred file, otherwise false.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_track_is_starred(IntPtr sessionPtr, IntPtr trackPtr);

        /// <summary>
        /// Star/Unstar the specified tracks.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="trackArrayPtr">An array of pointer to tracks.</param>
        /// <param name="num_tracks">Count of <c>trackArray</c>.</param>
        /// <param name="star">Starred status of the track.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_track_set_starred(IntPtr sessionPtr, IntPtr trackArrayPtr, int num_tracks, bool star);

        /// <summary>
        /// The number of artists performing on the specified track.
        /// </summary>
        /// <param name="albumPtr">The track whose number of participating artists you are interested in.</param>
        /// <returns>The number of artists performing on the specified track. If no metadata is available for the track yet, this function returns 0.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_track_num_artists(IntPtr trackPtr);

        /// <summary>
        /// The artist matching the specified index performing on the current track.
        /// </summary>
        /// <param name="albumPtr">The track whose participating artist you are interested in.</param>
        /// <param name="index">The index for the participating artist. Should be in the interval [0, <c>sp_track_num_artists()</c> - 1]</param>
        /// <returns>The participating artist, or NULL if invalid index.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_track_artist(IntPtr trackPtr, int index);

        /// <summary>
        /// The album of the specified track.
        /// </summary>
        /// <param name="albumPtr">A track object.</param>
        /// <returns>The album of the given track. You need to increase the refcount if you want to keep the pointer around. If no metadata is available for the track yet, this function returns 0.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_track_album(IntPtr trackPtr);

        /// <summary>
        /// The string representation of the specified track's name.
        /// </summary>
        /// <param name="albumPtr">A track object.</param>
        /// <returns>The string representation of the specified track's name.
        /// Returned string is valid as long as the album object stays allocated and
        /// no longer than the next call to <c>sp_session_process_events()</c>.
        /// If no metadata is available for the track yet, this function returns empty string. </returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
        internal static extern string sp_track_name(IntPtr trackPtr);

        /// <summary>
        /// The duration, in milliseconds, of the specified track.
        /// </summary>
        /// <param name="albumPtr">A track object.</param>
        /// <returns>The duration of the specified track, in milliseconds If no metadata is available for the track yet, this function returns 0.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_track_duration(IntPtr trackPtr);

        /// <summary>
        /// Returns popularity for track.
        /// </summary>
        /// <param name="albumPtr">A track object.</param>
        /// <returns>Popularity in range 0 to 100, 0 if undefined.
        /// If no metadata is available for the track yet, this function returns 0.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_track_popularity(IntPtr trackPtr);

        /// <summary>
        /// Returns the disc number for a track.
        /// </summary>
        /// <param name="albumPtr">A track object.</param>
        /// <returns>Disc index. Possible values are [1, total number of discs on album].
        /// This function returns valid data only for tracks appearing in a browse artist or browse album result (otherwise returns 0).</returns>
        [DllImport("libspotify")]
        internal static extern int sp_track_disc(IntPtr trackPtr);

        /// <summary>
        /// Returns the position of a track on its disc.
        /// </summary>
        /// <param name="albumPtr">A track object.</param>
        /// <returns>Track position, starts at 1 (relative the corresponding disc).
        /// This function returns valid data only for tracks appearing in a browse artist or browse album result (otherwise returns 0).</returns>
        [DllImport("libspotify")]
        internal static extern int sp_track_index(IntPtr trackPtr);

        /// <summary>
        /// Returns the newly created local track.
        /// </summary>
        /// <param name="artist">Name of the artist.</param>
        /// <param name="title">Song title.</param>
        /// <param name="album">Name of the album, or an empty string if not available.</param>
        /// <param name="length">Count in MS, or -1 if not available.</param>
        /// <returns>A track.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_localtrack_create(string artist, string title, string album, int length);

        /// <summary>
        /// Increase the reference count of a track.
        /// </summary>
        /// <param name="albumPtr">The track object.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_track_add_ref(IntPtr trackPtr);

        /// <summary>
        /// Decrease the reference count of a track.
        /// </summary>
        /// <param name="albumPtr">The track object.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_track_release(IntPtr trackPtr);

        /// <summary>
        /// Return the actual track that will be played if the given track is played
        /// </summary>
        /// <returns>A track.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_track_get_playable(IntPtr sessionPtr, IntPtr trackPtr);
    }
}