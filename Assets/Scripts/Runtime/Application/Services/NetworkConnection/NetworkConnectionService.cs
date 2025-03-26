using UnityEngine;

namespace Application.Services.Network
{
    public class NetworkConnectionService : INetworkConnectionService
    {
        bool INetworkConnectionService.IsInternetReachable()
        {
            return UnityEngine.Application.internetReachability != NetworkReachability.NotReachable;
        }
    }
}