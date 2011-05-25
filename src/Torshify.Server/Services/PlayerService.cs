using System;
using System.ServiceModel;
using Torshify.Server.Contracts;

namespace Torshify.Server.Services
{
    public class PlayerService : PlayerServiceBase, IPlayerService
    {
        public void Subscribe()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IPlayerCallbacks>();

            PlayerServiceCallbacksHandler.Instance.Register(callback);
        }

        public void Unsubscribe()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IPlayerCallbacks>();

            PlayerServiceCallbacksHandler.Instance.Unregister(callback);
        }
    }
}