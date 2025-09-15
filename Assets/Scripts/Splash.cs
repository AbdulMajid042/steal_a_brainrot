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
        if(PlayerPrefs.GetInt("FirseSession")==0)
        {
        //    PlayerPrefs.SetInt("PlayerCurrency", 1000);
            PlayerPrefs.SetInt("FirseSession", 1);

        }
    }
    void LoadScene()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }
}
