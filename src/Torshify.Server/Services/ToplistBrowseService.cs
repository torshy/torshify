using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using Microsoft.Practices.ServiceLocation;

using Torshify.Server.Contracts;
using Torshify.Server.Extensions;

namespace Torshify.Server.Services
{
    [ServiceBehavior]
    public class ToplistBrowseService : IToplistBrowseService
    {
        #region Public Methods

        public Artist[] BrowseTopArtists()
        {
            return Browse(ToplistType.Artists, browse => browse.Artists, artist => artist.ToDto());
        }

        public Album[] BrowseTopAlbums()
        {
            return Browse(ToplistType.Albums, browse => browse.Albums, album => album.ToDto());
        }

        public Track[] BrowseTopTracks()
        {
            return Browse(ToplistType.Tracks, browse => browse.Tracks, track => track.ToDto());
        }

        #endregion Public Methods

        #region Private Methods

        private static T[] Browse<T, TK>(ToplistType type, Func<IToplistBrowse, IEnumerable<TK>> iterator, Func<TK, T> factory)
        {
            var session = ServiceLocator.Current.Resolve<ISession>();
            var browse = session.Browse(type).WaitForCompletion();
            var toplist = iterator(browse);
            var dtos = toplist.Select(factory).ToList();

            return dtos.ToArray();
        }

        #endregion Private Methods
    }
}