using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WeaponData
{
    public string weaponName;
    public GameObject weaponSprite;
    public string price;        // shown as string, e.g. "1000"
    public int videosRequired;  // total videos required to unlock
    public bool isUnlocked;     // unlocked state
}

public class WeaponSelectionUI : MonoBehaviour
{
    [Header("Weapon List")]
    public List<WeaponData> weapons;

    [Header("UI References")]
    public Button leftButton;
    public Button rightButton;

    [Header("Bottom Buttons")]
    public Button coinBuyButton;
    public Text coinBuyText;

    public Button nextButton;


    public Button videoButton;
    public Text videoProgressText;

    [Header("Player Data")]
    public long playerCoins = 1000; // Example, load from your game manager

    private int currentIndex = 0;

    void Start()
    {
        playerCoins = RCC_PlayerPrefsX.GetLong("PlayerCurrency");
        leftButton.onClick.AddListener(OnLeftClicked);
        rightButton.onClick.AddListener(OnRightClicked);

        coinBuyButton.onClick.AddListener(OnBuyWithCoins);
        videoButton.onClick.AddListener(OnWatchVideo);

        LoadWeaponsState();
        UpdateUI();
    }

    void OnLeftClicked()
    {
        currentIndex = (currentIndex - 1 + weapons.Count) % weapons.Count;
        UpdateUI();
    }

    void OnRightClicked()
    {
        currentIndex = (currentIndex + 1) % weapons.Count;
        UpdateUI();
    }

    void OnBuyWithCoins()
    {
        var weapon = weapons[currentIndex];
        int priceInt = int.Parse(weapon.price);

        if (!weapon.isUnlocked && playerCoins >= priceInt)
        {
            playerCoins -= priceInt;
            RCC_PlayerPrefsX.SetLong("PlayerCurrency", RCC_PlayerPrefsX.GetLong("PlayerCurrency") - priceInt);
            weapon.isUnlocked = true;
            SaveWeaponsState();
            UpdateUI();
            Debug.Log("Unlocked with coins: " + weapon.weaponName);
        }
        else
        {
            Debug.Log("Not enough coins or already unlocked.");
        }
    }

    void OnWatchVideo()
    {
        if(Ads_Manager.instance)
        {
            Ads_Manager.instance.ShowRewardedVideo(WatchVideoGiveRewerd);
        }
    }

    public void WatchVideoGiveRewerd()
    {
        var weapon = weapons[currentIndex];
        if (weapon.isUnlocked) return;

        int watched = PlayerPrefs.GetInt(weapon.weaponName + "_VideosWatched", 0);
        watched++;
        PlayerPrefs.SetInt(weapon.weaponName + "_VideosWatched", watched);

        if (watched >= weapon.videosRequired)
        {
            weapon.isUnlocked = true;
            SaveWeaponsState();
            Debug.Log("Unlocked via videos: " + weapon.weaponName);
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if(PlayerPrefs.GetInt("ItemUnlocked_" + weapons[currentIndex].weaponName)==1)
        {
            weapons[currentIndex].isUnlocked = true;
        }

        foreach (var w in weapons)
        {
            if (w.weaponSprite != null)
                w.weaponSprite.SetActive(false);
        }
        var weapon = weapons[currentIndex];
        weapon.weaponSprite.SetActive(true);

        if (weapon.isUnlocked)
        {
            coinBuyText.text = "UNLOCKED";
            coinBuyButton.interactable = false;

            int watched = PlayerPrefs.GetInt(weapon.weaponName + "_VideosWatched", 0);
            videoProgressText.text = watched + "/" + weapon.videosRequired;
            videoButton.interactable = false;
            videoButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(true);
            coinBuyButton.gameObject.SetActive(false);
        }
        else
        {
            coinBuyText.text = "COINS " + weapon.price;
            coinBuyButton.interactable = true;

            int watched = PlayerPrefs.GetInt(weapon.weaponName + "_VideosWatched", 0);
            videoProgressText.text = watched + "/" + weapon.videosRequired;
            videoButton.interactable = true;
            videoButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(false);
            coinBuyButton.gameObject.SetActive(false);
        }
    }

    void SaveWeaponsState()
    {
        foreach (var weapon in weapons)
        {
            PlayerPrefs.SetInt("ItemUnlocked_"+weapon.weaponName, weapon.isUnlocked ? 1 : 0);
        }
    }

    void LoadWeaponsState()
    {
        foreach (var weapon in weapons)
        {
            weapon.isUnlocked = PlayerPrefs.GetInt(weapon.weaponName + "_Unlocked", weapon.isUnlocked ? 1 : 0) == 1;
        }
    }
}
