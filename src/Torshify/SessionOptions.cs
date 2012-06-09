namespace Torshify
{
    public class SessionOptions
    {
        #region Properties

        /// <summary>
        /// The location where Spotify will write cache files.
        /// This cache include tracks, cached browse results and coverarts.
        /// Set to empty string ("") to disable cache
        /// </summary>
        public string CacheLocation
        {
            get; set;
        }

        /// <summary>
        /// Device ID for offline synchronization
        /// </summary>
        public string DeviceID
        {
            get; set;
        }

        /// <summary>
        /// The location where Spotify will write setting files and per-user
        /// cache items. This includes playlists, track metadata, etc.
        /// 'SettingsLocation' may be the same path as 'CacheLocation'.
        /// </summary>
        public string SettingsLocation
        {
            get; set;
        }

        /// <summary>
        /// Path to API trace file
        /// </summary>
        public string TraceFileLocation
        {
            get; set;
        }

        /// <summary>
        /// "User-Agent" for your application - max 255 characters long
        /// The User-Agent should be a relevant, customer facing identification of your application
        /// </summary>
        public string UserAgent
        {
            get; set;
        }

        /// <summary>
        /// Compress local copy of playlists, reduces disk space usage
        /// </summary>
        public bool CompressPlaylists
        {
            get; 
            set;
        }

        /// <summary>
        /// Don't save metadata for local copies of playlists
        /// Reduces disk space usage at the expense of needing
        /// to request metadata from Spotify backend when loading list
        /// </summary>
        public bool DontSavePlaylistMetadata
        {
            get; 
            set;
        }

        /// <summary>
        /// Avoid loading playlists into RAM on startup. 
        /// See IPlaylist.IsInRam for more details.
        /// </summary>
        public bool InitiallyUnloadPlaylists
        {
            get; 
            set;
        }

        /// <summary>
        /// Url to the proxy server that should be used.
        /// The format is protocol://<host>:port (where protocal is http/https/socks4/socks5)
        /// </summary>
        public string Proxy
        {
            get; 
            set;
        }

        /// <summary>
        /// Username to authenticate with proxy server
        /// </summary>
        public string ProxyUsername
        {
            get; 
            set;
        }

        /// <summary>
        /// Password to authenticate with proxy server
        /// </summary>
        public string ProxyPassword
        {
            get; 
            set;
        }

        #endregion Properties
    }
}