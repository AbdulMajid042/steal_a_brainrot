using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyThisCharacter : MonoBehaviour
{
    public string characterName;
    public int adsRequired = 10;   // How many ads needed
    public int adsWatched = 0;     // Current progress
    public int inAppprice;              // Optional: for IAP
    public int income;
    public string rarity;
    private int frameCounter = 0;
    private Camera targetCamera;
    public Transform infoTransform;
    public string inAppString;

    void Start()
    {
        if (PlayerPrefs.GetInt(characterName) == 1)
        {
            Unlocked();
        }
        GetComponent<Brainrot>().SetStat();
        infoTransform = GetComponent<Brainrot>().infoTransform;
    }
    private void Update()
    {
        if(targetCamera==null)
        targetCamera = Camera.main;
        frameCounter++;
        if (frameCounter % 10 == 0 && targetCamera)
        {
            infoTransform.LookAt(targetCamera.transform);
        }
    }
    public void Unlocked()
    {
        gameObject.GetComponent<Brainrot>().PlaceToAvailablePlace();
        gameObject.GetComponent<Brainrot>().triggerCollider.SetActive(false);
        gameObject.GetComponent<Brainrot>().isBought=true;
    }
}
