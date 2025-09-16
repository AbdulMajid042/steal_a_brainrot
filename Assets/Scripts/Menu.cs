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
    public Text hackText;
    private void Start()
    {
        if (AdsManagerWrapper.Instance)
            ads = AdsManagerWrapper.Instance;

        if (ads)
        {
            ads.HideMediumBanner();
       //     ads.ShowSmallBanner();
        //    ads.ShowSmallBanner2();
        }
        BindAllButtons();
        PlayerPrefs.SetInt("Hack", 0);
    }

    void ShowingSmallBanner()
    {
        if(ads)
        {

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
        RCC_PlayerPrefsX.SetLong("PlayerCurrency", RCC_PlayerPrefsX.GetLong("PlayerCurrency") + 200);
        coinAttraction.SetActive(false);
        coinAttraction.SetActive(true);
    }
    private void Update()
    {
        playerTotalCurrency.text ="$ " + GetPlayerCurrency().ToString();
    }
    public long GetPlayerCurrency()
    {
        return RCC_PlayerPrefsX.GetLong("PlayerCurrency");
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
        {
            ads.ShowAdmobFirst();
        }
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
    int hack = 0;
    public void HackButtonTask()
    {
        hack++;
        hackText.text = hack.ToString();
        if(hack>=10)
        {
            PlayerPrefs.SetInt("Hack", 1);
        }
    }
}

