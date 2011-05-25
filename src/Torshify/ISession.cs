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

        ISession SetPrefferedBitrate(Bitrate bitrate);

        #endregion Methods
    }
}