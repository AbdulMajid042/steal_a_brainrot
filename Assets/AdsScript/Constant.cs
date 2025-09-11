using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public delegate void BasicCallBackWithParams(int pValue);
public delegate void BasicCallBack();

public static class Constant
{
    public static bool firebaseInitialized = false,firstAppOpenShown = false;
    public static void LogDesignEvent(string eventName)
    {

#if UNITY_ANDROID
        if (eventName.Contains(":"))
        {
            eventName = eventName.Replace(":", "_");
        }
        if (eventName.Contains("."))
        {
            eventName = eventName.Replace(".", "_");
        }
        if (firebaseInitialized)
        {
           Debug.Log("event Is:: " + eventName);
         //   Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName);
        }
#endif

    }
}
