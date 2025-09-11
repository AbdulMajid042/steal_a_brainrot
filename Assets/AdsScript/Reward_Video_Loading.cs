using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Reward_Video_Loading : MonoBehaviour
{
    bool check;
    public Image Bar;
    public float Loading_Time = 8;
    public static Reward_Video_Loading ins;
    public bool Reward_2X = false;
    public GameObject MessagePanel;
    public Text Notification;
    public static bool ShowMRECAfterVideo = false;
    private void OnEnable()
    {
        ins = this;
        check = false;
        Loading_Time = 8;
        Bar.fillAmount = 0f;
        Time.timeScale = 1;
        if (AdsManagerWrapper.Instance)
        {
            if (AdsManagerWrapper.Instance.IsRewardedAdReady())
            {
                Loading_Time = .5f;
                if (Notification) { Notification.text = "CONGRATULATIONS YOUR REWARDED COINS HAS BEEN ADDED \n "; }
            }
            else
            {
                if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                {
                    AdsManagerWrapper.Instance.LoadRewardedAd();
                    Loading_Time = 8;
                }
                else
                {
                    Loading_Time = .5f;
                    if (Notification) { Notification.text = "INTERNET CONNECTION FAILED \n PLEASE CHECK YOUR INTERNET CONNECTION"; }
                }
            }
        }
    }
    void Update()
    {
        if (!check && Bar.fillAmount < 1)
            Bar.fillAmount += 1 / Loading_Time * Time.unscaledDeltaTime;

        // else { Bar.fillAmount += 1 / Loading_Time * Time.deltaTime; }   

        if (!check && Bar.fillAmount >= 1)
        {


            if (AdsManagerWrapper.Instance)
            {
                if (AdsManagerWrapper.Instance.IsRewardedAdReady())
                {
                    AdsManagerWrapper.Instance.ShowRewardedVideo(GiveReward);
                }
                else
                {
                    if (Notification) { Notification.text = "Reward Video Not AVAILABLE  \n PLEASE TRY AGAIN "; }
                    if (MessagePanel) { MessagePanel.SetActive(true); }
                }

            }
            else
            {
                if (Notification) { Notification.text = "INTERNET CONNECTION FAILED \n PLEASE CHECK YOUR INTERNET CONNECTION"; }
                if (MessagePanel) { MessagePanel.SetActive(true); }
            }

            this.gameObject.SetActive(false);
            check = true;
        }
    }

    public void GiveReward()
    {
        Debug.Log("Reward is given");
        if (Reward_2X)
        {
            int rewardAmount = 0;
            if (Notification) { Notification.text = "CONGRATULATIONS YOUR REWARDED COINS HAS BEEN ADDED "; }
            if (ShowMRECAfterVideo)
            {
                //if (GameHudController.instance)
                //{
                //   rewardAmount = GameHudController.instance.rewardForSuccess;
                //}
            }
            else
            {

                if (PlayerPrefs.GetInt("TotalCoins") <= 1000)
                {
                    rewardAmount = 400;
                }
                else if (PlayerPrefs.GetInt("TotalCoins") <= 4000)
                {
                    rewardAmount = 1000;
                }
                else if (PlayerPrefs.GetInt("TotalCoins") > 4000)
                {
                    rewardAmount = 2000;
                }
            }


            PlayerPrefs.SetInt("TotalCoins", (PlayerPrefs.GetInt("TotalCoins") + rewardAmount));

            if (MessagePanel)
            {
                MessagePanel.SetActive(true);
            }



            //if (GameHudController.instance)
            //{
            //    GameHudController.instance.rewardForSuccess = rewardAmount;
            //    GameHudController.instance.UpdateCoins();
            //}
            //if (MainMenu.ins)
            //{
            //    MainMenu.ins.UpdateCoins();
            //}

        }
    }
}
