using System;

namespace Torshify.Core.Native
{
    internal class NativeRadioSearch : NativeSearch
    {
        #region Fields

        private readonly int _fromYear;
        private readonly int _toYear;
        private readonly RadioGenre _genre;
        private readonly object _userData;

        #endregion Fields

        #region Constructors

        public NativeRadioSearch(NativeSession session, int fromYear, int toYear, RadioGenre genre, object userData = null)
            : this(session)
        {
            _fromYear = fromYear;
            _toYear = toYear;
            _genre = genre;
            _userData = userData;
        }

        private NativeRadioSearch(ISession session)
            : base(session, null, 0, 0, 0, 0, 0, 0, null)
        {
        }

        #endregion Constructors

        #region Public Methods

        public override void Initialize()
        {
            _callbacks = new NativeSearchCallbacks(this, _userData);
            _tracks = new Lazy<DelegateArray<ITrack>>(() => new DelegateArray<ITrack>(GetNumberOfTracks, GetTrackIndex));
            _albums = new Lazy<DelegateArray<IAlbum>>(() => new DelegateArray<IAlbum>(GetNumberOfAlbums, GetAlbumIndex));
            _artists = new Lazy<DelegateArray<IArtist>>(() => new DelegateArray<IArtist>(GetNumberOfArtists, GetArtistIndex));

            lock (Spotify.Mutex)
            {
                Handle = Spotify.sp_radio_search_create(
                    Session.GetHandle(),
                    (uint)_fromYear,
                    (uint)_toYear,
                    _genre,
                    _callbacks.CallbackHandle,
                    _callbacks.UserDataHandle);

                Spotify.sp_search_add_ref(Handle);
            }
        }

        #endregion Public Methods
    }
}