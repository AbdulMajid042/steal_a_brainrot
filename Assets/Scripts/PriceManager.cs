using UnityEngine;
using UnityEngine.UI;

public class PriceManager : MonoBehaviour
{
    public static PriceManager instance;
    public long brainrotPrice;
    long playerCurrency;
    public long generatedMoneyToCollect;
    public Text playerTotalCurrency;
    [Header("           Dummy Reward")]
    public bool giveDummyReward;
    public long dummyReward;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        if (giveDummyReward)
        {
            RCC_PlayerPrefsX.SetLong("PlayerCurrency", dummyReward);
        }

    }
    private void Update()
    {
        playerTotalCurrency.text = "$ " + RCC_PlayerPrefsX.FormatCurrency(GetPlayerCurrency()); 
    }
    public void SetCurrency(long currency)
    {
        RCC_PlayerPrefsX.SetLong("PlayerCurrency", RCC_PlayerPrefsX.GetLong("PlayerCurrency") + currency);
    }
    public void SetCurrencyAfterBuy(long currency)
    {
        RCC_PlayerPrefsX.SetLong("PlayerCurrency", currency);
    }
    public long GetPlayerCurrency()
    {
        playerCurrency = RCC_PlayerPrefsX.GetLong("PlayerCurrency");
        return playerCurrency;
    }
}
