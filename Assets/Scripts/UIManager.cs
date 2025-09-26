using Firebase.Sample.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
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
    public Text stealingText, stolenText,baseUnlcokingMessageText,baseUnlockedMessageText;

    [Header("           Ads Panel. References")]
    public GameObject likeAdsPanel;
    public GameObject bonusRewardPanel;
    public GameObject characterUnlockPanel;
    public GameObject boyCollection, girlCollection;
    public Text adDurationText;

    Ads_Manager ads;
    public string sceneNameToLoad;
    public float delay;

    public int idleAdTime;
    private int startingTime;
    private int counter = 1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        PlayerPrefs.SetInt("Placed", 0);
    }
    private void Start()
    {
        if (Ads_Manager.instance)
            ads = Ads_Manager.instance;

        if (ads)
        {
            ads.HideMediumBanner();
        }

        startingTime = idleAdTime;
        Invoke("DecreaseTime", 1.0f);
        UpdateAdText();
        BindAllButtons();
        PlayerPrefs.SetInt("SessionNumber", PlayerPrefs.GetInt("SessionNumber") + 1);
    }

    public void OpenOutfits()
    {

        if (PlayerPrefs.GetString("Avatar") == "Boy")
        {
            boyCollection.SetActive(true);

        }
        else
        {
            girlCollection.SetActive(true);
        }
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
                    long temp = PriceManager.instance.GetPlayerCurrency() - Player.instance.currentBrainrot.GetComponent<Brainrot>().priceValue;
                    PriceManager.instance.SetCurrencyAfterBuy(temp);
                }
                if(AudioManager.instance)
                {
                    AudioManager.instance.PlayGenericSound(AudioManager.instance.positiveSfx);
                }

                AIStealingManager.instance.RefreshHouseObjectList();   //yo
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
        brainrot.sold = true; //yo
        PriceManager.instance.SetCurrency(brainrot.priceValue);
        brainrot.SellCharacter();                // frees the spot + stops generation
        Player.instance.currentBrainrot.SetActive(false);
        sell.SetActive(false);
    }

    public void StealBrainrot()
    {
        steal.SetActive(false);
        GameObject stolen = Player.instance.stolen;
        Player.instance.carriedBrainrot = true;
        stolen.GetComponentInChildren<Collider>().enabled = false;
        stolen.GetComponentInChildren<Brainrot>().isStolen = true;
        stolen.transform.parent = Player.instance.stolenBrainrotsTransform;
        stolen.transform.localPosition = Vector3.zero;
        stolen.transform.localEulerAngles = Vector3.zero;
    }

    public void StealBrainFromAI(GameObject Obj)  //yo
    {
        Player.instance.carriedBrainrot = true;
        Obj.GetComponentInChildren<Collider>().enabled = false;
        Obj.GetComponentInChildren<Brainrot>().isStolen = true;
        Obj.transform.parent = Player.instance.stolenBrainrotsTransform;
        Obj.transform.localPosition = Vector3.zero;
        Obj.transform.localEulerAngles = Vector3.zero;
        Obj.GetComponent<Brainrot>().isStolenbyAI = false;
        Obj.GetComponentInChildren<Brainrot>().PlayerTookBack = true;
        Obj.GetComponent<StealrotAIExtension>().DropCarriedObject();
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
        RCC_PlayerPrefsX.SetLong("PlayerCurrency", RCC_PlayerPrefsX.GetLong("PlayerCurrency") + 500);
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
    private void OnApplicationQuit()
    {
        GameObject.FindObjectOfType<ES3AutoSaveMgr>().Save();
    }
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            GameObject.FindObjectOfType<ES3AutoSaveMgr>().Save();
        }
    }

    void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            GameObject.FindObjectOfType<ES3AutoSaveMgr>().Save();
        }
    }
}
