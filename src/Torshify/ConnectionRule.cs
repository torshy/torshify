using System;

namespace Torshify
{
    /// <summary>
    /// Connection rules, bitwise OR of flags
    /// The default is SP_CONNECTION_RULE_NETWORK | SP_CONNECTION_RULE_ALLOW_SYNC
    /// </summary>
    [Flags]
    public enum ConnectionRule
    {
        /// <summary>
        /// Allow network traffic. When not set libspotify will force itself into offline mode
        /// </summary>
        Network = 0x1,

        /// <summary>
        /// Allow network traffic even if roaming
        /// </summary>
        NetworkIfRoaming = 0x2,

        /// <summary>
        /// Set to allow syncing of offline content over mobile connections
        /// </summary>
        AllowSyncOverMobile = 0x4,

        /// <summary>
        /// Set to allow syncing of offline content over WiFi
        /// </summary>
        AllowSyncOverWifi = 0x8
    }
}