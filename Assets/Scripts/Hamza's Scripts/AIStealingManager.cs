using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIStealingManager : MonoBehaviour
{
    public static AIStealingManager instance;

    [Header("Debug")]
    public List<Brainrot> houseObjects = new List<Brainrot>();

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        // Delay the scan to ensure all Brainrot objects are fully initialized
        StartCoroutine(DelayedRefresh());
    }

    IEnumerator DelayedRefresh()
    {
        yield return new WaitForSeconds(0.5f);
        RefreshHouseObjectList();
        Debug.Log($"[AIStealingManager] Found {houseObjects.Count} house objects.");
    }

    public void RefreshHouseObjectList()
    {
        houseObjects.Clear();
        Debug.Log("List Clear");
        Brainrot[] allObjects = FindObjectsOfType<Brainrot>(true); // <- 'true' finds even inactive ones

        foreach (var obj in allObjects)
        {
            if ((obj.isBought || obj.isStolen) && obj.sold == false)
            {
                houseObjects.Add(obj);
            }
        }

        // Sort by priceValue in descending order (highest first)
        houseObjects = houseObjects
            .OrderByDescending(o => o.priceValue)
            .ToList();
    }

    public Brainrot GetRandomStealableObject()
    {
        List<Brainrot> stealable = houseObjects.FindAll(o => o.isBought && !o.isStolen);

        if (stealable.Count == 0) return null;

        return stealable[Random.Range(0, stealable.Count)];
    }

    /// <summary>
    /// Gets the most expensive available object that has not been stolen yet.
    /// </summary>
    public Brainrot GetHighestValueStealableObject()
    {
        return houseObjects.FirstOrDefault(o => o.isBought && !o.isStolen);
    }

    public void MarkAsStolen(Brainrot obj)
    {
        obj.isStolen = true;
    }
    public void AIStealBrainrot(Brainrot target, Transform aiCarryPoint, string name)
    {
        if (target == null) return;

        // Disable collider so player can't steal it after AI does
        // Collider col = target.GetComponentInChildren<Collider>();
        // if (col) col.enabled = false;

        // Mark as stolen so it's removed from stealable pool
        target.SellCharacter();
        target.isStolenbyAI = true;
        target.isStolen = false;
        target.isBought = false;
        target.isAI = true;
        target.inmyhouse = false;
        target.moneyGeneratedCollider.SetActive(false);
        target.triggerCollider.SetActive(true);
        target.EnableTriggerCollider(true);
        // Player.instance.stolen.GetComponentInChildren<Collider>().enabled = true;
        //target.gameObject.GetComponent<Brainrot>().enabled = false;
        // Parent under AI's carrying point
        target.transform.parent = aiCarryPoint;
        target.transform.localPosition = Vector3.zero;
        target.transform.localEulerAngles = Vector3.zero;
        // Refresh list so other AIs don't target this object anymore
        RefreshHouseObjectList();

        if (UIManager.instance)
        {
            UIManager.instance.stolenText.gameObject.SetActive(false);
            UIManager.instance.stolenText.gameObject.SetActive(true);
            UIManager.instance.stolenText.text = name + " stole your " + "<color=red>" + target.name + "</color>";
        }
        Debug.Log($"[AIStealingManager] AI stole object: {target.name}");
    }
}
