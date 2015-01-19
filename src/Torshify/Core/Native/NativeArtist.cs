using System;
using System.Diagnostics;
using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativeArtist : NativeObject, IArtist
    {
        #region Constructors

        public NativeArtist(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Properties

        public string Name
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_artist_name(Handle);
                }
            }
        }

        public string PortraitId
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.ImageIdToString(Spotify.sp_artist_portrait(Handle, ImageSize.Normal));
                }
            }
        }

        public bool IsLoaded
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_artist_is_loaded(Handle);
                }
            }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            lock (Spotify.Mutex)
            {
                Spotify.sp_artist_add_ref(Handle);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            // Dispose managed
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
                        Ensure(() => Spotify.sp_artist_release(Handle));
                    }
                }
                catch
                {
                }
                finally
                {
                    ArtistManager.Remove(Handle);
                }
            }

            Debug.WriteLine("Artist disposed");
            base.Dispose(disposing);
        }

        #endregion Protected Methods
    }
}