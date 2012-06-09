using System;
using System.Runtime.InteropServices;

namespace Torshify.Core.Native
{
    internal partial class Spotify
    {
        public delegate void ToplistBrowseCompleteCallback(IntPtr toplistHandle, IntPtr userDataPtr);

        /// <summary>
        /// Increase the reference count of an toplist browse result
        /// </summary>
        /// <param name="browsePtr"> toplist browse object.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_toplistbrowse_add_ref(IntPtr browsePtr);

        /// <summary>
        /// Decrease the reference count of an toplist browse result
        /// </summary>
        /// <param name="browsePtr"> toplist browse object.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_toplistbrowse_release(IntPtr browsePtr);

        /// <summary>
        /// Initiate a request for browsing an toplist
        /// 
        /// The user is responsible for freeing the returned toplist browse using sp_toplistbrowse_release(). This can be done in the callback.
        /// </summary>
        /// <param name="sessionPtr">Session object</param>
        /// <param name="toplistType">Type of toplist to be browsed. see the sp_toplisttype enum for possible values</param>
        /// <param name="toplistRegion">Region. see sp_toplistregion enum. Country specific regions are coded as two chars in an integer.</param>
        /// <param name="userName">If region is SP_TOPLIST_REGION_USER this specifies which user to get toplists for. NULL means the logged in user.</param>
        /// <param name="completeCallback"> Callback to be invoked when browsing has been completed. Pass NULL if you are not interested in this event.</param>
        /// <param name="userDataPtr">Userdata passed to callback.</param>
        /// <returns>Toplist browse object</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_toplistbrowse_create(
            IntPtr sessionPtr,
            ToplistType toplistType,
            int toplistRegion,
            string userName,
            ToplistBrowseCompleteCallback completeCallback,
            IntPtr userDataPtr);

        /// <summary>
        /// Check if an toplist browse request is completed
        /// </summary>
        /// <param name="browsePtr"> toplist browse object.</param>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_toplistbrowse_is_loaded(IntPtr browsePtr);

        /// <summary>
        /// Check if browsing returned an error code.
        /// </summary>
        /// <param name="browsePtr"> toplist browse object.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_toplistbrowse_error(IntPtr browsePtr);

        /// <summary>
        /// Given an toplist browse object, return number of tracks
        /// </summary>
        /// <param name="browsePtr"> toplist browse object.</param>
        /// <returns>Number of tracks on toplist</returns>
        [DllImport("libspotify")]
        internal static extern int sp_toplistbrowse_num_tracks(IntPtr browsePtr);

        /// <summary>
        /// Given an toplist browse object, return a pointer to one of its tracks
        /// </summary>
        /// <param name="browsePtr"> toplist browse object.</param>
        /// <param name="index">The index for the track. Should be in the interval [0, sp_toplistbrowse_num_tracks() - 1]</param>
        /// <returns>A track.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_toplistbrowse_track(IntPtr browsePtr, int index);

        /// <summary>
        /// Given an toplist browse object, return number of artists
        /// </summary>
        /// <param name="browsePtr"> toplist browse object.</param>
        /// <returns>Number of artists on toplist</returns>
        [DllImport("libspotify")]
        internal static extern int sp_toplistbrowse_num_artists(IntPtr browsePtr);

        /// <summary>
        /// Given an toplist browse object, return a pointer to one of its artists
        /// </summary>
        /// <param name="browsePtr"> toplist browse object.</param>
        /// <param name="index">The index for the artists. Should be in the interval [0, sp_toplistbrowse_num_artists() - 1]</param>
        /// <returns>A track.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_toplistbrowse_artist(IntPtr browsePtr, int index);

        /// <summary>
        /// Given an toplist browse object, return number of albums
        /// </summary>
        /// <param name="browsePtr"> toplist browse object.</param>
        /// <returns>Number of albums on toplist</returns>
        [DllImport("libspotify")]
        internal static extern int sp_toplistbrowse_num_albums(IntPtr browsePtr);

        /// <summary>
        /// Given an toplist browse object, return a pointer to one of its albums
        /// </summary>
        /// <param name="browsePtr"> toplist browse object.</param>
        /// <param name="index">The index for the albums. Should be in the interval [0, sp_toplistbrowse_num_albums() - 1]</param>
        /// <returns>A track.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_toplistbrowse_album(IntPtr browsePtr, int index);

        /// <summary>
        /// Return the time (in ms) that was spent waiting for the Spotify backend to serve the request
        /// 
        /// -1 if the request was served from the local cache
        /// If the result is not yet loaded the return value is undefined
        /// </summary>
        /// <param name="browsePtr"> toplist browse object.</param>
        [DllImport("libspotify")]
        internal static extern int sp_toplistbrowse_backend_request_duration(IntPtr browsePtr);
    }
}