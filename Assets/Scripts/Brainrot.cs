using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Brainrot : MonoBehaviour
{
    [Header("           Stats")]
    public bool showDummyValue;
    public string dummyPrice, dummyMoneyGenerationValue;
    public Transform infoTransform;
    public string characterName;
    public int priceValue, moneyGenerationValue;

    [Header("           Text Fields")]
    public TextMeshPro nameText;
    public TextMeshPro priceText, moneyGenerationText,generatedMoneyText;


    [Header("           Movement")]
    public float moveSpeed = 2f;
    private Transform pointA;
    private Transform pointB;
    public Transform pointC; // assign in Inspector for bought destination
    Vector3 originalPosition;

    [Header("           Misc. References")]

    public GameObject triggerCollider,moneyGeneratedCollider;
    public GameObject particle;
    private Transform currentTarget;
    private Animator animator;
    private int frameCounter = 0;
    private Camera targetCamera;

    public bool isBought = false;   // flag for purchase
    private bool reachedPointC = false;
    public bool isGeneratingMoney = false;

    public int generatedMoney = 0;

    public int spotNumber = -1;
    public bool isLast;

    [Header("           AI References")]

    public bool isAI;
    public bool isStolen;
    private void Start()
    {
    //    moveSpeed = 30;
        SetStat();
        SetPoints();
        SetCamera();
        if(particle)
        {
            particle.SetActive(true);
        }
        Invoke("SaveStateMethod", 1.0f);
    }
    void SaveStateMethod()
    {
        if (isBought || isStolen)
        {
            EnableTriggerCollider(false);
            StartCoroutine(GenerateMoneyRoutine());
        }
    }

    private void Update()
    {
        #region for Not AI Brainrots
        if (!isAI)
        {
            if (!isBought)
            {
                MoveBetweenPoints(); // looping A  B
            }
            else if (!reachedPointC)
            {
                MoveToPointC(); // after purchase
            }

            frameCounter++;
            if (frameCounter % 10 == 0 && targetCamera)
            {
                infoTransform.LookAt(targetCamera.transform);
            }
        }
        else
        {
            if (animator)
                animator.SetBool("IsMove", false);
        }
        #endregion


        if (isGeneratingMoney)
        {
            generatedMoneyText.text = "$"+generatedMoney.ToString();
        }
    }
    public void PlaceToAvailablePlace()
    {
        if(isAI)
        {
            MyHouse.instance.TryReserveFirstFreePlace(out spotNumber, out pointC);
            transform.position = pointC.position;
            reachedPointC = true;

            if (animator)
                animator.SetBool("IsMove", false); // stop animation

            moneyGeneratedCollider.SetActive(true);
            StartGeneratingMoney();
        }
    }
    public void SetStat()
    {
        animator = GetComponent<Animator>();
        nameText.text = characterName;
        priceText.text = "Price: " + FormatNumber(priceValue) + "$";
        moneyGenerationText.text = FormatNumber(moneyGenerationValue) + "$/sec";

        if (showDummyValue)
        {
            priceText.text = dummyPrice;
            moneyGenerationText.text = dummyMoneyGenerationValue;
        }
    }
    string FormatNumber(long value)
    {
        if (value >= 1_000_000_000_000) // Trillions
            return (value / 1_000_000_000_000f).ToString("0.0") + "t";
        else if (value >= 1_000_000_000) // Billions
            return (value / 1_000_000_000f).ToString("0.0") + "b";
        else if (value >= 1_000_000) // Millions
            return (value / 1_000_000f).ToString("0.0") + "m";
        else if (value >= 1_000) // Thousands
            return (value / 1_000f).ToString("0.0") + "k";
        else
            return value.ToString();
    }

    void SetPoints()
    {
     //   pointA = GameObject.Find("pointA").transform;
        originalPosition = transform.position;
        pointB = GameObject.Find("pointB").transform;
    //    transform.position = pointA.position;
        currentTarget = pointB;
    }

    void SetCamera()
    {
        targetCamera = Camera.main;
    }

    void MoveBetweenPoints()
    {
        if (isGeneratingMoney)
            return;
        if (pointB == null) return;

        if (animator)
            animator.SetBool("IsMove", true);

        // Move towards pointB
        transform.position = Vector3.MoveTowards(
            transform.position,
            currentTarget.position,
            moveSpeed * Time.deltaTime
        );

        // If reached pointB, teleport back to original position
        if (Vector3.Distance(transform.position, pointB.position) < 0.05f)
        {
            if (isLast)
                PlayerPrefs.SetInt("LastReached",1);
            if (PlayerPrefs.GetInt("LastReached")==1)
            {
                transform.position = originalPosition;
            }
        }
    }
    void MoveToPointC()
    {
        if (pointC == null) return;

        if (animator)
            animator.SetBool("IsMove", true);

        transform.position = Vector3.MoveTowards(
            transform.position,
            pointC.position,
            (moveSpeed * 2) * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, pointC.position) < 0.05f)
        {
            transform.position = pointC.position;
            reachedPointC = true;

            if (animator)
                animator.SetBool("IsMove", false); // stop animation

            StartGeneratingMoney();
        }
        else
        {
            transform.LookAt(pointC.position);
        }
    }

    public void BuyCharacter()
    {
        // Try to reserve a spot and set pointC to that spot immediately
        if (MyHouse.instance != null &&
            MyHouse.instance.TryReserveFirstFreePlace(out spotNumber, out pointC))
        {
            isBought = true;            // will start moving to pointC in Update
            reachedPointC = false;

            if (particle) particle.SetActive(false);
        }
    }
    public void SellCharacter()
    {
        // stop money generation and free the reserved spot
        isGeneratingMoney = false;

        if (MyHouse.instance != null)
            MyHouse.instance.ReleasePlace(spotNumber);

        spotNumber = -1;
        reachedPointC = false;
    }

    public void EnableTriggerCollider(bool status)
    {
        triggerCollider.SetActive(status);
        moneyGeneratedCollider.SetActive(!status);
    }
    void StartGeneratingMoney()
    {
        transform.localEulerAngles = pointC.eulerAngles;
        if (!isGeneratingMoney) // prevent multiple coroutines
        {
            isGeneratingMoney = true;
            StartCoroutine(GenerateMoneyRoutine());
        }
    }
    IEnumerator GenerateMoneyRoutine()
    {
        while (isGeneratingMoney)
        {
            generatedMoney = generatedMoney + moneyGenerationValue;
            PriceManager.instance.generatedMoneyToCollect = generatedMoney;
            // wait 1 second before next generation
            yield return new WaitForSeconds(1f);
        }
    }
}
