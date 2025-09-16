using UnityEngine;
using UnityEngine.UI;

public class CharacterUnlockPanel : MonoBehaviour
{
    [System.Serializable]
    public class CharacterUnlockData
    {
        public string characterName;
        public string inAppString;
        public int adsRequired = 10;   // How many ads needed
        public int adsWatched = 0;     // Current progress
        public long price;              // Optional: for IAP
        public int inAppPrice;              // Optional: for IAP
        public long income;
        public string rarity;
        public bool isUnlocked = false;
    }

    [Header("UI References")]
    public Text characterNameText;
    public Text priceText;
    public Text inAppPriceText;
    public Text incomeText;
    public Text rarityText;
    public Button watchAdButton;
    public Text watchAdButtonText; // Show progress 2/10 on the button
    public Button buyButton;

    [Header("Character Data")]
    public CharacterUnlockData characterData;

    private void OnEnable()
    {
        LoadProgress();
        UpdateUI();
        buyButton.interactable = true;
        watchAdButton.interactable = true;

        watchAdButton.onClick.AddListener(OnWatchAdClicked);
        buyButton.onClick.AddListener(OnBuyClicked);
    }

    public void UpdateUI()
    {
        characterNameText.text = characterData.characterName;
        priceText.text = "Price: " + FormatNumber(characterData.price);
        inAppPriceText.text = "Buy "+ "$ " + FormatNumber(characterData.inAppPrice);
        incomeText.text = "Income: " + FormatNumber(characterData.income);
        rarityText.text = "Rarity: " + characterData.rarity;
        watchAdButtonText.text = $"{characterData.adsWatched}/{characterData.adsRequired}";
        //if (characterData.isUnlocked)
        //{
        //    watchAdButton.interactable = false;
        //    buyButton.interactable = false;
        //    watchAdButtonText.text = "Unlocked!";
        //}
        //else
        //{
        //    watchAdButtonText.text = $"{characterData.adsWatched}/{characterData.adsRequired}";
        //}
    }
    string FormatNumber(long value)
    {
        if (value >= 1_000_000_000_000) // trillions
            return (value / 1_000_000_000_000f).ToString("0.#") + "t";
        else if (value >= 1_000_000_000) // billions
            return (value / 1_000_000_000f).ToString("0.#") + "b";
        else if (value >= 1_000_000) // millions
            return (value / 1_000_000f).ToString("0.#") + "m";
        else if (value >= 1_000) // thousands
            return (value / 1_000f).ToString("0.#") + "k";
        else
            return value.ToString();
    }

    void OnWatchAdClicked()
    {
        if (PlayerPrefs.GetInt("Hack") == 1)
        {
            characterData.adsWatched = characterData.adsRequired;
            OnAdSuccess();
            return;
        }
        if (AdsManagerWrapper.Instance)
        {
            AdsManagerWrapper.Instance.ShowRewardedVideo(OnAdSuccess);
        }
    }

    void OnAdSuccess()
    {
        characterData.adsWatched++;
        if(Player.instance.unlockable.GetComponent<BuyThisCharacter>())
        {
            Player.instance.unlockable.GetComponent<BuyThisCharacter>().adsWatched++;
        }
        if (characterData.adsWatched >= characterData.adsRequired)
        {
            UnlockCharacter();
        }

        SaveProgress();
        UpdateUI();
    }

    void OnBuyClicked()
    {
        GameObject.FindObjectOfType<IAP_Manager>().BuyCharacter(characterData.inAppString);
    }

    public void OnIAPSuccess()
    {
        UnlockCharacter();
        SaveProgress();
        UpdateUI();
    }

    void UnlockCharacter()
    {
        characterData.isUnlocked = true;
        gameObject.SetActive(false);
        PlayerPrefs.SetInt(characterData.characterName, 1);
        if(Player.instance.unlockable)
        {
            Player.instance.unlockable.GetComponent<Brainrot>().enabled = true;
            Player.instance.unlockable.GetComponent<BuyThisCharacter>().Unlocked();
            Player.instance.unlockable.GetComponent<BuyThisCharacter>().enabled=false;
        }
    }

    void SaveProgress()
    {
        string key = characterData.characterName;
        PlayerPrefs.SetInt(key + "_adsWatched", characterData.adsWatched);
        PlayerPrefs.SetInt(key + "_unlocked", characterData.isUnlocked ? 1 : 0);
        PlayerPrefs.Save();
    }

    void LoadProgress()
    {
        string key = characterData.characterName;
        characterData.adsWatched = PlayerPrefs.GetInt(key + "_adsWatched", 0);
        characterData.isUnlocked = PlayerPrefs.GetInt(key + "_unlocked", 0) == 1;
    }
}
