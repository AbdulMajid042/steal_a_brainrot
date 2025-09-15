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
        [SerializeField] string interTime;
        [SerializeField] string interGraceTimee;
        [SerializeField] string startAppOpenTime;
        [SerializeField] string watchAdPanel;
        [SerializeField] string checkInternetPanel;
        [SerializeField] string totalAttempts;
        [SerializeField] string loadingTime;
        [SerializeField] string thirdSlotLock;
        [SerializeField] string fourthSlotLock;
        [SerializeField] string showIAPinLoading;
        [SerializeField] string enableQuizz;
        [SerializeField] string quizScreen;
        [SerializeField] string enableSelectionScreens;
        [SerializeField] string appOpenAfterInter;
        [SerializeField] string appOpenCountAfterInter;

        [SerializeField] string occurance;
        [SerializeField] string needAppOpen;


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
            //InitFirebaseRemoteConfig();
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

            Debug.Log($"[Before Fetch] Inter Ad Time = {AdsManagerWrapper.Instance.idleAdTime}");
            Debug.Log($"[Before Fetch] Inter Grace AdTime = {AdsManagerWrapper.Instance.InterGraceTime}");
            Debug.Log($"[Before Fetch] AppOpen Ad Time = {AdsManagerWrapper.Instance.startAppOpenTime}");
            Debug.Log($"[Before Fetch]  Can Show Checl Internet Panel Value = {AdsManagerWrapper.Instance.canShowCheckInternetPanel}");
            Debug.Log($"[Before Fetch]  Inapp premium Attempts BreaksPoints = {AdsManagerWrapper.Instance.IAP_Attempts}");
            Debug.Log($"[Before Fetch] loading Ad Text time = {AdsManagerWrapper.Instance.loadingAdTextTime}");
            Debug.Log($"[Before Fetch] Third Slot is Lock = {AdsManagerWrapper.Instance.thirdSlotlock}");
            Debug.Log($"[Before Fetch] Fourth Slot is Loc = {AdsManagerWrapper.Instance.fourthSlotlock}");
            Debug.Log($"[Before Fetch] Shwo Inapp Panel in Loading = {AdsManagerWrapper.Instance.showIapLoading}");
            Debug.Log($"[Before Fetch] Enable Quiz is = {AdsManagerWrapper.Instance.enableQuiz}");
            Debug.Log($"[Before Fetch] Quiz Screen Count value is = {AdsManagerWrapper.Instance.quizScreenCount}");
            Debug.Log($"[Before Fetch] Enable Selection Screens = {AdsManagerWrapper.Instance.enableSelection}");



            var AppOpenTime = config.GetValue(startAppOpenTime);
            if (AppOpenTime.Source != Firebase.RemoteConfig.ValueSource.DefaultValue)
            {
                AdsManagerWrapper.Instance.startAppOpenTime = AppOpenTime.LongValue;
                Debug.Log($"[Remote Config] AppOpen Time fetched: {AdsManagerWrapper.Instance.startAppOpenTime}");
            }
            else
            {
                Debug.LogWarning($"[Remote Config] Key '{startAppOpenTime}' not found! Keeping default value: {AdsManagerWrapper.Instance.startAppOpenTime}");
            }




            // Fetch value and check if it is default/empty
            var interTimeValue = config.GetValue(interTime);
            if (interTimeValue.Source != Firebase.RemoteConfig.ValueSource.DefaultValue)
            {
                AdsManagerWrapper.Instance.idleAdTime = interTimeValue.LongValue;
                Debug.Log($"[Remote Config] Interstitial Time fetched: {AdsManagerWrapper.Instance.idleAdTime}");
            }
            else
            {
                Debug.LogWarning($"[Remote Config] Key '{interTime}' not found! Keeping default value: {AdsManagerWrapper.Instance.idleAdTime}");
            }







            var interGraceTime = config.GetValue(interGraceTimee);
            if (interTimeValue.Source != Firebase.RemoteConfig.ValueSource.DefaultValue)
            {
                AdsManagerWrapper.Instance.InterGraceTime = interGraceTime.LongValue;
                Debug.Log($"[Remote Config] Interstitial Grace Time fetched: {AdsManagerWrapper.Instance.InterGraceTime}");
            }
            else
            {
                Debug.LogWarning($"[Remote Config] Key '{interGraceTime}' not found! Keeping default value: {AdsManagerWrapper.Instance.InterGraceTime}");
            }


            var adsEnableValue = config.GetValue(watchAdPanel);
            if (adsEnableValue.Source != Firebase.RemoteConfig.ValueSource.DefaultValue)
            {
                AdsManagerWrapper.Instance.isWatchAdPanel = adsEnableValue.BooleanValue;
                Debug.Log($"[Remote Config] Watch AD Panel Panel: {AdsManagerWrapper.Instance.isWatchAdPanel}");
            }
            else
            {
                Debug.LogWarning($"[Remote Config] Key '{watchAdPanel}' not found! Keeping default value: {AdsManagerWrapper.Instance.isWatchAdPanel}");
            }





            var checkInternet = config.GetValue(checkInternetPanel);
            if (checkInternet.Source != Firebase.RemoteConfig.ValueSource.DefaultValue)
            {
                AdsManagerWrapper.Instance.canShowCheckInternetPanel = checkInternet.BooleanValue;
                Debug.Log($"[Remote Config] Watch AD Panel Panel: {AdsManagerWrapper.Instance.canShowCheckInternetPanel}");
            }
            else
            {
                Debug.LogWarning($"[Remote Config] Key '{checkInternetPanel}' not found! Keeping default value: {AdsManagerWrapper.Instance.canShowCheckInternetPanel}");
            }



            var IAP_Attempts = config.GetValue(totalAttempts);
            if (IAP_Attempts.Source != Firebase.RemoteConfig.ValueSource.DefaultValue)
            {
                AdsManagerWrapper.Instance.IAP_Attempts= IAP_Attempts.LongValue;
                Debug.Log($"[Remote Config] Watch AD Panel Panel: {AdsManagerWrapper.Instance.IAP_Attempts}");
            }
            else
            {
                Debug.LogWarning($"[Remote Config] Key '{IAP_Attempts}' not found! Keeping default value: {AdsManagerWrapper.Instance.IAP_Attempts}");
            }



            AdsManagerWrapper.Instance.loadingAdTextTime = RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(loadingTime).LongValue;
            AdsManagerWrapper.Instance.thirdSlotlock = RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(thirdSlotLock).BooleanValue;
            AdsManagerWrapper.Instance.fourthSlotlock = RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(fourthSlotLock).BooleanValue;
            AdsManagerWrapper.Instance.showIapLoading = RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(showIAPinLoading).BooleanValue;
            AdsManagerWrapper.Instance.enableQuiz = RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(enableQuizz).BooleanValue;
            AdsManagerWrapper.Instance.quizScreenCount = RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(quizScreen).LongValue;
            AdsManagerWrapper.Instance.enableSelection = RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(enableSelectionScreens).BooleanValue;
            AdsManagerWrapper.Instance.appOpenAfterIad = RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(appOpenAfterInter).BooleanValue;
            AdsManagerWrapper.Instance.appOpenCount = RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(appOpenCountAfterInter).LongValue;

            AdmobeAdsManager.instance.occurance = RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(occurance).LongValue;
            AdmobeAdsManager.instance.needAppOpen = RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(needAppOpen).BooleanValue;


            Debug.Log($"[After Fetch] Inter Ad Time = {AdsManagerWrapper.Instance.idleAdTime}");
            Debug.Log($"[After Fetch] Inter Grace AdTime = {AdsManagerWrapper.Instance.InterGraceTime}");
            Debug.Log($"[After Fetch] Appopen Time = {AdsManagerWrapper.Instance.startAppOpenTime}");
            Debug.Log($"[After Fetch] Watch AD Panel Value = {AdsManagerWrapper.Instance.isWatchAdPanel}");
            Debug.Log($"[After Fetch] Can Show Checl Internet Panel Value = {AdsManagerWrapper.Instance.canShowCheckInternetPanel}");
            Debug.Log($"[After Fetch] Inapp premium Attempts BreaksPoints = {AdsManagerWrapper.Instance.IAP_Attempts}");
            Debug.Log($"[After Fetch] loading Ad Text time = {AdsManagerWrapper.Instance.loadingAdTextTime}");
            Debug.Log($"[After Fetch] Third Slot is Lock = {AdsManagerWrapper.Instance.thirdSlotlock}");
            Debug.Log($"[After Fetch] Fourth Slot is Loc = {AdsManagerWrapper.Instance.fourthSlotlock}");
            Debug.Log($"[After Fetch] Shwo Inapp Panel in Loading = {AdsManagerWrapper.Instance.showIapLoading}");
            Debug.Log($"[After Fetch] Enable Quiz is = {AdsManagerWrapper.Instance.enableQuiz}");
            Debug.Log($"[After Fetch] Quiz Screen Count value is = {AdsManagerWrapper.Instance.quizScreenCount}");
            Debug.Log($"[After Fetch] Enable Selection Screens = {AdsManagerWrapper.Instance.enableSelection}");

        }
    }
}