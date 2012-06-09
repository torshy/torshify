namespace Torshify
{
    public static class TrackExtensions
    {
        public static Error Load(this ITrack track)
        {
            return track.Session.PlayerLoad(track);
        }

        public static Error Play(this ITrack track)
        {
            var error = track.Load();

            if (error == Error.OK)
            {
                track.Session.PlayerPlay();
            }

            return error;
        }

        public static Error Prefetch(this ITrack track)
        {
            return track.Session.PlayerPrefetch(track);
        }
    }
}