using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Sample.Analytics;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [Header("           Button References")]
    public GameObject buyBrainrot;
    public GameObject sell;
    public GameObject steal;
    public GameObject unlockHouse;


    [Header("           Misc. References")]
    public GameObject notEnoughMoney;
    public GameObject coinAttraction;
    public Image lockCircle;

    [Header("           Ads Panel. References")]
    public GameObject likeAdsPanel;
    public GameObject bonusRewardPanel;
    public Text adDurationText;

    AdsManagerWrapper ads;
    public string sceneNameToLoad;
    public float delay;

    public int idleAdTime;
    private int startingTime;
    private int counter = 1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        if (AdsManagerWrapper.Instance)
            ads = AdsManagerWrapper.Instance;

        if (ads)
            ads.HideMediumBanner();

        startingTime = idleAdTime;
        Invoke("DecreaseTime", 1.0f);
        UpdateAdText();
        BindAllButtons();
        if (GRS_FirebaseHandler.Instance)
            GRS_FirebaseHandler.Instance.LogIAPPurchased("GameStart");
    }

    void BindAllButtons()
    {
        Button[] allButtons = FindObjectsOfType<Button>(true);
        foreach (Button btn in allButtons)
        {
            // Clear old listeners (optional, if you want to avoid duplicates)
            // btn.onClick.RemoveAllListeners();

            // Add ClickSound listener
            if(btn.transform.parent.gameObject.name!= "InvectorUI")
                btn.onClick.AddListener(ClickSound);
        }
    }
    void ClickSound()
    {
        if (AudioManager.instance)
        {
            AudioManager.instance.PlayClickSound();
        }
    }
    void UpdateAdText()
    {
        int minutes = idleAdTime / 60;
        int seconds = idleAdTime % 60;
        adDurationText.text = "AD IN : " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    void DecreaseTime()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1)
            return;
        idleAdTime--;
        if (idleAdTime == 0)
        {
            counter++;
            if (counter % 2 == 0)
            {
                likeAdsPanel.SetActive(true);
            }
            else
            {
                bonusRewardPanel.SetActive(true);
            }
            SetTime(0);
        }
        else
        {
            Invoke("DecreaseTime", 1.0f);
        }
        UpdateAdText();
    }
    public void ResetIdleTime()
    {
        SetTime(1);
        idleAdTime = startingTime;
        Invoke("DecreaseTime", 1.0f);
    }
    public void BuyBrainrot()
    {
        if (PriceManager.instance)
        {
            if (isPlayerCurrencyIsEnough())
            {
                if (Player.instance.currentBrainrot)
                {
                    Player.instance.currentBrainrot.GetComponent<Brainrot>().BuyCharacter();
                    Player.instance.currentBrainrot.GetComponent<Brainrot>().EnableTriggerCollider(false);
                    buyBrainrot.SetActive(false);
                    int temp = PriceManager.instance.GetPlayerCurrency() - Player.instance.currentBrainrot.GetComponent<Brainrot>().priceValue;
                    PriceManager.instance.SetCurrencyAfterBuy(temp);
                }
                if(AudioManager.instance)
                {
                    AudioManager.instance.PlayGenericSound(AudioManager.instance.positiveSfx);
                }
            }
            else
            {
                notEnoughMoney.SetActive(false);
                notEnoughMoney.SetActive(true);
                if (AudioManager.instance)
                {
                    AudioManager.instance.PlayGenericSound(AudioManager.instance.negativeSfx);
                }
            }
        }
    }
    public void SellBrainrot()
    {
        var brainrot = Player.instance.currentBrainrot.GetComponent<Brainrot>();

        PriceManager.instance.SetCurrency(brainrot.priceValue);

        brainrot.SellCharacter();                // frees the spot + stops generation
        Player.instance.currentBrainrot.SetActive(false);
        sell.SetActive(false);
    }

    public void StealBrainrot()
    {
        steal.SetActive(false);
        Player.instance.carriedBrainrot = true;
        Player.instance.stolen.GetComponentInChildren<Collider>().enabled = false;
        Player.instance.stolen.GetComponentInChildren<Brainrot>().isStolen = true;
        Player.instance.stolen.transform.parent = Player.instance.stolenBrainrotsTransform;
        Player.instance.stolen.transform.localPosition = Vector3.zero;
        Player.instance.stolen.transform.localEulerAngles = Vector3.zero;
    }
    public void UnlockHouse()
    {
        if (ads)
        {
            ads.ShowRewardedVideo(UnlockNow);
        }
    }
    void UnlockNow()
    {
        if (Player.instance.aiHouse != null)
        {
            Player.instance.aiHouse.basedLock.SetActive(false);
            Player.instance.aiHouse.gameObject.SetActive(false);
            unlockHouse.SetActive(false);
        }
    }
    bool isPlayerCurrencyIsEnough()
    {
        if (PriceManager.instance.GetPlayerCurrency() >= PriceManager.instance.brainrotPrice)
        {
            return true;
        }
        return false;
    }

    public void ShowAd()
    {
        if (ads)
            ads.ShowInterstitial();
    }
    public void LoadMenu()
    {
        Invoke("LoadScene", delay);
    }
    void LoadScene()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }
    public void SetTime(float t)
    {
        Time.timeScale = t;
    }

    public void FreeCash()
    {
        if (ads)
        {
            ads.ShowRewardedVideo(GiveFreeCash);
        }
    }
    void GiveFreeCash()
    {
        PlayerPrefs.SetInt("PlayerCurrency", PlayerPrefs.GetInt("PlayerCurrency") + 200);
        coinAttraction.SetActive(false);
        coinAttraction.SetActive(true);
    }
    public void BackHouse()
    {
        if (Player.instance)
        {
            Player.instance.BackHouse();
        }
    }
}
