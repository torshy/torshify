using System;

using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativePlaylistTrack : NativeTrack, IPlaylistTrack
    {
        #region Fields

        private readonly IPlaylist _playlist;
        private readonly int _position;

        #endregion Fields

        #region Constructors

        public NativePlaylistTrack(ISession session, IntPtr handle, IPlaylist playlist, int position)
            : base(session, handle)
        {
            _playlist = playlist;
            _position = position;
        }

        #endregion Constructors

        #region Properties

        public DateTime CreateTime
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    int trackCreateTime = Spotify.sp_playlist_track_create_time(_playlist.GetHandle(), _position);
                    return new DateTime(TimeSpan.FromSeconds(trackCreateTime).Ticks);
                }
            }
        }

        public IPlaylist Playlist
        {
            get
            {
                return _playlist;
            }
        }

        public bool Seen
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_playlist_track_seen(_playlist.GetHandle(), _position);
                }
            }
        }

        #endregion Properties

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() == typeof(NativeTrack))
            {
                return base.Equals(obj);
            }

            if (obj.GetType() != typeof(NativePlaylistTrack))
            {
                return false;
            }

            NativePlaylistTrack pt = (NativePlaylistTrack)obj;
            return pt._playlist == _playlist && pt._position == _position;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                PlaylistTrackManager.Remove(Playlist, _position);
            }

            base.Dispose(disposing);
        }

        #endregion Methods
    }
}