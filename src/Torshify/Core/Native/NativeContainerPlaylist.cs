using System;
using System.Runtime.InteropServices;

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
            get { return _type; }
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
            int index = _container.Playlists.IndexOf(this);
            int bufferSize = Spotify.STRINGBUFFER_SIZE;
            IntPtr bufferPtr = IntPtr.Zero;

            try
            {
                bufferPtr = Marshal.AllocHGlobal(bufferSize);
                Error error;
                lock (Spotify.Mutex)
                {
                    error = Spotify.sp_playlistcontainer_playlist_folder_name(_container.GetHandle(), index, bufferPtr, bufferSize);
                }

                return error == Error.OK ? Spotify.GetString(bufferPtr, string.Empty) : string.Empty;
            }
            finally
            {
                if (bufferPtr != IntPtr.Zero)
                {
                    try
                    {
                        Marshal.FreeHGlobal(bufferPtr);
                    }
                    catch
                    {
                    }
                }
            }
        }

        #endregion Private Methods
    }
}