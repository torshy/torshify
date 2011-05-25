namespace Torshify
{
    /// <summary>
    /// Connection rules, bitwise OR of flags
    /// The default is SP_CONNECTION_RULE_NETWORK | SP_CONNECTION_RULE_ALLOW_SYNC
    /// </summary>
    public enum ConnectionRule
    {
        Network,
        NetworkIfRoaming,
        AllowSyncOverMobile,
        AllowSyncOverWifi
    }
}