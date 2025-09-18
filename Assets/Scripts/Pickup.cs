using System.Collections;
using TMPro;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public string pickupName;
    public float timeToShowVideo = 5f;
    public TextMeshPro timeText;

    private float currentTime;
    private bool playerInside = false;
    private Coroutine countdownCoroutine;
    public GameObject parentObject;

    private void Start()
    {
        currentTime = timeToShowVideo;
        if(PlayerPrefs.GetInt("ItemUnlocked_" + pickupName)==1)
        {
            Invoke("GiveReward", 2.5f);
        }
    //    UpdateTimeText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            // Start countdown only once
            if (countdownCoroutine == null)
                countdownCoroutine = StartCoroutine(StartCountdown());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            // Stop countdown and reset timer
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
            }

            currentTime = timeToShowVideo;
            if (timeText != null)
            {
                timeText.text = "";
            }
        }
    }

    private IEnumerator StartCountdown()
    {
        currentTime = timeToShowVideo;

        while (currentTime > 0 && playerInside)
        {
            currentTime -= Time.deltaTime;
            UpdateTimeText();
            yield return null;
        }

        if (playerInside && currentTime <= 0)
        {
            ShowRewardedVideo();
        }

        countdownCoroutine = null; // reset reference
    }

    private void UpdateTimeText()
    {
        if (timeText != null)
        {
            timeText.text = Mathf.Ceil(currentTime).ToString();
        }
    }

    private void ShowRewardedVideo()
    {
        if(Ads_Manager.instance)
        {
            Ads_Manager.instance.ShowRewardedVideo(GiveReward);
        }
    }
    void GiveReward()
    {
        parentObject.SetActive(false);
        FindObjectOfType<PlayerInventory>(true).UnlockItem(pickupName);
    }
}
