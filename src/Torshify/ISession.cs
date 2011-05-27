using System;

namespace Torshify
{
    public interface ISession : IDisposable
    {
        #region Events

        event EventHandler<SessionEventArgs> ConnectionError;

        event EventHandler<SessionEventArgs> EndOfTrack;

        event EventHandler<SessionEventArgs> Exception;

        event EventHandler<SessionEventArgs> LoginComplete;

        event EventHandler<SessionEventArgs> LogMessage;

        event EventHandler<SessionEventArgs> LogoutComplete;

        event EventHandler<SessionEventArgs> MessageToUser;

        event EventHandler<SessionEventArgs> MetadataUpdated;

        event EventHandler<MusicDeliveryEventArgs> MusicDeliver;

        event EventHandler<SessionEventArgs> PlayTokenLost;

        event EventHandler<SessionEventArgs> StartPlayback;

        event EventHandler<SessionEventArgs> StopPlayback;

        event EventHandler<SessionEventArgs> StreamingError;

        event EventHandler<SessionEventArgs> UserinfoUpdated;

        event EventHandler<SessionEventArgs> OfflineStatusUpdated;

        #endregion Events

        #region Properties

        IPlaylistContainer PlaylistContainer
        {
            get;
        }

        IPlaylist Starred
        {
            get;
        }

        ConnectionState ConnectionState
        {
            get;
        }

        IArray<IUser> Friends
        {
            get;
        }

        IUser LoggedInUser
        {
            get;
        }

        #endregion Properties

        #region Methods

        void Login(string userName, string password);

        void Logout();

        Error PlayerLoad(ITrack track);

        Error PlayerPrefetch(ITrack track);

        void PlayerPause();

        void PlayerPlay();

        void PlayerSeek(TimeSpan offset);

        void PlayerUnload();

        ISearch Search(
            string query, 
            int trackOffset,
            int trackCount,
            int albumOffset, 
            int albumCount, 
            int artistOffset, 
            int artistCount,
            object userData = null);

        ISearch Search(
            int fromYear,
            int toYear,
            RadioGenre genre,
            object userData = null);

        IAlbumBrowse Browse(IAlbum album, object userData = null);

        IArtistBrowse Browse(IArtist artist, object userData = null);

        IToplistBrowse Browse(ToplistType type, object userData = null);

        IToplistBrowse Browse(ToplistType type, int encodedCountryCode, object userData = null);

        IToplistBrowse Browse(ToplistType type, string userName, object userData = null);

        IToplistBrowse BrowseCurrentUser(ToplistType type, object userData = null);

        IImage GetImage(string id);

        /// <summary>
        /// Set preferred bitrate for music streaming
        /// </summary>
        /// <param name="bitrate">Preferred bitrate</param>
        /// <returns>Current session</returns>
        ISession SetPreferredBitrate(Bitrate bitrate);

        /// <summary>
        /// Set preferred bitrate for offline sync
        /// </summary>
        /// <param name="bitrate">Preferred bitrat</param>
        /// <param name="resync">Set to true if libspotify should resynchronize already synchronized tracks. Usually you should set this to false.</param>
        /// <returns>Current session</returns>
        ISession SetPreferredOfflineBitrate(Bitrate bitrate, bool resync);

        /// <summary>
        /// Set current connection type
        /// </summary>
        /// <remarks>Used in conjunction with SetConnectionRules to control
        /// how libspotify should behave in respect to network activity and offline synchronization.</remarks>
        /// <param name="connectionType">Connection type</param>
        /// <returns></returns>
        ISession SetConnectionType(ConnectionType connectionType);

        /// <summary>
        /// Set rules for how libspotify connects to Spotify servers and synchronizes offline content
        /// </summary>
        /// <remarks>Used in conjunction with SetConnectionType to control
        /// how libspotify should behave in respect to network activity and offline synchronization.</remarks>
        /// <param name="connectionRule">Connection rules</param>
        /// <returns>Current session</returns>
        ISession SetConnectionRules(ConnectionRule connectionRule);

        /// <summary>
        /// Get total number of tracks that needs download before everything
        /// from all playlists that is marked for offline is fully synchronized
        /// </summary>
        /// <returns></returns>
        int GetNumberOfOfflineTracksRemainingToSync();

        /// <summary>
        /// Return number of playlisys that is marked for offline synchronization
        /// </summary>
        /// <returns></returns>
        int GetNumberOfOfflinePlaylists();

        /// <summary>
        /// Return offline synchronization status.
        /// </summary>
        /// <returns>Sync status</returns>
        OfflineSyncStatus GetOfflineSyncStatus();

        #endregion Methods
    }
}