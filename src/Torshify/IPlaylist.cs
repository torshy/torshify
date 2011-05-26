using System;

namespace Torshify
{
    public interface IPlaylist : ISessionObject
    {
        #region Events

        event EventHandler<DescriptionEventArgs> DescriptionChanged;

        event EventHandler<ImageEventArgs> ImageChanged;

        event EventHandler MetadataUpdated;

        event EventHandler Renamed;

        event EventHandler StateChanged;

        event EventHandler<TrackCreatedChangedEventArgs> TrackCreatedChanged;

        event EventHandler<TracksAddedEventArgs> TracksAdded;

        event EventHandler<TrackSeenEventArgs> TrackSeenChanged;

        event EventHandler<TracksMovedEventArgs> TracksMoved;

        event EventHandler<TracksRemovedEventArgs> TracksRemoved;

        event EventHandler<PlaylistUpdateEventArgs> UpdateInProgress;

        event EventHandler SubscribersChanged;

        #endregion Events

        #region Properties

        bool IsLoaded 
        { 
            get;
        }

        string Description
        {
            get;
        }

        string ImageId
        {
            get;
        }

        bool IsCollaborative
        {
            get; set;
        }

        string Name
        {
            get; set;
        }

        bool PendingChanges
        {
            get;
        }

        IEditableArray<IPlaylistTrack> Tracks
        {
            get;
        }

        /// <summary>
        /// Get offline status
        /// </summary>
        /// <remarks>
        /// When in Downloading mode, the GetOfflineDownloadCompleted() can be used to query progress of the download
        /// </remarks>
        PlaylistOfflineStatus OfflineStatus
        {
            get;
        }

        #endregion Properties

        #region Methods

        void AutoLinkTracks(bool autoLink);

        /// <summary>
        /// Mark a playlist to be synchronized for offline playback
        /// </summary>
        /// <param name="offline"></param>
        void SetOfflineMode(bool offline);

        /// <summary>
        /// Get download progress for an offline playlist
        /// </summary>
        /// <returns>Value from 0 - 100 that indicates amount of playlist that is downloaded</returns>
        int GetOfflineDownloadCompleted();

        #endregion Methods
    }
}