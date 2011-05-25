using System.ServiceModel;
using System.ServiceModel.Web;

namespace Torshify.Server.Contracts
{
    [ServiceContract(CallbackContract = typeof(IPlaylistCallbacks))]
    public interface IPlaylistService
    {
        [OperationContract]
        void Subscribe();

        [OperationContract]
        void Unsubscribe();

        [OperationContract]
        string[] GetPlaylistNames();

        [OperationContract]
        Playlist GetPlaylist(string name);

        [OperationContract]
        Playlist[] GetPlaylists();
    }

    [ServiceContract]
    public interface IPlaylistWebService
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string[] GetPlaylistNames();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        Playlist GetPlaylist(string name);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        Playlist[] GetPlaylists();
    }

    [ServiceContract]
    public interface IPlaylistCallbacks
    {
        [OperationContract(IsOneWay = true)]
        void OnTracksAdded(int playlistId, int[] trackIndices, Track[] tracks);

        [OperationContract(IsOneWay = true)]
        void OnTracksMoved(int playlistId, int[] trackIndices, int newPosition);

        [OperationContract(IsOneWay = true)]
        void OnTracksRemoved(int playlistId, int[] trackIndices);

        [OperationContract(IsOneWay = true)]
        void OnRenamed(int playlistId);

        [OperationContract(IsOneWay = true)]
        void OnPlaylistAdded(int position, int playlistId);

        [OperationContract(IsOneWay = true)]
        void OnPlaylistRemoved(int position, int playlistId);

        [OperationContract(IsOneWay = true)]
        void OnPlaylistMoved(int oldPosition, int newPosition);
    }
}