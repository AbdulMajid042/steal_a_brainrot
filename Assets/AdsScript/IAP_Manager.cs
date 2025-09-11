using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine.Purchasing.Extension;
using UnityEngine.SceneManagement;
using Firebase.Sample.Analytics;
//using Demo_Project;

#region InAppStructure
public enum InAppItemName
{
    Coins,
    RemoveAds,
    Unlock_All_Levels,
    Unlock_All_Guns,
    Unlock_Everything,
    Gun
}

[System.Serializable]
public class ConsumableInApps
{
    public string inAppName, inAppId;

    public int quantityToGive;

    //public int priceForInApp;
    public InAppItemName inAppType = InAppItemName.Coins;
}

[System.Serializable]
public class NonConsumeableInApps
{
    public string inAppName;
    public string inAppId;
    public InAppItemName inAppType = InAppItemName.RemoveAds;
}

#endregion

public class IAP_Manager : MonoBehaviour, IDetailedStoreListener
{
    private static IStoreController m_StoreController; // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.


    public List<ConsumableInApps> consumeableInApps;
    public List<NonConsumeableInApps> nonConsumeableInApps;

    //public AcknowledgementPanel acknowledgementPanel;
    public string environment = "production";

    public static IAP_Manager Instance;

    
#if UNITY_ANDROID
    private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";
#endif

    public void OnPurchaseComplete(string id)
    {
        Debug.LogError("Coins have been added successfully");
    }

    public void OnPurchaseFailed()
    {
        Debug.LogError("Purchase Failed");
    }

    public void RemoveAdsPurchased()
    {
        Debug.LogError("Remove Ads purchased!");
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);    
        Instance = this;
    }

    private void OnEnable()
    {
        Instance = this;
    }

    async void InitGamingService()
    {
        try
        {
            var options = new InitializationOptions()
                .SetEnvironmentName(environment);

            await UnityServices.InitializeAsync(options);
        }
        catch (Exception exception)
        {
#if UNITY_EDITOR
            Debug.LogException(exception);
#endif
            // An error occurred during initialization.
        }
    }

    void Start()
    {
        Instance = this;

        InitGamingService();
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }

        //        DontDestroyOnLoad(this.gameObject);
    }

    public void UpdatePricesCoins(int bundleIndex, UnityEngine.UI.Text _text)
    {
        if (IsInitialized())
        {
            _text.text = "BUY " + m_StoreController.products.WithID(consumeableInApps[bundleIndex].inAppId).metadata.localizedPriceString;
            //price_Bundle_02.text =
            //    m_StoreController.products.WithID("purchase_bundle_2").metadata.localizedPriceString;
        }
    }

    public void UpdatePricesGuns(int bundleIndex, UnityEngine.UI.Text text)
    {
        if (IsInitialized())
        {
            text.text = "BUY " + m_StoreController.products.WithID(nonConsumeableInApps[bundleIndex].inAppId).metadata.localizedPriceString;
        }
    }

    public void UpdatePricesRemoveAds(UnityEngine.UI.Text text)
    {
        if (IsInitialized())
        {
            text.text = "BUY " + m_StoreController.products.WithID(nonConsumeableInApps[0].inAppId).metadata.localizedPriceString;
        }
    }

    public void UpdatePricesAllGuns( UnityEngine.UI.Text text)
    {
        if (IsInitialized())
        {
            text.text = "BUY " + m_StoreController.products.WithID(nonConsumeableInApps[1].inAppId).metadata.localizedPriceString;
        }
    }

    public void UpdatePricesAllLevels(UnityEngine.UI.Text text)
    {
        if (IsInitialized())
        {
            text.text = "BUY " + m_StoreController.products.WithID(nonConsumeableInApps[2].inAppId).metadata.localizedPriceString;
        }
    }
    public void UpdatePricesEverything(UnityEngine.UI.Text text)
    {
        if (IsInitialized())
        {
            text.text = "UNLOCK EVERYTHING IN " + m_StoreController.products.WithID(nonConsumeableInApps[3].inAppId).metadata.localizedPriceString;
        }
    }

    //    void Update()
    //    {
    //        UpdatePrices();
    //    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var consumable in consumeableInApps)
        {
            builder.AddProduct(consumable.inAppId, UnityEngine.Purchasing.ProductType.Consumable);
        }

        foreach (var nonConsumable in nonConsumeableInApps)
        {
            builder.AddProduct(nonConsumable.inAppId, UnityEngine.Purchasing.ProductType.NonConsumable);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void Coins(int bundleNumber)
    {
        BuyProductID(consumeableInApps[bundleNumber].inAppId);
        //MainMenuManager.instance.ButtonClickSound();
    }

    public void RemoveAds()
    {
        //BuyProductID("removeads");
        BuyProductID(nonConsumeableInApps[0].inAppId);
    }

    public void Unlock_All_Guns()
    {
        //BuyProductID("unlockguns");
        BuyProductID(nonConsumeableInApps[1].inAppId);
    }

    public void Unlock_All_Levels()
    {
        //BuyProductID("unlocklevels");
        BuyProductID(nonConsumeableInApps[2].inAppId);
    }

    public void Unlock_Everything()
    {
        //BuyProductID("unlockeverything");
        BuyProductID(nonConsumeableInApps[3].inAppId);
    }

    int GunNumber = 0;
    public void Unlock_GunByIndex(int gunNumber)
    {
        GunNumber = gunNumber;
        //BuyProductID("unlockeverything");
        BuyProductID(nonConsumeableInApps[2].inAppId);
    }

    public void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log(
                    "BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        ConsumableInApps consumableSucceededInApp =
            consumeableInApps.Find(x => x.inAppId == args.purchasedProduct.definition.id);

        // A consumable product has been purchased by this user.
        if (consumableSucceededInApp != null)
        {
            if (consumableSucceededInApp.inAppType == InAppItemName.Coins)
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                
                for(int i = 0; i < consumeableInApps.Count; i++)
                {
                    if(args.purchasedProduct.definition.id == consumeableInApps[i].inAppId)
                    {
                        //int coins = PlayerPrefs.GetInt(PlayerPrefStrings.Coins, 0);
                        //coins += consumeableInApps[i].quantityToGive;
                        //PlayerPrefs.SetInt(PlayerPrefStrings.Coins, coins);
                        //MainMenuManager.instance.CoinRewardNotification("Congratulations! You have purchased " + consumeableInApps[i].quantityToGive + " coins.");
                        //MainMenuManager.instance.UpdateCoins()/*;*/
                    }
                }
            }
        }

        // Or ... a non-consumable product has been purchased by this user.
        else
        {
            NonConsumeableInApps nonConsumableSucceededInapp =
                nonConsumeableInApps.Find(x => x.inAppId == args.purchasedProduct.definition.id);
            if (nonConsumableSucceededInapp != null)
            {
                if (nonConsumableSucceededInapp.inAppType == InAppItemName.RemoveAds)
                {
                    if (GRS_FirebaseHandler.Instance) 
                        GRS_FirebaseHandler.Instance.LogIAPPurchased("RemoveAds");
                    iapNumber = 1;
                    WaitIAP();
                }
                else if (nonConsumableSucceededInapp.inAppType == InAppItemName.Gun)
                {
                    //Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                    //// unlock Gun Here
                    //if (GRS_FirebaseHandler.Instance) GRS_FirebaseHandler.LogIAPPurchased("UnlockGun", GunUnlockManager.instance.allGuns.allGuns[GunNumber].GunName);

                    //iapNumber = 3;
                    //WaitIAP();
                    ////MainMenuManager.instance.CoinRewardNotification("Congratulations! You have purchased the gun.");
                    ////PlayerPrefs.SetInt(PlayerPrefStrings.GunNO + GunNumber, 1);
                }
            }
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
            product.definition.storeSpecificId, failureReason));
    }

    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result, info) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result +
                          ". If no further messages, no purchases available to restore. Info: " + info);
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    int iapNumber = 0;

    private void WaitIAP()
    {
        switch (iapNumber)
        {
            case 1:
                Time.timeScale = 1;
                if(GameObject.Find("LikeAdsPanel"))
                    GameObject.Find("LikeAdsPanel").SetActive(false);
                //// remove ads
                //PlayerPrefManager.Instance.RemoveAds();
                PlayerPrefs.SetInt("RemoveAds", 1);
                AdsManagerWrapper.Instance.HideSmallBannerEvent();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                //if (GamePlay_GUI_Handler.instance) GamePlay_GUI_Handler.instance.OnRemoveAds();
                //->AdmobAdsManager.IsInitilized = false;
                //FindFirstObjectByType<AcknowledgementPanel>().ShowAcknowledgement(AcknowledgementPanel.AcknowledgementType.RemoveAds);
                break;
            case 2:
                // Unlock Everything
                //PlayerPrefs.SetInt("RemoveAds", 1);
                PlayerPrefs.SetInt("UnlockEverything", 1);
                PlayerPrefs.SetInt("RemoveAds", 1);

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                //PlayerPrefManager.Instance.UnlockAllGuns();
                //FindFirstObjectByType<AcknowledgementPanel>().ShowAcknowledgement(AcknowledgementPanel.AcknowledgementType.UnlockAllGuns);
                break;
            case 3:

                Debug.Log("Gun Unlocked by IAP");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                // all levels
                //PlayerPrefManager.Instance.UnlockAllLevels();
                //FindFirstObjectByType<AcknowledgementPanel>().ShowAcknowledgement(AcknowledgementPanel.AcknowledgementType.UnlockLevels);
                break;
            case 4:
                // everything
                //PlayerPrefManager.Instance.UnlockEveryThing();
                //->AdmobAdsManager.IsInitilized = false;
                //FindFirstObjectByType<AcknowledgementPanel>().ShowAcknowledgement(AcknowledgementPanel.AcknowledgementType.UnlockEveryThing);
                break;
            case 5:
                // Coins
                break;
            default:
                break;
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("Resion: " + error + "( Message:) " + message);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
            product.definition.storeSpecificId, failureDescription));
    }
}