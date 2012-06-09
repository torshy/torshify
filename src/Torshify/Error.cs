using Torshify.Core.Native;

namespace Torshify
{
    public enum Error
    {
        OK = 0,
        BadApiVersion = 1,
        ApiInitializationFailed = 2,
        TrackNotPlayable = 3,
        BadApplicationKey = 5,
        BadUsernameOrPassword = 6,
        UserBanned = 7,
        UnableToContactServer = 8,
        ClientTooOld = 9,
        OtherPermanent = 10,
        BadUserAgent = 11,
        MissingCallback = 12,
        InvalidIndata = 13,
        IndexOutOfRange = 14,
        UserNeedsPremium = 15,
        OtherTransient = 16,
        IsLoading = 17,
        NoStreamAvailable = 18, 
        PermissionDenied = 19, 
        InboxIsFull = 20, 
        NoCache = 21, 
        NoSuchUser = 22,
        NoCredentials = 23,
        NetworkDisabled = 24,
        InvalidDeviceID = 25,
        CantOpenTraceFile = 26,
        ApplicationBanned = 27,
        OfflineTooManyTracks = 31,
        OfflineDiskCache = 32,
        OfflineExpired = 33,
        OfflineNotAllowed = 34,
        OfflineLicenseLost = 35,
        OfflineLicenseError = 36,
        LastFmAuthError = 39,
        InvalidArgument = 40,
        SystemFailure = 41
    }

    public static class ErrorExtensions
    {
        public static string GetMessage(this Error error)
        {
            lock (Spotify.Mutex)
            {
                return Spotify.sp_error_message(error);
            }
        }
    }
}