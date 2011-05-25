using System.ServiceModel;
using System.ServiceModel.Web;

namespace Torshify.Server.Contracts
{
    [ServiceContract]
    public interface IToplistBrowseService
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        Artist[] BrowseTopArtists();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        Album[] BrowseTopAlbums();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        Track[] BrowseTopTracks();
    }
}