using UnityEngine;
using UnityEngine.UI;

public class AdminMemerot : MonoBehaviour
{
    public string inappString;
    public string memerotName;
    public Button watchVideoButton;
    public Button buyButton;
    public Button nextButton;
    public int videosWatched;
    public int videosRequired;
    public Text videoProgressText;
    // Start is called before the first frame update
    void Start()
    {
        buyButton.onClick.AddListener(OnBuyClicked);
        watchVideoButton.onClick.AddListener(OnWatchVideoClicked);
        LoadProgress();
        UpdateUI();
    }
    void OnBuyClicked()
    {
        GameObject.FindObjectOfType<IAP_Manager>().BuyCharacter(inappString);
    }

    void OnWatchVideoClicked()
    {
        if (Ads_Manager.instance)
        {
            Ads_Manager.instance.ShowRewardedVideo(WatchVideoRewardMethod);
        }
    }

    void WatchVideoRewardMethod()
    {

        videosWatched++;

        PlayerPrefs.SetInt(memerotName + "_adsWatched", videosWatched);
        PlayerPrefs.Save();
        if (videosWatched >= videosRequired)
        {
            //    PlayerPrefs.SetInt($"Memerot_{currentIndex}_Unlocked", 1);
            PlayerPrefs.Save();
        }

        UpdateUI();
    }
    void UpdateUI()
    {
        videoProgressText.text = $"{videosWatched}/{videosRequired}";
        bool notUnlocked = videosWatched < videosRequired;
        watchVideoButton.interactable = notUnlocked;
        watchVideoButton.gameObject.SetActive(notUnlocked);
        buyButton.gameObject.SetActive(notUnlocked);
        nextButton.gameObject.SetActive(!notUnlocked);

        if (PlayerPrefs.GetInt(memerotName) == 1)
        {
            watchVideoButton.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(true);
        }
    }
    void LoadProgress()
    {
        videosWatched = PlayerPrefs.GetInt(memerotName + "_adsWatched");
    }

    public void UnlockCharacter()
    {
        PlayerPrefs.SetInt(memerotName, 1);
        UpdateUI();
    }
}
