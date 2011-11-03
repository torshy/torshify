namespace Torshify
{
    public enum TrackOfflineStatus
    {
        No = 0,
        Waiting = 1,
        Downloading = 2,
        Done = 3,
        Error = 4,
        DoneExpired = 5,
        LimitExceeded = 6,
        DoneResync = 7
    }
}