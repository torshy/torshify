using System;
using System.Runtime.InteropServices;

namespace Torshify.Core.Native
{
    internal partial class Spotify
    {
        public delegate void AlbumBrowseCompleteCallback(IntPtr albumBrowsePtr, IntPtr userDataPtr);

        /// <summary>
        /// Check if the album object is populated with data.
        /// </summary>
        /// <param name="albumPtr">The album object.</param>
        /// <returns>True if metadata is present, false if not.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_album_is_loaded(IntPtr albumPtr);

        /// <summary>
        /// Return true if the album is available in the current region.
        /// </summary>
        /// <param name="albumPtr">The album.</param>
        /// <returns>True if album is available for playback, otherwise false.</returns>
        /// <remarks>The album must be loaded or this function will always return false.
        /// <seealso cref="sp_album_is_loaded"/>
        /// </remarks>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_album_is_available(IntPtr albumPtr);

        /// <summary>
        /// Get the artist associated with the given album.
        /// </summary>
        /// <param name="albumPtr">Album object.</param>
        /// <returns>A reference to the artist. null if the metadata has not been loaded yet.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_album_artist(IntPtr albumPtr);

        /// <summary>
        /// Return image ID representing the album's coverart.
        /// </summary>
        /// <param name="albumPtr">Album object.</param>
        /// <returns>ID byte sequence that can be passed to <c>sp_image_create()</c>.
        /// If the album has no image or the metadata for the album is not loaded yet, this function returns null.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_album_cover(IntPtr albumPtr, ImageSize imageSize);

        /// <summary>
        /// Return name of album.
        /// </summary>
        /// <param name="albumPtr">Album object.</param>
        /// <returns>Name of album. Returned string is valid as long as the album object stays allocated
        /// and no longer than the next call to <c>sp_session_process_events()</c>.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
        internal static extern string sp_album_name(IntPtr albumPtr);

        /// <summary>
        /// Return release year of specified album.
        /// </summary>
        /// <param name="albumPtr">The album.</param>
        /// <returns>Release year.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_album_year(IntPtr albumPtr);

        /// <summary>
        /// Return type of specified album.
        /// </summary>
        /// <param name="albumPtr">Album object.</param>
        /// <returns>The album type.</returns>
        [DllImport("libspotify")]
        internal static extern AlbumType sp_album_type(IntPtr albumPtr);

        /// <summary>
        /// Increase the reference count of an album.
        /// </summary>
        /// <param name="albumPtr">The album.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_album_add_ref(IntPtr albumPtr);

        /// <summary>
        /// Decrease the reference count of an album.
        /// </summary>
        /// <param name="albumPtr">The album.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_album_release(IntPtr albumPtr);

        /// <summary>
        /// Initiate a request for browsing an album
        /// 
        /// The user is responsible for freeing the returned album browse using sp_albumbrowse_release(). This can be done in the callback.
        /// </summary>
        /// <param name="sessionPtr">Session object</param>
        /// <param name="albumPtr">Album to be browsed. The album metadata does not have to be loaded</param>
        /// <param name="callback">Callback to be invoked when browsing has been completed. Pass NULL if you are not interested in this event.</param>
        /// <param name="userdataPtr">Userdata passed to callback.</param>
        /// <returns>Album browse object</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_albumbrowse_create(IntPtr sessionPtr, IntPtr albumPtr, AlbumBrowseCompleteCallback callback, IntPtr userdataPtr);

        /// <summary>
        /// Check if an album browse request is completed
        /// </summary>
        /// <param name="browsePtr"> Album browse object.</param>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_albumbrowse_is_loaded(IntPtr browsePtr);

        /// <summary>
        /// Check if browsing returned an error code.
        /// </summary>
        /// <param name="browsePtr"> Album browse object.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_albumbrowse_error(IntPtr browsePtr);

        /// <summary>
        /// Given an album browse object, return the pointer to its album object
        /// </summary>
        /// <param name="browsePtr"> Album browse object.</param>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_albumbrowse_album(IntPtr browsePtr);

        /// <summary>
        ///  Given an album browse object, return the pointer to its artist object
        /// </summary>
        /// <param name="browsePtr"> Album browse object.</param>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_albumbrowse_artist(IntPtr browsePtr);

        /// <summary>
        /// Given an album browse object, return number of copyright strings
        /// </summary>
        /// <param name="browsePtr"> Album browse object.</param>
        [DllImport("libspotify")]
        internal static extern int sp_albumbrowse_num_copyrights(IntPtr browsePtr);

        /// <summary>
        /// Given an album browse object, return one of its copyright strings
        /// </summary>
        /// <param name="browsePtr"> Album browse object.</param>
        /// <param name="index">The index for the copyright string. Should be in the interval [0, sp_albumbrowse_num_copyrights() - 1]</param>
        /// <returns>Copyright string in UTF-8 format, or NULL if the index is invalid.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
        internal static extern string sp_albumbrowse_copyright(IntPtr browsePtr, int index);

        /// <summary>
        /// Given an album browse object, return number of tracks
        /// </summary>
        /// <param name="browsePtr"> Album browse object.</param>
        /// <returns>Number of tracks on album</returns>
        [DllImport("libspotify")]
        internal static extern int sp_albumbrowse_num_tracks(IntPtr browsePtr);

        /// <summary>
        /// Given an album browse object, return a pointer to one of its tracks
        /// </summary>
        /// <param name="browsePtr"> Album browse object.</param>
        /// <param name="index">The index for the track. Should be in the interval [0, sp_albumbrowse_num_tracks() - 1]</param>
        /// <returns>A track.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_albumbrowse_track(IntPtr browsePtr, int index);

        /// <summary>
        /// Given an album browse object, return its review
        /// </summary>
        /// <param name="browsePtr"> Album browse object.</param>
        /// <returns>Review string in UTF-8 format.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
        internal static extern string sp_albumbrowse_review(IntPtr browsePtr);

        /// <summary>
        /// Increase the reference count of an album browse result
        /// </summary>
        /// <param name="browsePtr"> Album browse object.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_albumbrowse_add_ref(IntPtr browsePtr);

        /// <summary>
        /// Decrease the reference count of an album browse result
        /// </summary>
        /// <param name="browsePtr"> Album browse object.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_albumbrowse_release(IntPtr browsePtr);

        /// <summary>
        /// Return the time (in ms) that was spent waiting for the Spotify backend to serve the request
        /// 
        /// -1 if the request was served from the local cache
        /// If the result is not yet loaded the return value is undefined
        /// </summary>
        /// <param name="browsePtr"> album browse object.</param>
        [DllImport("libspotify")]
        internal static extern int sp_albumbrowse_backend_request_duration(IntPtr browsePtr);
    }
}