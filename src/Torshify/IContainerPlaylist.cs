namespace Torshify
{
    public interface IContainerPlaylist : IPlaylist
    {
        PlaylistType Type { get; }

        /// <summary>
        /// Get the number of new tracks in a playlist since the corresponding
        /// function TryClearUnseenTracks() was called.
        /// </summary>
        /// <param name="maxReturnedTracks"></param>
        /// <returns></returns>
        ITrack[] GetUnseenTracks(int maxReturnedTracks, out int totalUnseenTracks);

        /// <summary>
        /// Clears a playlist for unseen tracks, so that next call to GetUnseenTracks() will return en empty list until a new track is added to the playslist.
        /// </summary>
        bool TryClearUnseenTracks();
    }
}
