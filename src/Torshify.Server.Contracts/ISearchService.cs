using System.ServiceModel;
using System.ServiceModel.Web;

namespace Torshify.Server.Contracts
{
    [ServiceContract]
    public interface ISearchService
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        SearchResult Search(
            string query,
            int trackOffset,
            int trackCount,
            int albumOffset,
            int albumCount,
            int artistOffset,
            int artistCount);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        SearchResult SearchRadio(
            int fromYear,
            int toYear,
            int genre);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string[] GetRadioGenres();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        int[] GetRadioGenreValues();
    }
}