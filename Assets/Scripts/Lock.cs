using System.Collections;
using TMPro;
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
            if (Ads_Manager.instance)
            {
                Ads_Manager.instance.ShowRewardedVideo(UnlockMethod);
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
    bool temp = true;
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

            if (minutes == 0 && seconds < 11)
            {
                if(temp)
                {
                    UIManager.instance.baseUnlcokingMessageText.gameObject.SetActive(false);
                    UIManager.instance.baseUnlcokingMessageText.gameObject.SetActive(true);
                    temp = false;
                }
                UIManager.instance.baseUnlcokingMessageText.text = "Your base is unlocking in " + "<color=red>" + seconds + " sec" + "</color>";
            }
            if (minutes==0 && seconds < 1)
            {
                UIManager.instance.baseUnlockedMessageText.gameObject.SetActive(false);
                UIManager.instance.baseUnlockedMessageText.gameObject.SetActive(true);
                UIManager.instance.baseUnlockedMessageText.text = "Your base is " + "<color=red>" + "Unlocked!" + "</color>";
            }

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
        Invoke("MakeTempTrue", 10f);
    }

    void MakeTempTrue()
    {
        temp = true;
    }
}
