using UnityEngine;
using System.Collections;

    public class NetworkConnection
    {

        public static bool isNetworkAvailable()
        {
            bool isNetworkConnected = false;
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork
                || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                isNetworkConnected = true;
                return isNetworkConnected;

            }
            return isNetworkConnected;
        }


    }


