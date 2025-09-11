using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    GameObject tempBuyButton;
    public GameObject currentBrainrot,stolen;
    public Transform playerInitialTransform;
    public Transform stolenBrainrotsTransform;

    public bool carriedBrainrot = false;

    public AIHouse aiHouse;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        PlayerPrefs.SetInt("LastReached", 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Brainrot")
        {
            if (UIManager.instance)
            {
                UIManager.instance.buyBrainrot.SetActive(true);
                tempBuyButton = UIManager.instance.buyBrainrot;
                Player.instance.currentBrainrot = other.GetComponentInParent<Brainrot>().gameObject;
                SetPriceToBuyThisBrainrot(other.transform);
            }
        }
        if (other.gameObject.tag == "SellTrigger")
        {
            if (UIManager.instance)
            {
                if (other.GetComponentInParent<Brainrot>().isGeneratingMoney)
                    UIManager.instance.sell.SetActive(true);
                if (other.GetComponentInParent<Brainrot>().generatedMoney > other.GetComponentInParent<Brainrot>().moneyGenerationValue * 5)
                {
                    UIManager.instance.coinAttraction.SetActive(false);
                    UIManager.instance.coinAttraction.SetActive(true);
                    PriceManager.instance.SetCurrency(other.GetComponentInParent<Brainrot>().generatedMoney);
                    other.GetComponentInParent<Brainrot>().generatedMoney = 0;
                }

                Player.instance.currentBrainrot = other.GetComponentInParent<Brainrot>().gameObject;
            }
        }
        if(other.gameObject.tag=="Lock")
        {
            other.gameObject.GetComponent<Lock>().LockHouse();
        }
        if (other.gameObject.tag == "StealTrigger")
        {
            if (UIManager.instance && carriedBrainrot==false)
            {
                UIManager.instance.steal.SetActive(true);
                stolen = other.GetComponentInParent<Brainrot>().gameObject;
            }
        }
        if (other.gameObject.tag == "StealCollectionCollider")
        {
           if(carriedBrainrot==true)
            {
                if(stolen)
                {
                    stolen.transform.parent= null;
                    stolen.GetComponent<Brainrot>().PlaceToAvailablePlace();
                }
                carriedBrainrot = false;
            }
        }
        if (other.gameObject.tag == "UnlockHouseCollider")
        {
            if (UIManager.instance)
            {
                UIManager.instance.unlockHouse.SetActive(true);
                aiHouse = other.GetComponentInParent<AIHouse>();
            }
        }
    }
    void SetPriceToBuyThisBrainrot(Transform collider)
    {
        if (PriceManager.instance)
        {
            PriceManager.instance.brainrotPrice = collider.GetComponentInParent<Brainrot>().priceValue;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Brainrot")
        {
            Invoke("DisableBuyButtonWithDelay", 1.0f);
        }

        if (other.gameObject.tag == "SellTrigger")
        {
            if (UIManager.instance)
            {
                UIManager.instance.sell.SetActive(false);
            }
        }
        if (other.gameObject.tag == "StealTrigger")
        {
            if (UIManager.instance)
            {
                UIManager.instance.steal.SetActive(false);
            }
        }
        if (other.gameObject.tag == "UnlockHouseCollider")
        {
            if (UIManager.instance)
            {
                UIManager.instance.unlockHouse.SetActive(false);
            }
        }
    }
    void DisableBuyButtonWithDelay()
    {
        if (tempBuyButton)
        {
            tempBuyButton.SetActive(false);
        }
    }
    public void BackHouse()
    {
        transform.position = playerInitialTransform.position;
        transform.eulerAngles = playerInitialTransform.eulerAngles;
    }
}
