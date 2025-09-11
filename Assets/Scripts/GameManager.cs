using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Material daySky,nightSky;
    public Color dayColor,nightColor;
    public GameObject boyPlayer, girlPlayer;
    public Animator targetAnimator;   // Drag your Animator here
    public Avatar boyAvatar,girlAvatar;
    private void Awake()
    {
        if(PlayerPrefs.GetString("Avatar")=="Boy")
        {
            boyPlayer.SetActive(true);
            girlPlayer.SetActive(false);
            targetAnimator.avatar = boyAvatar;
        }
        else
        {
            boyPlayer.SetActive(false);
            girlPlayer.SetActive(true);
            targetAnimator.avatar = girlAvatar;
        }
    }
    private void Start()
    {
        if(PlayerPrefs.GetString("Time")=="Day")
        {
            RenderSettings.skybox = daySky;
            RenderSettings.ambientLight = dayColor;
        }
        else
        {
            RenderSettings.skybox = nightSky;
            RenderSettings.ambientLight = nightColor;
        }
    }
}
