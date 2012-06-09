using System;

namespace Torshify
{
    /// <summary>
    /// Controls the type of data that will be included in artist browse queries
    /// </summary>
    public enum ArtistBrowseType
    {
        /// <summary>
        /// All information except tophit tracks
        /// </summary>
        [Obsolete("This mode is deprecated and will removed in a future release")]
        Full,
        
        /// <summary>
        /// Only albums and data about them, no tracks.
        /// </summary>
        NoTracks,
        
        /// <summary>
        /// Only return data about the artist (artist name, similar artist biography, etc
        /// No tracks or album will be available.
        /// </summary>
        NoAlbums
    }
}