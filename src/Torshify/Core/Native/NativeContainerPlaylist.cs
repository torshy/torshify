using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativeContainerPlaylist : NativePlaylist, IContainerPlaylist
    {
        #region Fields

        private readonly IntPtr _folderId;
        private readonly PlaylistType _type;
        private readonly IPlaylistContainer _container;

        #endregion Fields

        #region Constructors

        public NativeContainerPlaylist(ISession session, IntPtr handle, IntPtr folderId, PlaylistType type, IPlaylistContainer container)
            : base(session, handle)
        {
            _folderId = folderId;
            _type = type;
            _container = container;
        }

        #endregion Constructors

        #region Properties

        public PlaylistType Type
        {
            get
            {
                return _type;
            }
        }

        public override string Name
        {
            get
            {
                if (Type == PlaylistType.Playlist)
                {
                    return base.Name;
                }

                if (Type == PlaylistType.StartFolder)
                {
                    return GetFolderName();
                }

                return null;
            }
            set
            {
                if (Type == PlaylistType.Playlist)
                {
                    base.Name = value;
                }

                throw new InvalidOperationException("Can't set the name of folders.");
            }
        }

        #endregion Properties

        #region Public Methods

        public ITrack[] GetUnseenTracks(int maxTracks, out int totalUnseenTracks)
        {
            List<ITrack> tracks = new List<ITrack>();

            AssertHandle();

            int intPtrSize = Marshal.SizeOf(typeof(IntPtr));
            IntPtr trackArrayPtr = Marshal.AllocHGlobal(intPtrSize * maxTracks);
            IntPtr[] trackArray = new IntPtr[maxTracks];

            lock (Spotify.Mutex)
            {
                totalUnseenTracks = Spotify.sp_playlistcontainer_get_unseen_tracks(_container.GetHandle(), Handle, trackArrayPtr, maxTracks);

                try
                {
                    Marshal.Copy(trackArrayPtr, trackArray, 0, trackArray.Length);

                    if (totalUnseenTracks > 0)
                    {
                        foreach (var trackPtr in trackArray)
                        {
                            if (trackPtr != IntPtr.Zero)
                            {
                                ITrack track = TrackManager.Get(Session, trackPtr);
                                tracks.Add(track);
                            }
                        }
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(trackArrayPtr);
                }
            }

            return tracks.ToArray();
        }

        public bool TryClearUnseenTracks()
        {
            AssertHandle();

            lock (Spotify.Mutex)
            {
                return Spotify.sp_playlistcontainer_clear_unseen_tracks(_container.GetHandle(), Handle) == 0;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() == typeof(NativePlaylist))
            {
                return base.Equals(obj);
            }

            if (obj.GetType() != typeof(NativeContainerPlaylist))
            {
                return false;
            }

            var cp = (NativeContainerPlaylist)obj;

            return cp.Handle == Handle && cp._folderId == _folderId && _type == cp._type;
        }

        #endregion Public Methods

        #region Private Methods

        private string GetFolderName()
        {
            IntPtr ptr = IntPtr.Zero;

            try
            {
                int index = _container.Playlists.IndexOf(this);
                int bufferSize = Spotify.STRINGBUFFER_SIZE;

                Error error;

                lock (Spotify.Mutex)
                {
                    ptr = Marshal.AllocHGlobal(bufferSize);
                    error = Spotify.sp_playlistcontainer_playlist_folder_name(_container.GetHandle(), index, ptr, bufferSize);
                }

                if (error == Error.OK)
                {
                    return Spotify.GetString(ptr, "Folder");
                }

                return error.GetMessage();
            }
            catch
            {
                return "Folder";
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        #endregion Private Methods
    }
}