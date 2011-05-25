using System.ServiceModel;
using Torshify.Server.Contracts;

namespace Torshify.Server.Services
{
    [ServiceBehavior]
    public class PlaylistService : PlaylistServiceBase, IPlaylistService
    {
        #region Public Methods

        public void Subscribe()
        {
            var callbacks = OperationContext.Current.GetCallbackChannel<IPlaylistCallbacks>();

            PlaylistServiceCallbacksHandler.Instance.Register(callbacks);
        }

        public void Unsubscribe()
        {
            var callbacks = OperationContext.Current.GetCallbackChannel<IPlaylistCallbacks>();

            PlaylistServiceCallbacksHandler.Instance.Unregister(callbacks);
        }

        #endregion Public Methods
    }
}