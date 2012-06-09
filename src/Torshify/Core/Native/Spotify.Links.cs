using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Torshify.Core.Native
{
    internal partial class Spotify
    {
        /// <summary>
        /// Create a Spotify link given a string
        /// </summary>
        /// <remarks>You need to release the link when you are done with it.</remarks>
        /// <param name="link">A string representation of a Spotify link</param>
        /// <returns>A link representation of the given string representation. If the link could not be parsed, this function returns NULL.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_string(string link);
        /// <summary>
        /// Creates a link object from an artist.
        /// </summary>
        /// <remarks>You need to release the link when you are done with it.</remarks>
        /// <param name="artistPtr">The artist.</param>
        /// <returns>A link object representing the artist.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_artist(IntPtr artistPtr);

        /// <summary>
        /// Creates a link object from a track.
        /// </summary>
        /// <remarks>You need to release the link when you are done with it.</remarks>
        /// <param name="trackPtr">The track.</param>
        /// <param name="offset">The offset</param>
        /// <returns>A link object representing the track.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_track(IntPtr trackPtr, int offset);

        /// <summary>
        /// Creates a link object from an album.
        /// </summary>
        /// <remarks>You need to release the link when you are done with it.</remarks>
        /// <param name="albumPtr">The album.</param>
        /// <returns>A link object representing the track.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_album(IntPtr albumPtr);

        /// <summary>
        /// Creates a link object from a search.
        /// </summary>
        /// <remarks>You need to release the link when you are done with it.</remarks>
        /// <param name="searchPtr">The search.</param>
        /// <returns>A link object representing the search.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_search(IntPtr searchPtr);

        /// <summary>
        /// Creates a link object from a playlist.
        /// </summary>
        /// <remarks>You need to release the link when you are done with it.</remarks>
        /// <param name="playlistPtr">The playlist.</param>
        /// <returns>A link object representing the playlist.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_playlist(IntPtr playlistPtr);

        /// <summary>
        /// Creates a link object from a user.
        /// </summary>
        /// <remarks>You need to release the link when you are done with it.</remarks>
        /// <param name="userPtr">The user.</param>
        /// <returns>A link object representing the user.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_user(IntPtr userPtr);

        /// <summary>
        /// Create an image link object from an album
        /// </summary>
        /// <param name="albumPtr">An album object</param>
        /// <returns>A link representing the album cover. Type is set to SP_LINKTYPE_IMAGE</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_album_cover(IntPtr albumPtr, ImageSize imageSize);

        /// <summary>
        /// Creates a link object from an artist portrait
        /// 
        /// You need to release the link when you are done with it.
        /// @see sp_link_release()
        /// @sp_artistbrowse_num_portraits()
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object</param>
        /// <param name="index">The index of the portrait. Should be in the interval [0, sp_artistbrowse_num_portraits() - 1]</param>
        /// <returns> A link object representing an image</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_artistbrowse_portrait(IntPtr artistBrowsePtr, int index);

        /// <summary>
        /// Creates a link object pointing to an artist portrait
        /// </summary>
        /// <param name="artistPtr">Artist browse object</param>
        /// <returns>A link object representing an image</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_artist_portrait(IntPtr artistPtr, ImageSize imageSize);

        /// <summary>
        /// Create a link object representing the given image
        /// 
        /// A link representing the image.
        /// @note You need to release the link when you are done with it.
        /// @see sp_link_release()
        /// </summary>
        /// <param name="imagePtr">Image object</param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_image(IntPtr imagePtr);

        /// <summary>
        /// Create a string representation of the given Spotify link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose string representation you are interested in.</param>
        /// <param name="bufferPtr">The buffer to hold the string representation of link.</param>
        /// <param name="buffer_size">The max size of the buffer that will hold the string representation.
        /// The resulting string is guaranteed to always be null terminated if buffer_size &gt; 0.</param>
        /// <returns>The number of characters in the string representation of the link.
        /// If this value is greater or equal than buffer_size, output was truncated.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_link_as_string(
            IntPtr linkPtr,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringBuilderMarshaler))]StringBuilder buffer,
            int bufferSize);
        [DllImport("libspotify")]
        internal static extern int sp_link_as_string(
            IntPtr linkPtr,
            IntPtr buffer,
            int bufferSize);

        /// <summary>
        /// Gets the link type of the specified link.
        /// </summary>
        /// <param name="linkPtr">The link.</param>
        /// <returns>The link type of the specified link - see the SpotifyLinkType enum for possible values.</returns>
        [DllImport("libspotify")]
        internal static extern LinkType sp_link_type(IntPtr linkPtr);

        /// <summary>
        /// The track representation for the given link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose track you are interested in.</param>
        /// <returns>The track representation of the given track link.
        /// If the link is not of track type then NULL is returned.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_as_track(IntPtr linkPtr);

        /// <summary>
        /// The track and offset into track representation for the given link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose track you are interested in.</param>
        /// <param name="offsetPtr">The offset into track (in seconds). If the link does not contain an offset this will be set to 0.</param>
        /// <returns>The track representation of the given track link If the link is not of track type then NULL is returned.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_as_track_and_offset(IntPtr linkPtr, out int offsetPtr);

        /// <summary>
        /// The album representation for the given link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose album you are interested in.</param>
        /// <returns>The album representation of the given album link.
        /// If the link is not of album type then NULL is returned.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_as_album(IntPtr linkPtr);

        /// <summary>
        /// The artist representation for the given link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose artist you are interested in.</param>
        /// <returns>The artist representation of the given link.
        /// If the link is not of artist type then NULL is returned.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_as_artist(IntPtr linkPtr);

        /// <summary>
        /// The user representation for the given link
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose user you are interested in</param>
        /// <returns>The user representation of the given link</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_as_user(IntPtr linkPtr);

        /// <summary>
        /// Adds a reference to the specified link.
        /// </summary>
        /// <param name="linkPtr">The link.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_link_add_ref(IntPtr linkPtr);

        /// <summary>
        /// Releases the specified link.
        /// </summary>
        /// <param name="linkPtr">The link.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_link_release(IntPtr linkPtr);
    }
}