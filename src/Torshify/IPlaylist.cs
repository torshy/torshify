using System;
using System.Collections.ObjectModel;

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

        /// <summary>
        /// Gets a value indicating whether this playlist is loaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this playlist is loaded; otherwise, <c>false</c>.
        /// </value>
        bool IsLoaded 
        { 
            get;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Gets the image id.
        /// </summary>
        string ImageId
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this playlist is collaborative.
        /// </summary>
        /// <value>
        /// <c>true</c> if this playlist is collaborative; otherwise, <c>false</c>.
        /// </value>
        bool IsCollaborative
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the name of this playilst
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets a value indicating whether this playlist has any pending changes.
        /// </summary>
        /// <value>
        ///   <c>true</c> if it has any pending changes; otherwise, <c>false</c>.
        /// </value>
        bool PendingChanges
        {
            get;
        }

        /// <summary>
        /// Gets the tracks for this playlist
        /// </summary>
        IPlaylistTrackList Tracks
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

        /// <summary>
        /// Returns a collection of canonical usernames of who are subscribing to this playlist
        /// </summary>
        ReadOnlyCollection<string> Subscribers
        {
            get;
        }

        /// <summary>
        /// Returns the owner of the playlist
        /// </summary>
        IUser Owner
        {
            get;
        }

        /// <summary>
        /// Return whether a playlist is loaded in RAM (as opposed to only stored on disk)
        /// </summary>
        bool IsInRam
        {
            get; 
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Set autolinking state for the playlist
        /// 
        /// If a playlist is autolinked, unplayable tracks will be made playable
        /// by linking them to other Spotify tracks, where possible.
        /// </summary>
        /// <param name="autoLink">if set to <c>true</c> auto linking is enabled.</param>
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

        /// <summary>
        /// Ask library to update the subscription count for a playlist
        /// </summary>
        void UpdateSubscribers();

        #endregion Methods
    }
}