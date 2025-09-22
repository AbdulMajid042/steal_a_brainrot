using Invector.vCharacterController.AI;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Invector.vCharacterController.AI.vSimpleMeleeAI_Motor;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(vSimpleMeleeAI_Controller))]
public class StealrotAIExtension : MonoBehaviour
{
    private vSimpleMeleeAI_Controller ai;

    [Header("Player & House References")]
    public Transform player;
    public Lock playerHouseLock;
    public GameObject Lock; // The object that represents a locked door 

    [Header("AI Settings")]
    public float houseDetectionRadius = 15f;
    public float waitAtHouseTime = 5f;

    public bool goingToHouse;
    private float houseWaitTimer;
    public bool hasVisitedHouse;

    public float VisitTimeGap;

    [Header("Stealing Settings")]
    public Brainrot targetBrainrot;   // brainrot AI will steal
    public Transform carryPoint;      // where the AI will hold the stolen object
    public float stealDistance = 2f;  // how close AI needs to be to steal

    private bool hasStolen = false;
    public bool isCarrying = false;   // NEW: persistent check if AI is currently carrying an object
    public bool AIinsideHouse = false;

    void Start()
    {
        ai = GetComponent<vSimpleMeleeAI_Controller>();

        if (carryPoint.transform.childCount >= 1)
        {
            isCarrying = true;
        }
    }

    void Update()
    {
        if (ai == null) return;

        // If AI is already carrying something, skip stealing logic
        if (isCarrying)
        {
            if(GetComponent<vSimpleMeleeAI_Controller>())
            {
                GetComponent<vSimpleMeleeAI_Controller>().patrolSpeed = 7f;
                GetComponent<vSimpleMeleeAI_Controller>().wanderSpeed = 7f;
                GetComponent<vSimpleMeleeAI_Controller>().chaseSpeed = 7f;
                GetComponent<vSimpleMeleeAI_Controller>().strafeSpeed = 7f;
            }
            return;
        }
        else
        {
            GetComponent<vSimpleMeleeAI_Controller>().patrolSpeed = 3f;
            GetComponent<vSimpleMeleeAI_Controller>().wanderSpeed = 3f;
            GetComponent<vSimpleMeleeAI_Controller>().chaseSpeed = 3f;
            GetComponent<vSimpleMeleeAI_Controller>().strafeSpeed = 3f;
        }

        //  When going to house, keep re-applying target until timer ends
        if (goingToHouse && AIStealingManager.instance.houseObjects.Count > 0)
        {
            ai.SetCurrentTarget(AIStealingManager.instance.houseObjects[0].transform);
            ai.currentState = AIStates.Chase;

            // Check if AI is close enough to the house to steal
            if (!hasStolen && targetBrainrot != null && targetBrainrot.GetComponent<Brainrot>().inmyhouse)
            {
                float dist = Vector3.Distance(transform.position, targetBrainrot.transform.position);
                if (dist <= stealDistance && AIinsideHouse)
                {
                    PerformSteal();
                }
            }

            houseWaitTimer -= Time.deltaTime;
            if (houseWaitTimer <= 0f)
            {
                goingToHouse = false;
                ai.RemoveCurrentTarget();
                hasStolen = false; // reset for next time
                Invoke("HasVisitedReset", VisitTimeGap);
            }
            return;
        }

        //  Check if house is UNLOCKED and AI hasn't visited yet
        if (playerHouseLock != null && Lock != null)
        {
            if (!Lock.activeSelf && !hasVisitedHouse && (AIStealingManager.instance.houseObjects.Count > 0))
            {
                float dist = Vector3.Distance(transform.position, AIStealingManager.instance.houseObjects[0].transform.position);
                if (dist < houseDetectionRadius)
                {
                    GoToHouse();
                    hasVisitedHouse = true;
                }
            }
            else if (Lock.activeSelf)
            {
                //  Reset visit flag when house is locked again
                hasVisitedHouse = false;
            }
        }
    }
    bool temp = false;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("inside"))
        {
            AIinsideHouse = true;
            if (Lock.activeSelf)
            {
                if (GetComponent<vSimpleMeleeAI_Controller>())
                    GetComponent<vSimpleMeleeAI_Controller>().ThrowThisAI();
            }
            temp = true;
            if (UIManager.instance  && temp == false)
            {
                UIManager.instance.stealingText.gameObject.SetActive(false);
                UIManager.instance.stealingText.gameObject.SetActive(true);
                UIManager.instance.stealingText.text = gameObject.name + " is stealing your" + "<color=red>" + " brainrot" + "</color>";
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("inside"))
        {
            AIinsideHouse = false;
            temp = false;
        }
    }

    public void ForceChasePlayer()
    {
        if (ai == null || player == null) return;
        ai.SetCurrentTarget(player);
        ai.currentState = AIStates.Chase;
    }

    public void GoToHouse()
    {
        if (ai == null || AIStealingManager.instance.houseObjects.Count == 0) return;
        if (playerHouseLock != null && Lock != null && Lock.activeSelf) return;

        // Dynamically find a brainrot to steal
        Brainrot brainrot = AIStealingManager.instance.houseObjects[0].GetComponentInChildren<Brainrot>();
        if (brainrot != null)
        {
            targetBrainrot = brainrot;
        }

        goingToHouse = true;
        houseWaitTimer = waitAtHouseTime;

        ai.RemoveCurrentTarget();
        ai.SetCurrentTarget(AIStealingManager.instance.houseObjects[0].transform);
        ai.currentState = AIStates.Chase;
    }

    void PerformSteal()
    {
        if (targetBrainrot == null) return;

        hasStolen = true;
        isCarrying = true;  //  AI is now carrying something
        AIStealingManager.instance.AIStealBrainrot(targetBrainrot, carryPoint ? carryPoint : transform,gameObject.name);
        Debug.Log($"{gameObject.name} stole {targetBrainrot.name}");
    }

    public void DropCarriedObject()
    {
        // Call this when AI loses or drops the stolen object
        isCarrying = false;
        targetBrainrot = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, houseDetectionRadius);
    }

    public void HasVisitedReset()
    {
        hasVisitedHouse = false;
    }
}
