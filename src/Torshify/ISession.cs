using System;
using System.Security.Authentication;

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
        
        event EventHandler<SessionEventArgs> OfflineError;

        event EventHandler<CredentialsBlobEventArgs> CredentialsBlobUpdated;

        event EventHandler<SessionEventArgs> ConnectionStateUpdated;

        event EventHandler<SessionEventArgs> ScrobbleError;

        event EventHandler<PrivateSessionModeChangedEventArgs> PrivateSessionModeChanged;

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

        IUser LoggedInUser
        {
            get;
        }

        /// <summary>
        ///  Get currently logged in users country
        /// <remarks>
        ///  Country encoded in an integer 'SE' = 'S' << 8 | 'E'
        /// </remarks>
        /// </summary>
        int LoggedInUserCountry
        {
            get;
        }

        /// <summary>
        /// Get/sets whether volume normalization should be enabled
        /// </summary>
        bool IsVolumeNormalizationEnabled
        {
            get; 
            set;
        }

        /// <summary>
        /// Get/set if private session is enabled. This disables sharing what the user is listening to
        /// to services such as Spotify Social, Facebook and LastFM. The private session will
        /// last for a time, and then libspotify will revert to the normal state. The private
        /// session is prolonged by user activity.
        /// </summary>
        bool IsPrivateSessionEnabled
        {
            get; 
            set;
        }

        #endregion Properties

        #region Methods

        void Login(string userName, string password, bool rememberMe = false);

        void LoginWithBlob(string userName, string blob);

        /// <summary>
        /// <exception cref="AuthenticationException">Thrown when no credentials are stored.</exception>
        /// </summary>
        void Relogin();

        /// <summary>
        /// Remove stored credentials in libspotify. If no credentials are currently stored, nothing will happen.
        /// </summary>
        void ForgetStoredLogin();

        /// <summary>
        /// Get username of the user that will be logged in via ISession.Relogin()
        /// </summary>
        /// <returns>Username of user which will be used in Relogin. If no login stored, it will return an empty string</returns>
        string GetRememberedUser();

        void Logout();

        Error PlayerLoad(ITrack track);

        Error PlayerPrefetch(ITrack track);

        Error PlayerPause();

        Error PlayerPlay();

        Error PlayerSeek(TimeSpan offset);

        Error PlayerUnload();

        ISearch Search(
            string query, 
            int trackOffset,
            int trackCount,
            int albumOffset, 
            int albumCount, 
            int artistOffset, 
            int artistCount,
            int playlistOffset,
            int playlistCount,
            SearchType searchType,
            object userData = null);

        IAlbumBrowse Browse(IAlbum album, object userData = null);

        IArtistBrowse Browse(IArtist artist, object userData = null);

        IArtistBrowse Browse(IArtist artist, ArtistBrowseType browseType, object userData = null);

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
        /// <returns>Current session</returns>
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
        /// Set maximum cache size in megabytes.
        /// Setting it to 0 (the default) will let libspotify automatically resize the cache (10% of disk free space)
        /// </summary>
        /// <param name="megabytes">Maximum cache size in megabytes.</param>
        /// <returns>Current session</returns>
        ISession SetCacheSize(uint megabytes);

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

        /// <summary>
        /// Returns the starred list for a user
        /// </summary>
        /// <param name="canonicalUserName">Canonical username</param>
        /// <returns> A playlist or NULL if no user is logged in</returns>
        IPlaylist GetStarredForUser(string canonicalUserName);

        /// <summary>
        /// Return the published container for given a canonical username,
        /// or the currently logged in user if canonicalUsername is NULL.
        /// </summary>
        /// <param name="canonicalUsername"></param>
        /// <returns>Playlist container object, NULL if not logged in.</returns>
        IPlaylistContainer GetPlaylistContainerForUser(string canonicalUsername);

        /// <summary>
        ///  This will make libspotify write all data that is meant to be stored
        /// on disk to the disk immediately. libspotify does this periodically
        /// by itself and also on logout. So under normal conditions this should never need to be used.
        /// </summary>
        Error FlushCaches();

        #endregion Methods
    }
}