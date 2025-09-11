using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMediumBanner : MonoBehaviour
{
    private void OnEnable()
    {
        if (AdsManagerWrapper.Instance)
        {
            AdsManagerWrapper.Instance.ShowMediumBanner();
        }
    }
    private void OnDisable()
    {
        if (AdsManagerWrapper.Instance)
        {
            AdsManagerWrapper.Instance.HideMediumBanner();
        }
    }
}
