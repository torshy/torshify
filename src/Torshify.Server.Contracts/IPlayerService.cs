using System.ServiceModel;
using System.ServiceModel.Web;

namespace Torshify.Server.Contracts
{
    [ServiceContract(CallbackContract = typeof(IPlayerCallbacks))]
    public interface IPlayerService
    {
        [OperationContract(IsOneWay = true)]
        void Subscribe();

        [OperationContract(IsOneWay = true)]
        void Unsubscribe();

        [OperationContract(IsOneWay = true)]
        void Enqueue(int trackId);

        [OperationContract(IsOneWay = true)]
        void Play(int trackId);

        [OperationContract(IsOneWay = true)]
        void TogglePlayPause();

        [OperationContract(IsOneWay = true)]
        void Pause();

        [OperationContract(IsOneWay = true)]
        void Seek(int milliseconds);

        [OperationContract(IsOneWay = true)]
        void Prefetch(int trackId);

        [OperationContract]
        bool Next();

        [OperationContract]
        bool Previous();

        [OperationContract]
        bool CanGoNext();

        [OperationContract]
        bool CanGoPrevious();
    }

    [ServiceContract]
    public interface IPlayerWebService
    {
        [OperationContract(IsOneWay = true)]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        void Enqueue(int trackId);

        [OperationContract(IsOneWay = true)]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        void Play(int trackId);

        [OperationContract(IsOneWay = true)]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        void Pause();

        [OperationContract(IsOneWay = true)]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        void TogglePlayPause();

        [OperationContract(IsOneWay = true)]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        void Seek(int milliseconds);

        [OperationContract(IsOneWay = true)]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        void Prefetch(int trackId);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        bool Next();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        bool Previous();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        bool CanGoNext();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        bool CanGoPrevious();
    }

    [ServiceContract]
    public interface IPlayerCallbacks
    {
        [OperationContract(IsOneWay = true)]
        void OnLostPlayToken();

        [OperationContract(IsOneWay = true)]
        void OnIsPlayingChanged(bool isPlaying);
    }
}