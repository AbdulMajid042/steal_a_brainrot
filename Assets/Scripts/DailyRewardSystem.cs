using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardSystem : MonoBehaviour
{
    [Header("UI Setup")]
    public GameObject rewardPanel;      // The whole daily reward panel
    public Button[] claimButtons;       // Buttons for each day
    public GameObject[] lockIcons;      // Lock icons
    public GameObject[] claimedIcons;   // "Claimed" overlays/images
    public Button getX3Button;          // "Get X3" button

    [Header("Reward Settings")]
    public int[] rewardAmounts = { 100, 200, 300, 400, 500, 600, 1000 };

    private int currentDay;
    private DateTime lastClaimDate;
    private int todayReward = 0;
    private bool rewardClaimed = false;

    private void Start()
    {
        LoadData();
        UpdateUI();

        // Show panel only if today's reward is unclaimed
        rewardPanel.SetActive(IsRewardReadyToClaim());
        getX3Button.onClick.AddListener(ClaimTripleReward);
        for (int i = 0; i < claimButtons.Length; i++)
        {
            int index = i; // capture local copy for delegate
            claimButtons[i].onClick.RemoveAllListeners();
            claimButtons[i].onClick.AddListener(() => ClaimReward(index));
        }
    }

    void LoadData()
    {
        currentDay = PlayerPrefs.GetInt("DailyReward_Day", 0);
        string lastDate = PlayerPrefs.GetString("DailyReward_Date", "");

        if (!string.IsNullOrEmpty(lastDate))
            lastClaimDate = DateTime.Parse(lastDate);
        else
            lastClaimDate = DateTime.MinValue;

        // If a new day has passed
        if (lastClaimDate.Date < DateTime.Now.Date)
        {
            if (currentDay < rewardAmounts.Length)
                currentDay++;
            else
                currentDay = 1; // Reset cycle
            rewardClaimed = false; // reset claimed for new day
        }
        else
        {
            rewardClaimed = PlayerPrefs.GetInt("DailyReward_Claimed", 0) == 1;
        }
    }

    void UpdateUI()
    {
        if(currentDay!=0)
            claimButtons[currentDay-1].gameObject.GetComponent<Animator>().enabled=true;

        for (int i = 0; i < claimButtons.Length; i++)
        {
            bool unlocked = i < currentDay;
            bool claimed = (i + 1) < currentDay || (rewardClaimed && (i + 1) == currentDay);

            claimButtons[i].interactable = unlocked && !claimed;
            lockIcons[i].SetActive(!unlocked);
            claimedIcons[i].SetActive(claimed);
            getX3Button.interactable = true;
        }
    }

    public void ClaimReward(int dayIndex)
    {
        if ((dayIndex + 1) != currentDay || rewardClaimed) return;

        todayReward = rewardAmounts[dayIndex];
        Debug.Log($"Claimed reward: {todayReward} coins");

        GivePlayerCoins(todayReward);

        rewardClaimed = true;
        SaveData();
        UpdateUI();

        getX3Button.interactable = true;
    }

    public void ClaimTripleReward()
    {
        if (Ads_Manager.instance)
        {
            Ads_Manager.instance.ShowRewardedVideo(GiveTrippleReward);
        }
    }

    void GiveTrippleReward()
    {
        todayReward = rewardAmounts[currentDay - 1];
        int tripleReward = todayReward * 3;
        Debug.Log($"Claimed TRIPLE reward: {tripleReward} coins");

        // TODO: Replace with ad integration
        GivePlayerCoins(tripleReward);

        getX3Button.interactable = false;

        rewardPanel.SetActive(false);
    }


    void SaveData()
    {
        PlayerPrefs.SetInt("DailyReward_Day", currentDay);
        PlayerPrefs.SetString("DailyReward_Date", DateTime.Now.ToString());
        PlayerPrefs.SetInt("DailyReward_Claimed", rewardClaimed ? 1 : 0);
        PlayerPrefs.Save();
    }

    void GivePlayerCoins(int amount)
    {
        // TODO: Replace with your currency system
        PlayerPrefs.SetInt("PlayerCurrency", PlayerPrefs.GetInt("PlayerCurrency") + amount);
        GameObject.FindObjectOfType<Menu>().coinAttraction.SetActive(false);
        GameObject.FindObjectOfType<Menu>().coinAttraction.SetActive(true);
        rewardPanel.SetActive(false);
    }

    bool IsRewardReadyToClaim()
    {
        // Reward is ready if it's a new day AND today's reward is not yet claimed
        return !rewardClaimed && currentDay > 0;
    }
}
