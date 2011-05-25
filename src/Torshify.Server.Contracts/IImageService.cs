using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Torshify.Server.Contracts
{
    [ServiceContract]
    public interface IImageService
    {
        [OperationContract]
        [WebGet]
        Stream GetImage(string id);
    }
}