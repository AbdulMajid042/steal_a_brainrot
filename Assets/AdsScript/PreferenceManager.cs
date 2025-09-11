using UnityEngine;

public class PreferenceManager : MonoBehaviour
{
    private const string Subscription = "Subscription";
    private const string RemoveAds = "RemoveAds";
    private const string _PrivacyPolicyStatus = "PrivacyPolicyStatus";
    public static bool GetAdsStatus()
    {
        //0 means ads enabled and 1 means ads are disabled. so true means ads are enabled and false means ads are disabled.

        //if (SubscriptionStatus != 0)
        //{
        //    return false;
        //}

        return (PlayerPrefs.GetInt(RemoveAds, 0) == 0);
    }

    public static void SetAdsStatus(int value)
    {
        PlayerPrefs.SetInt(RemoveAds, value);
    }

    public static int SubscriptionStatus
    {
        get
        {
            //return 1;
            return PlayerPrefs.GetInt(Subscription, 0);
        }
        set
        {
            PlayerPrefs.SetInt(Subscription, value);
        }
    }
    public static int PrivacyPolicyAgreeStatus
    {
        set
        {
            PlayerPrefs.SetInt(_PrivacyPolicyStatus, value);
        }
        get
        {
            return PlayerPrefs.GetInt(_PrivacyPolicyStatus, 0);
        }
    }
}
