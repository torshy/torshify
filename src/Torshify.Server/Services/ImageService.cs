using System.IO;
using System.ServiceModel.Web;

using Microsoft.Practices.ServiceLocation;

using Torshify.Server.Contracts;
using Torshify.Server.Extensions;

namespace Torshify.Server.Services
{
    public class ImageService : IImageService
    {
        public Stream GetImage(string id)
        {
            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "image/png";
            }

            var session = ServiceLocator.Current.Resolve<ISession>();
            var image = session.GetImage(id).WaitForCompletion();

            return new MemoryStream(image.Data);
        }
    }
}