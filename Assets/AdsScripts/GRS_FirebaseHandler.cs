namespace Firebase.Sample.Analytics
{
    using Firebase;
    using Firebase.Analytics;
    using Firebase.Extensions;
    using UnityEngine;


    // Handler for UI buttons on the scene.  Also performs some 
    // necessary setup (initializing the firebase app, etc) on 
    // startup. 
    public class GRS_FirebaseHandler : MonoBehaviour
    {

        DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    public bool firebaseInitialized = false;
    public static GRS_FirebaseHandler Instance;
                  // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        ////if (PlayerPrefs.GetInt("privacyAccepted") == 1)
        ////{
        ////Invoke(nameof(initializeFB), 4f);
        ////}

        initializeFB();
    }

    public void initFBWithDelay()
    {
        Invoke(nameof(initializeFB), 4f);
    }

    void initializeFB()
    {
        try
        {
            Debug.unityLogger.logEnabled = true;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.Log(" GG >>Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }
        catch
        {
        }
    }

    void InitializeFirebase()
    {
        Debug.Log("GG >>Enabling data collection Firebase.");
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);


        //Debug.Log("Set user properties."); 
        //// Set the user's sign up method. 
        //FirebaseAnalytics.SetUserProperty( 
        //FirebaseAnalytics.UserPropertySignUpMethod, 
        //"Google"); 
        //// Set the user ID. 
        //FirebaseAnalytics.SetUserId("uber_user_510"); 
        //// Set default session duration values. 
        //FirebaseAnalytics.SetMinimumSessionDuration(new TimeSpan(0, 0, 10)); 
        //FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0)); 
        firebaseInitialized = true;
        //InitializeFireBaseMessaging();
        ////GRS_AdIDs.Check_Firebase = true;
        //LevelManegs.firebase = true;
        //GlobalScripts.firebase = true;
    }
    public void Analytics_DesignEvent(string _event)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("FB_" + _event); //HaseebConsoli
            Debug.Log("FB_" + _event);
        }
    }
    //public void InitializeFireBaseMessaging()
    //{
    //    Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
    //    Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
    //    Debug.Log("FB_"+"MESSAGING");

    //}
    //public void OnTokenReceived(object sender, Messaging.TokenReceivedEventArgs token)
    //{
    //    Debug.Log("Received Registration Token: " + token.Token);
    //}
    //public void OnMessageReceived(object sender, Messaging.MessageReceivedEventArgs e)
    //{
    //    Debug.Log("Received a new message from: " + e.Message.From);
    //}
    public void Analytics_ProgressionEvent_Mode(string world, int mode)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("FB_" + world + mode); //HaseebConsoli
            Debug.Log("FB_" + world + mode); //HaseebConsoli
        }

    }
        public void LogIAPPurchased(string world)
        {
            if (firebaseInitialized)
            {
                FirebaseAnalytics.LogEvent(world); //HaseebConsoli
            }
        }
        public void RewardedVideoComppleteEvent()
        {

        }
        public void OnRewardedVideoDisplayEvent()
        {

        }
        public void LogEventPlay(string world)
        {
            if (firebaseInitialized)
            {
                FirebaseAnalytics.LogEvent(world); //HaseebConsoli
            }
        }
        public void DesignEvent(string world)
        {
            if (firebaseInitialized)
            {
                FirebaseAnalytics.LogEvent(world); //HaseebConsoli
            }
        }
        public void Analytics_ProgressionEvent_Level(int mode, int level)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("FB_" + "_Mode_" + mode + "_Level_" + level); //HaseebConsoli
            Debug.Log("FB_" + "_Mode_" + mode + "_Level_" + level); //HaseebConsoli
        }

    }
    public void Analytics_ProgressionEvent_GamePlay(string word, int mode, int level)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("FB_" + word + "Character" + mode + "Skin" + level); //HaseebConsoli
            Debug.Log("FB_" + word + "_Mode_" + mode + "_Level_" + level); //HaseebConsoli
        }

    }
    public void Analytics_ProgressionEvent_StartWorking(string word, int mode, int level)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("FB_" + word + "_Mode_" + mode + "_Level_" + level); //HaseebConsoli
            Debug.Log("FB_" + word + "_Mode_" + mode + "_Level_" + level); //HaseebConsoli
        }

    }
    public void Analytics_ProgressionEvent_CheckPoint(string word, int mode, int level, int wayPoint, int checkPoint)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("FB_" + word + "_M_" + mode + "_L_" + level + "_W_" + wayPoint + "_C_" + checkPoint); //HaseebConsoli
            Debug.Log("FB_" + word + "_M_" + mode + "_L_" + level + "_W_" + wayPoint + "_C_" + checkPoint); //HaseebConsoli
        }

    }
}
}

