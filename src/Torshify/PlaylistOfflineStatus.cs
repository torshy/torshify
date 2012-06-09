namespace Torshify
{
    public enum PlaylistOfflineStatus
    {
        /// <summary>
        /// The playlist is not offline enabled
        /// </summary>
        No = 0,

        /// <summary>
        /// Playlist is synchronized to local storage
        /// </summary>
        Yes = 1,

        /// <summary>
        /// This playlist is currently downloading. Only one playlist can be in this state any given time
        /// </summary>
        Downloading = 2,

        /// <summary>
        /// Playlist queued for download
        /// </summary>
        Waiting = 3
    }
}