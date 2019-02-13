using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class AppsFlyerStartUp : MonoBehaviour {

    public static AppsFlyerStartUp Instance;
    private readonly string APPSFLYER_DEV_KEY = "hKzdj3CNAA25Nao2Hst2A3";
    private readonly string APP_PACKAGE_NAME = "com.vgsz.android.agentwarorigins";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        /* Mandatory - set your AppsFlyer’s Developer key. */
        AppsFlyer.setAppsFlyerKey(APPSFLYER_DEV_KEY);
        /* For detailed logging */
        AppsFlyer.setIsDebug(true);
#if UNITY_IOS
/* Mandatory - set your apple app ID
   NOTE: You should enter the number only and not the "ID" prefix */
   AppsFlyer.setAppID ("YOUR_APP_ID_HERE");
   AppsFlyer.trackAppLaunch ();
#elif UNITY_ANDROID
        /* Mandatory - set your Android package name */
        AppsFlyer.setAppID(APP_PACKAGE_NAME);
        /* For getting the conversion data in Android, you need to add the "AppsFlyerTrackerCallbacks" listener.*/
        AppsFlyer.init(APPSFLYER_DEV_KEY, "AppsFlyerTrackerCallbacks");
#endif
    }


    //custom events tracking

    public void TrackInAppPurchase(Product product)
    {
        Dictionary<string, string> inAppPurchaseEvent = new Dictionary<string, string>();
        inAppPurchaseEvent.Add(AFInAppEvents.REVENUE, product.metadata.localizedPrice.ToString());
        inAppPurchaseEvent.Add(AFInAppEvents.CURRENCY, product.metadata.isoCurrencyCode);
        inAppPurchaseEvent.Add(AFInAppEvents.RECEIPT_ID, product.transactionID);
        AppsFlyer.trackRichEvent(AFInAppEvents.PURCHASE, inAppPurchaseEvent);
    }

    public void TrackCustomEvent(string eventType)
    {

        AppsFlyer.trackRichEvent(eventType, null);
    }

    public void TrackRichEvent(string eventType,Dictionary<string,string> eventValues)
    {
        AppsFlyer.trackRichEvent(eventType, eventValues);
    }




}
