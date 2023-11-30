using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using UnityEngine;


namespace GliderServices
{
    public static class ServiceConnection
    {

        public static async Task InitUnityServices() 
        {
            await UnityServices.InitializeAsync();
        }

        public static async Task SignIn()
        {            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign In Successful.");
        }

        public static void SignOut()
        {   
            AuthenticationService.Instance.SignOut();
        }

        public static void DisplayConnectionInfos()
        {
            Debug.Log(string.Format("Signed In: {0}", AuthenticationService.Instance.IsSignedIn));
            Debug.Log(string.Format("Authorized: {0}", AuthenticationService.Instance.IsAuthorized));
            Debug.Log(string.Format("Expired: {0}\n", AuthenticationService.Instance.IsExpired));
            Debug.Log(string.Format("Connected to Network: {0}", IsConnectedToNetwork()));
        }

        public static bool IsSignedIn() => AuthenticationService.Instance.IsSignedIn;
        // Checks players token is still valid.
        public static bool IsValidAccessToken() => AuthenticationService.Instance.IsAuthorized;

        // Only checks whether can connect to a network, not the quality of the network or whether network is connected to internet.
        // To actually check connected to internet, could send request to something like google.com using UnityEngine.Networking;
        public static bool IsConnectedToNetwork()
        {
            if (Application.internetReachability == 0 || NetworkReachability.NotReachable != 0) return false;
            return true;
        }

        public static async Task DeleteAccount()
        {
            await AuthenticationService.Instance.DeleteAccountAsync();
        }

        public static async void SyncPlayerNameServerToLocal(string localName)
        {
            if (localName == AuthenticationService.Instance.PlayerName) return;
            await AuthenticationService.Instance.UpdatePlayerNameAsync(localName);
        }

    }
}
