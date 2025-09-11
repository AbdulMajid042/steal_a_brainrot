using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Menu : MonoBehaviour
{
    public GameObject coinAttraction;
    AdsManagerWrapper ads;
    public Text playerTotalCurrency;
    public string sceneNameToLoad;
    public float delay;
    public GameObject dailyRewardPanel;
    private void Start()
    {
        if(PlayerPrefs.GetInt("hello")==0)
        {
            PlayerPrefs.SetInt("hello", 1);
            dailyRewardPanel.SetActive(true);
        }
        if (AdsManagerWrapper.Instance)
            ads = AdsManagerWrapper.Instance;

        if (ads)
        {
            ads.HideMediumBanner();
            ads.ShowSmallBanner();
        }
        BindAllButtons();
    }
    void BindAllButtons()
    {
        Button[] allButtons = FindObjectsOfType<Button>(true);
        foreach (Button btn in allButtons)
        {
            // Clear old listeners (optional, if you want to avoid duplicates)
            // btn.onClick.RemoveAllListeners();

            // Add ClickSound listener
            btn.onClick.AddListener(ClickSound);
        }
    }
    void ClickSound()
    {
        if(AudioManager.instance)
        {
            AudioManager.instance.PlayClickSound();
        }
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
    private void Update()
    {
        playerTotalCurrency.text ="$ " + GetPlayerCurrency().ToString();
    }
    public int GetPlayerCurrency()
    {
        return PlayerPrefs.GetInt("PlayerCurrency");
    }
    public void OpenHyperlink(string url)
    {
        Application.OpenURL(url);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void ShowAd()
    {
        if (ads)
            ads.ShowInterstitial();
    }
    public void LoadGame()
    {
        Invoke("LoadScene", delay);
    }
    void LoadScene()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }

    public void SelectAvatar(string avatarName)
    {
        PlayerPrefs.SetString("Avatar", avatarName);
    }
    public void SelectTime(string timeName)
    {
        PlayerPrefs.SetString("Time", timeName);
    }
}

