using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Analytics;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{

    public static FirebaseInitializer Instance;
    private FirebaseApp app;
    private bool isFirebaseInitialized;
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
    // Use this for initialization
    void Start () {

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                InitializeFirebase();
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    void InitializeFirebase()
    {
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        //add cloudmessaging delegates
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;


        isFirebaseInitialized = true;

    }

    void SetUserSignUpMethod(string signupmethod)
    {
        FirebaseAnalytics.SetUserProperty(
            FirebaseAnalytics.UserPropertySignUpMethod,
           signupmethod);
    }

    // Reset analytics data for this app instance.
    public void ResetAnalyticsData()
    {
        Debug.Log("Reset analytics data.");
        FirebaseAnalytics.ResetAnalyticsData();
    }

    public void LogClickEvent(string parameterValue)
    {
            Parameter[] parameters =
            {
                new Parameter(FirebaseAnalytics.ParameterContentType, "Click"),
                new Parameter(FirebaseAnalytics.ParameterItemId, parameterValue),
            };
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSelectContent, parameters);
    }

    public void LogCustomEvent(string eventName,Parameter[] parameters)
    {
        if (isFirebaseInitialized)
        {
            FirebaseAnalytics.LogEvent(eventName, parameters);
        }
      
    }


    public void LogCustomEvent(string eventName, Parameter parameter)
    {
        if (isFirebaseInitialized)
        {

            FirebaseAnalytics.LogEvent(eventName, parameter);
        }

    }

    public void LogCustomEvent(string eventName)
    {
        if (isFirebaseInitialized)
        {

            FirebaseAnalytics.LogEvent(eventName);
        }

    }

    public void SetScreenName(string screenName)
    {
        FirebaseAnalytics.SetCurrentScreen(screenName,"UnityPlayerActivity");
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);

    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
        foreach (var key in e.Message.Data.Keys)
        {
            Debug.Log("Key " + key + "Values" + e.Message.Data[key]);
        }
      
    }
}
