namespace Firebase.Sample.Analytics
{
    using Firebase;
    using Firebase.Analytics;
    using Firebase.Extensions;
    using System;
    using System.Collections;
    using System.Threading.Tasks;
    using UnityEngine;

    public class FirebaseRemoteConfigHandler : MonoBehaviour
    {
        public static FirebaseRemoteConfigHandler Instance;
        public bool IsAddEnable =true;
        //IOSNoticationController iOSNoticationController;

        DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
        string notificationTitle, notificationBody, notificationSubtitle;
        bool fetchComplete = false;
        public static bool FetchDataComplete;
      [SerializeField]  private bool isFirebaseRemoteConfigInit;
     
        [HideInInspector]
        public bool showSbInMenu, showMbInMenu, showInterstitialInMenu, showAppOpenInMenu;
        [HideInInspector]
        public bool showSbInGameplay, showMbInGameplay, showInterstitialInGameplay, showAppOpenInGameplay;

        [Header("Firebase RemoteConfig Parameter Srings")]
        [SerializeField] string drainrotOccurance;
        [SerializeField] string drainrotNeedAppOpen;
        [SerializeField] string drainrot_revenue_multiplier;
        [SerializeField] string drainrot_needBanner;



        //[SerializeField] string AdsEnable;
        //[SerializeField] string funBooth;
        private void Awake()
        {
            Instance = this;
        }
        int PrivacyPolicyAgreeStatus = 0;
        private void Start()
        {
            //iOSNoticationController = GetComponent<IOSNoticationController>();
            if (PrivacyPolicyAgreeStatus == 1)
            {
                //InitFirebaseRemoteConfig();
            }
            //Invoke(nameof(Init_Helper), 4);

            Invoke("InitFirebaseRemoteConfig", 2f);
        //    InitFirebaseRemoteConfig();
        }
        public void InitFirebaseRemoteConfig()
        {

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    Debug.Log("GG >> FirebaseRemoteConfig:Initialized");
                    Init_Helper();
                }
                else
                {

                }
            });
        }
        public void Init_Helper()
        {
            Debug.Log("Helper Initialized ");
            System.Collections.Generic.Dictionary<string, object> defaults =
            new System.Collections.Generic.Dictionary<string, object>();
            FirebaseApp app = FirebaseApp.DefaultInstance;
            RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);

            isFirebaseRemoteConfigInit = true;

            FetchFireBase();
            StartCoroutine(FetchRemoteData());
            //Invoke(nameof(FetchFireBase), 3f);
        }

        IEnumerator FetchRemoteData()
        {
            Debug.Log("Fetching Data Enter Coroutine");

            FetchFireBase();
            yield return new WaitForSeconds(2);
            //FetchDataAsync();
        }


        public void FetchFireBase()
        {
            FetchDataAsync();

        }
        public Task FetchDataAsync()
        {
            Debug.Log("GG >> Fetching data...");
            Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                TimeSpan.Zero);
            return fetchTask.ContinueWith(FetchComplete);
        }
        void FetchComplete(Task fetchTask)
        {
            if (fetchTask.IsCanceled)
            {
                Debug.Log("GG >> Fetch cancelled.");
            }
            else if (fetchTask.IsFaulted)
            {
                Debug.Log("GG >> Fetch encountered an error.");
            }
            else if (fetchTask.IsCompleted)
            {
                Debug.Log("GG >> Fetch completed successfully!");
                FetchDataComplete = true;
            }

            var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
            switch (info.LastFetchStatus)
            {
                case Firebase.RemoteConfig.LastFetchStatus.Success:
                    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                    Debug.Log(String.Format("GG >> Remote data loaded and ready (last fetch time {0}).",
                        info.FetchTime));
                    StoreRemoteValues();
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason)
                    {
                        case Firebase.RemoteConfig.FetchFailureReason.Error:
                            Debug.Log("GG >> Fetch failed for unknown reason");
                            break;
                        case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                            Debug.Log("GG >> Fetch throttled until " + info.ThrottledEndTime);
                            break;
                    }
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Pending:
                    Debug.Log("GG >> Latest Fetch call still pending.");
                    break;
            }
        }

        public void StoreRemoteValues()
        {
            if (!isFirebaseRemoteConfigInit || !FetchDataComplete)
            {
                Debug.LogWarning("GG >> Either Firebase isn't initialized or data fetch isn't complete");
                return;
            }
            var config = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance;
            Ads_Manager.instance.drainrotOccurance = RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(drainrotOccurance).LongValue;
            Ads_Manager.instance.drainrotNeedAppOpen = RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(drainrotNeedAppOpen).BooleanValue;
            Ads_Manager.instance.drainrot_revenue_multiplier = RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(drainrot_revenue_multiplier).LongValue;
            Ads_Manager.instance.drainrot_needBanner = RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(drainrot_needBanner).BooleanValue;
        }
    }
}