using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Lock : MonoBehaviour
{
    public bool isFreeLock = false;
    public float fillRate;
    public GameObject circle;
    public GameObject parentObject;
    private Image circleImage;
    public TextMeshPro durationToShow;
    public int duration;
    private int startingTime;

    public GameObject freeLockObject;
    private void Start()
    {
        if (circle)
            circleImage = circle.GetComponent<Image>();
        startingTime = duration;
    }
    public void LockHouse()
    {
        StartCoroutine(FillCircle());
    }

    IEnumerator FillCircle()
    {
        circle.SetActive(true);
        circleImage.fillAmount = 0f;
        float elapsed = 0f;

        while (elapsed < fillRate)
        {
            elapsed += Time.deltaTime;
            circleImage.fillAmount = Mathf.Clamp01(elapsed / fillRate);
            yield return null;
        }
        circle.SetActive(false);
        if (isFreeLock)
            UnlockMethod();
        else
        {
            if(AdsManagerWrapper.Instance)
            {
                AdsManagerWrapper.Instance.ShowRewardedVideo(UnlockMethod);
            }
        }
    }
    public void UnlockMethod()
    {
        parentObject.SetActive(false);
        durationToShow.gameObject.SetActive(true);
        MyHouse.instance.myHouseLock.SetActive(true);
        GetComponent<Collider>().enabled = false;
        circle.SetActive(false);
        if (freeLockObject)
            freeLockObject.SetActive(false);
        Invoke("UnlockWithDelay", 2.0f);
    }
    void UnlockWithDelay()
    {
        parentObject.SetActive(false);
        durationToShow.gameObject.SetActive(true);
        MyHouse.instance.myHouseLock.SetActive(true);
        GetComponent<Collider>().enabled = false;
        circle.SetActive(false);
        if (freeLockObject)
            freeLockObject.SetActive(false);
        StartCoroutine(LockAgain());

    }

    IEnumerator LockAgain()
    {
        // Countdown
        while (duration > 0)
        {
            duration--;

            int minutes = duration / 60;
            int seconds = duration % 60;

            if (minutes > 0)
                durationToShow.text = "Locked\n\n" + $"{minutes} m {seconds} s";
            else
                durationToShow.text = "Locked\n\n" + $"{seconds} s";

            yield return new WaitForSeconds(1f); // update every second
        }
        // When duration ends
        duration = startingTime;
        if (!isFreeLock)
        {
            parentObject.SetActive(true);
            GetComponent<Collider>().enabled = true;
            MyHouse.instance.myHouseLock.SetActive(false);
            durationToShow.gameObject.SetActive(false);
            circleImage.fillAmount = 0f;
        }
        else
        {
            MyHouse.instance.myHouseLock.SetActive(false);
            gameObject.SetActive(false);
        }

    }
}
