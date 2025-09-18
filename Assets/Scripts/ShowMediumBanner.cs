using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMediumBanner : MonoBehaviour
{
    private void OnEnable()
    {
        if (Ads_Manager.instance)
        {
            Ads_Manager.instance.ShowMediumBanner();
        }
    }
    private void OnDisable()
    {
        if (Ads_Manager.instance)
        {
            Ads_Manager.instance.HideMediumBanner();
        }
    }
}
