using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    public string sceneNameToLoad;
    public float delay;
    private void Start()
    {
        Invoke("LoadScene", delay);

        PlayerPrefs.SetFloat("SoundVolume", 1f);
        PlayerPrefs.SetFloat("MusicVolume", 1f);
        PlayerPrefs.SetFloat("Sensitivity", 0.5f);
        PlayerPrefs.SetInt("LastReached", 0);
        if (PlayerPrefs.GetInt("FirseSession")==0)
        {
            RCC_PlayerPrefsX.SetLong("PlayerCurrency", 1000);
            PlayerPrefs.SetInt("FirseSession", 1);
        }
        Invoke("ShowAppOpen", 1.0f);
    }
    void LoadScene()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }
    void ShowAppOpen()
    {
        if(AdmobeAdsManager.instance)
        {
                AdmobeAdsManager.instance.ShowAppOpenAdAtSplash();
        }
        if(AdmobeAdsManager.instance.isSplashAdShown==false)
            Invoke("ShowAppOpen", 1.0f);
    }
}
