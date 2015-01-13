using System;
using System.Diagnostics;
using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativeAlbum : NativeObject, IAlbum
    {
        private Lazy<IArtist> _artist;

        #region Constructors

        public NativeAlbum(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Properties

        public bool IsLoaded
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_album_is_loaded(Handle);
                }
            }
        }

        public IArtist Artist
        {
            get
            {
                return _artist.Value;
            }
        }

        public string CoverId
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.ImageIdToString(Spotify.sp_album_cover(Handle, ImageSize.Normal));
                }
            }
        }

        public bool IsAvailable
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_album_is_available(Handle);
                }
            }
        }

        public string Name
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_album_name(Handle);
                }
            }
        }

        public AlbumType Type
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_album_type(Handle);
                }
            }
        }

        public int Year
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_album_year(Handle);
                }
            }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            _artist = new Lazy<IArtist>(() =>
                                            {
                                                AssertHandle();

                                                lock (Spotify.Mutex)
                                                {
                                                    return ArtistManager.Get(Session, Spotify.sp_album_artist(Handle));
                                                }
                                            });

            lock (Spotify.Mutex)
            {
                Spotify.sp_album_add_ref(Handle);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            // Dispose unmanaged
            if (!IsInvalid)
            {
                try
                {
                    lock (Spotify.Mutex)
                    {
                        Spotify.sp_album_release(Handle);
                    }
                }
                catch
                {
                }
                finally
                {
                    AlbumManager.Remove(Handle);
                }

                Debug.WriteLine("Album disposed");
            }

            base.Dispose(disposing);
        }

        #endregion Protected Methods
    }
}