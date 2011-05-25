using System;
using System.ServiceModel;

using Microsoft.Practices.ServiceLocation;

using Torshify.Server.Contracts;
using Torshify.Server.Extensions;
using System.Linq;

namespace Torshify.Server.Services
{
    [ServiceBehavior]
    public class SearchService : ISearchService
    {
        #region Public Methods

        public string[] GetRadioGenres()
        {
            return Enum.GetNames(typeof (SearchRadioGenre));
        }

        public int[] GetRadioGenreValues()
        {
            return Enum.GetValues(typeof(SearchRadioGenre)).Cast<int>().ToArray();
        }

        public SearchResult Search(string query, int trackOffset, int trackCount, int albumOffset, int albumCount, int artistOffset, int artistCount)
        {
            if (trackCount == 0)
            {
                trackCount = 100;
            }

            if (albumCount == 0)
            {
                albumCount = 100;
            }

            if (artistCount == 0)
            {
                artistCount = 100;
            }

            var session = ServiceLocator.Current.Resolve<ISession>();
            var search = session.Search(
                query,
                trackOffset,
                trackCount,
                albumOffset,
                albumCount,
                artistOffset,
                artistCount).
                WaitForCompletion();

            return search.ToDto();
        }

        public SearchResult SearchRadio(int fromYear, int toYear, int genre)
        {
            if (fromYear == 0)
            {
                fromYear = DateTime.Now.Year;
            }

            if (toYear == 0)
            {
                toYear = DateTime.Now.Year;
            }

            var session = ServiceLocator.Current.Resolve<ISession>();
            var search = session.Search(
                fromYear,
                toYear,
                (RadioGenre)genre)
                .WaitForCompletion();

            return search.ToDto();
        }

        #endregion Public Methods
    }
}