using UnityEngine;
using UnityEngine.UI;

public class MemerotSelectionManager : MonoBehaviour
{
    [System.Serializable]
    public class Memerot
    {
        public bool wantBuyButton;
        public string inappString;
        public string name;
        public string price;
        public string incomePerSec;
        public string rarity;
        public GameObject modelPrefab;

        public int videosRequired = 10; // How many videos needed
    }

    [Header("Memerot Data")]
    public Memerot[] memerots;
    private int currentIndex = 0;

    [Header("UI References")]
    public Text nameText;
    public Text priceText;
    public Text incomeText;
    public Text rarityText;
    public Text videoProgressText;
    public Text inappPriceText;

    public Button buyButton;
    public Button nextButton;
    public Button skipButton;
    public Button watchVideoButton;

    private GameObject currentModelInstance;

    // Track how many videos user watched per memerot
    private int[] videosWatched;

    void Start()
    {
        videosWatched = new int[memerots.Length];

        // 🔹 Load saved progress
        for (int i = 0; i < memerots.Length; i++)
        {
            videosWatched[i]=PlayerPrefs.GetInt(memerots[i].name + "_adsWatched");
        //    videosWatched[i] = PlayerPrefs.GetInt($"Memerot_{i}_Videos", 0);
        }

        UpdateUI();

        buyButton.onClick.AddListener(OnBuyClicked);
        skipButton.onClick.AddListener(OnSkipClicked);
        watchVideoButton.onClick.AddListener(OnWatchVideoClicked);
    }

    void UpdateUI()
    {
        Memerot memerot = memerots[currentIndex];

        buyButton.gameObject.SetActive(memerot.wantBuyButton);
        nameText.text = memerot.name;
        priceText.text = memerot.price;
        incomeText.text = memerot.incomePerSec;
        rarityText.text = memerot.rarity;
        inappPriceText.text = $"Buy $ {memerot.videosRequired}";

        videoProgressText.text = $"{videosWatched[currentIndex]}/{memerot.videosRequired}";

        if (currentModelInstance != null)
            currentModelInstance.SetActive(false);

        if (memerot.modelPrefab != null)
        {
            memerot.modelPrefab.SetActive(true);
            currentModelInstance = memerot.modelPrefab;
        }

        bool notUnlocked = videosWatched[currentIndex] < memerot.videosRequired;
        watchVideoButton.interactable = notUnlocked;
        watchVideoButton.gameObject.SetActive(notUnlocked);
        nextButton.gameObject.SetActive(!notUnlocked);

        if(PlayerPrefs.GetInt($"Memerot_{currentIndex}_Unlocked")==1)
        {
            watchVideoButton.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(true);
        }
    }

    public void NextMemerot()
    {
        currentIndex++;
        if (currentIndex >= memerots.Length) currentIndex = 0;
        UpdateUI();
    }

    public void PreviousMemerot()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = memerots.Length - 1;
        UpdateUI();
    }

    void OnBuyClicked()
    {
        GameObject.FindObjectOfType<IAP_Manager>().BuyCharacter(memerots[currentIndex].inappString);
    }

    void OnSkipClicked()
    {
        Debug.Log("Skipped selection!");
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
        Memerot memerot = memerots[currentIndex];

        Debug.Log("User watched a rewarded video ad");
        videosWatched[currentIndex]++;

        // 🔹 Save progress
        PlayerPrefs.SetInt(memerot.name + "_adsWatched", videosWatched[currentIndex]);
    //    PlayerPrefs.SetInt($"Memerot_{currentIndex}_Videos", videosWatched[currentIndex]);
        PlayerPrefs.Save();

        if (videosWatched[currentIndex] >= memerot.videosRequired)
        {
            Debug.Log($"{memerot.name} unlocked by watching videos!");
            PlayerPrefs.SetInt($"Memerot_{currentIndex}_Unlocked", 1);
            PlayerPrefs.SetInt(memerot.name, 1);
            PlayerPrefs.Save();
        }
        UpdateUI();
    }
    public void UnlockCharacter()
    {
        Memerot memerot = memerots[currentIndex];
        PlayerPrefs.SetInt($"Memerot_{currentIndex}_Unlocked", 1);
        PlayerPrefs.SetInt(memerot.name, 1);
        UpdateUI();
    }
    public void UnlockAllCharacter()
    {
        for (int i = 0; i < memerots.Length; i++)
        {
            Memerot memerot = memerots[i];

            // Mark as unlocked
            PlayerPrefs.SetInt($"Memerot_{i}_Unlocked", 1);
            PlayerPrefs.SetInt(memerot.name, 1);

            // Set videos watched to max
            videosWatched[i] = memerot.videosRequired;
            PlayerPrefs.SetInt(memerot.name + "_adsWatched", videosWatched[i]);
        }
        PlayerPrefs.Save();
        UpdateUI();
    }
}
