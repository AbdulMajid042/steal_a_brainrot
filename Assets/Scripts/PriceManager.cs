using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriceManager : MonoBehaviour
{
    public static PriceManager instance;
    public int brainrotPrice;
    int playerCurrency;
    public int generatedMoneyToCollect;
    public Text playerTotalCurrency;
    [Header("           Dummy Reward")]
    public bool giveDummyReward;
    public int dummyReward;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        if(giveDummyReward)
        {
            PlayerPrefs.SetInt("PlayerCurrency", dummyReward);
        }
    }
    private void Update()
    {
        playerTotalCurrency.text ="$ "+ GetPlayerCurrency().ToString();
    }
    public void SetCurrency(int currency)
    {
        PlayerPrefs.SetInt("PlayerCurrency", PlayerPrefs.GetInt("PlayerCurrency")+currency);
    }
    public void SetCurrencyAfterBuy(int currency)
    {
        PlayerPrefs.SetInt("PlayerCurrency", currency);
    }
    public int GetPlayerCurrency()
    {
        playerCurrency = PlayerPrefs.GetInt("PlayerCurrency");
        return playerCurrency;
    }
}
