using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativeTrack : NativeObject, ITrack
    {
        #region Fields

        private Lazy<IArray<IArtist>> _artists;
        private Lazy<IAlbum> _album;
        #endregion Fields

        #region Constructors

        public NativeTrack(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Properties

        public IAlbum Album
        {
            get
            {
                return _album.Value;
            }
        }

        public IArray<IArtist> Artists
        {
            get
            {
                return _artists.Value;
            }
        }

        public int Disc
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_track_disc(Handle);
                }
            }
        }

        public TimeSpan Duration
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return TimeSpan.FromMilliseconds(Spotify.sp_track_duration(Handle));
                }
            }
        }

        public Error Error
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_track_error(Handle);
                }
            }
        }

        public int Index
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_track_index(Handle);
                }
            }
        }

        public TrackAvailablity Availability
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_track_get_availability(Session.GetHandle(), Handle);
                }
            }
        }

        public TrackOfflineStatus OfflineStatus
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_track_offline_get_status(Handle);
                }
            }
        }

        public bool IsStarred
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_track_is_starred(Session.GetHandle(), Handle);
                }
            }
            set
            {
                AssertHandle();

                IntPtr arrayPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
                IntPtr[] ptrArray = new[] { Handle };
                try
                {
                    Marshal.Copy(ptrArray, 0, arrayPtr, 1);
                    lock (Spotify.Mutex)
                    {
                        Spotify.sp_track_set_starred(Session.GetHandle(), arrayPtr, 1, value);
                    }
                }
                finally
                {
                    try
                    {
                        Marshal.FreeHGlobal(arrayPtr);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public bool IsLocal
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_track_is_local(Session.GetHandle(), Handle);
                }
            }
        }

        public bool IsAutolinked
        {
            get 
            { 
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_track_is_autolinked(Session.GetHandle(), Handle);
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
                    return Spotify.sp_track_name(Handle);
                }
            }
        }

        public int Popularity
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_track_popularity(Handle);
                }
            }
        }

        public ITrack AutolinkedTrack
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return TrackManager.Get(Session, Spotify.sp_track_get_playable(Session.GetHandle(), Handle));
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
                    return Spotify.sp_track_is_loaded(Handle);
                }
            }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            lock (Spotify.Mutex)
            {
                Spotify.sp_track_add_ref(Handle);
            }

            _artists = new Lazy<IArray<IArtist>>(() => new DelegateArray<IArtist>(GetArtistCount, GetArtistIndex));
            _album = new Lazy<IAlbum>(() =>
                                          {
                                              AssertHandle();

                                              if (this.WaitUntilLoaded())
                                              {
                                                  lock (Spotify.Mutex)
                                                  {
                                                      if (Error == Error.OK)
                                                      {
                                                          return AlbumManager.Get(Session, Spotify.sp_track_album(Handle));
                                                      }
                                                  }
                                              }

                                              if (Error == Error.OtherPermanent)
                                              {
                                                  return null;
                                              }

                                              throw new InvalidOperationException("Track not loaded yet");
                                          });
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed
            }

            // Dispose unmanaged
            if (!IsInvalid)
            {
                try
                {
                    lock (Spotify.Mutex)
                    {
                        Ensure(() => Spotify.sp_track_release(Handle));
                    }
                }
                catch
                {
                }
                finally
                {
                    TrackManager.Remove(Handle);
                    Debug.WriteLine("Track disposed");
                }
            }

            base.Dispose(disposing);
        }

        #endregion Protected Methods

        #region Private Methods

        private IArtist GetArtistIndex(int index)
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return ArtistManager.Get(Session, Spotify.sp_track_artist(Handle, index));
            }
        }

        private int GetArtistCount()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_track_num_artists(Handle);
            }
        }

        #endregion Private Methods
    }
}