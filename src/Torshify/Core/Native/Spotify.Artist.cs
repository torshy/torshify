using System;
using System.Runtime.InteropServices;

namespace Torshify.Core.Native
{
    internal partial class Spotify
    {
        internal delegate void ArtistBrowseCompleteCallback(IntPtr browsePtr, IntPtr userDataPtr);

        /// <summary>
        /// Check if the artist object is populated with data.
        /// </summary>
        /// <param name="artistPtr">The artist object.</param>
        /// <returns>True if metadata is present, false if not.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_artist_is_loaded(IntPtr artistPtr);

        /// <summary>
        /// Return name of artist.
        /// </summary>
        /// <param name="artistPtr">Artist object.</param>
        /// <returns>Name of artist. Returned string is valid as long as the artist object stays allocated
        /// and no longer than the next call to <c>sp_session_process_events()</c>.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
        internal static extern string sp_artist_name(IntPtr artistPtr);

        /// <summary>
        /// Increase the reference count of an artist.
        /// </summary>
        /// <param name="artistPtr">The artist.</param>
        [DllImport("libspotify")]
        internal static extern void sp_artist_add_ref(IntPtr artistPtr);

        /// <summary>
        /// Decrease the reference count of an artist.
        /// </summary>
        /// <param name="artistPtr">The artist.</param>
        [DllImport("libspotify")]
        internal static extern void sp_artist_release(IntPtr artistPtr);

        /// <summary>
        /// Initiate a request for browsing an artist
        /// 
        /// The user is responsible for freeing the returned artist browse using sp_artistbrowse_release(). This can be done in the callback.
        /// </summary>
        /// <param name="sessionPtr">Session object</param>
        /// <param name="artistPtr">Artist to be browsed. The artist metadata does not have to be loaded</param>
        /// <param name="callback">Callback to be invoked when browsing has been completed. Pass NULL if you are not interested in this event.</param>
        /// <param name="userDataPtr">Userdata passed to callback.</param>
        /// <returns>Artist browse object</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_artistbrowse_create(IntPtr sessionPtr, IntPtr artistPtr, ArtistBrowseCompleteCallback callback, IntPtr userDataPtr);

        /// <summary>
        /// Check if an artist browse request is completed
        /// </summary>
        /// <param name="artistBrowsePtr"Artist browse object</param>
        /// <returns>True if browsing is completed, false if not</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_artistbrowse_is_loaded(IntPtr artistBrowsePtr);

        /// <summary>
        /// Check if browsing returned an error code.
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object</param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern Error sp_artistbrowse_error(IntPtr artistBrowsePtr);

        /// <summary>
        /// Given an artist browse object, return a pointer to its artist object
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object</param>
        /// <returns>Artist object</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_artistbrowse_artist(IntPtr artistBrowsePtr);

        /// <summary>
        /// Given an artist browse object, return number of portraits available
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object</param>
        /// <returns>Number of portraits for given artist</returns>
        [DllImport("libspotify")]
        internal static extern int sp_artistbrowse_num_portraits(IntPtr artistBrowsePtr);

        /// <summary>
        /// Return image ID representing a portrait of the artist
        /// </summary>
        /// <param name="browsePtr">Artist object</param>
        /// <param name="index">The index of the portrait. Should be in the interval [0, sp_artistbrowse_num_portraits() - 1]</param>
        /// <returns>ID byte sequence that can be passed to sp_image_create()</returns>
        [DllImport("libspotify")]
        internal static extern byte[] sp_artistbrowse_portrait(IntPtr browsePtr, int index);

        /// <summary>
        /// Given an artist browse object, return number of tracks
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object</param>
        /// <returns>Number of tracks for given artist</returns>
        [DllImport("libspotify")]
        internal static extern int sp_artistbrowse_num_tracks(IntPtr artistBrowsePtr);

        /// <summary>
        /// Given an artist browse object, return one of its tracks
        /// </summary>
        /// <param name="browsePtr">Album browse object</param>
        /// <param name="index">The index for the track. Should be in the interval [0, sp_artistbrowse_num_tracks() - 1]</param>
        /// <returns>A track object, or NULL if the index is out of range.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_artistbrowse_track(IntPtr browsePtr, int index);

        /// <summary>
        /// Given an artist browse object, return number of albums
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object</param>
        /// <returns>Number of albums for given artist</returns>
        [DllImport("libspotify")]
        internal static extern int sp_artistbrowse_num_albums(IntPtr artistBrowsePtr);

        /// <summary>
        /// Given an artist browse object, return one of its albums
        /// </summary>
        /// <param name="browsePtr">Album browse object</param>
        /// <param name="index">The index for the album. Should be in the interval [0, sp_artistbrowse_num_albums() - 1]</param>
        /// <returns>A track object, or NULL if the index is out of range.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_artistbrowse_album(IntPtr browsePtr, int index);

        /// <summary>
        /// Given an artist browse object, return number of similar artists
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object</param>
        /// <returns>Number of similar artists for given artist</returns>
        [DllImport("libspotify")]
        internal static extern int sp_artistbrowse_num_similar_artists(IntPtr artistBrowsePtr);

        /// <summary>
        /// Given an artist browse object, return a similar artist by index
        /// </summary>
        /// <param name="browsePtr">Album browse object</param>
        /// <param name="index"> The index for the artist. Should be in the interval [0, sp_artistbrowse_num_similar_artists() - 1]</param>
        /// <returns>A pointer to an artist object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_artistbrowse_similar_artist(IntPtr browsePtr, int index);

        /// <summary>
        /// Given an artist browse object, return the artists biography
        /// 
        /// This function must be called from the same thread that did sp_session_create()
        /// </summary>
        /// <param name="browsePtr">Artist browse object</param>
        /// <returns>Biography string in UTF-8 format. Returned string is valid as long as the album object stays allocated and no longer than the next call to sp_session_process_events()</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
        internal static extern string sp_artistbrowse_biography(IntPtr browsePtr);

        /// <summary>
        /// Increase the reference count of an artist browse result
        /// </summary>
        /// <param name="browsePtr"> Album artist object.</param>
        [DllImport("libspotify")]
        internal static extern void sp_artistbrowse_add_ref(IntPtr browsePtr);

        /// <summary>
        /// Decrease the reference count of an artist browse result
        /// </summary>
        /// <param name="browsePtr"> artist browse object.</param>
        [DllImport("libspotify")]
        internal static extern void sp_artistbrowse_release(IntPtr browsePtr);
    }
}