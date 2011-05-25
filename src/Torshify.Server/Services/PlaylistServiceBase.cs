using System;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Torshify.Server.Contracts;
using Torshify.Server.Extensions;

namespace Torshify.Server.Services
{
    public class PlaylistServiceBase
    {
        #region Public Methods

        public string[] GetPlaylistNames()
        {
            var session = ServiceLocator.Current.Resolve<ISession>();
            return session
                .PlaylistContainer
                .Playlists
                .Where(p => p.Type == PlaylistType.Playlist)
                .Select(p => p.Name).ToArray();
        }

        public Playlist GetPlaylist(string name)
        {
            var session = ServiceLocator.Current.Resolve<ISession>();
            var playlist =
                session
                    .PlaylistContainer
                    .Playlists
                    .Where(p => p.Type == PlaylistType.Playlist)
                    .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            return playlist != null ? playlist.ToDto() : null;
        }

        public Playlist[] GetPlaylists()
        {
            var session = ServiceLocator.Current.Resolve<ISession>();
            return session
                .PlaylistContainer
                .Playlists
                .Where(p => p.Type == PlaylistType.Playlist)
                .Select(p => p.ToDto()).ToArray();
        }

        #endregion Public Methods
    }
}