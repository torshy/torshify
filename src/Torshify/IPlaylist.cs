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

        #endregion Properties

        #region Methods

        void AutoLinkTracks(bool autoLink);

        #endregion Methods
    }
}