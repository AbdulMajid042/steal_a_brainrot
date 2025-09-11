using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventoryItem
{
    public string itemName;          // Name of the item (e.g. "Axe")
    public Button itemButton;        // Button in the UI store
    public GameObject adIcon;        // Small AD icon on the button
    public bool unlockedByDefault;   // Axe = true
    public bool isUnlocked;          // Current state
}

public class PlayerInventory : MonoBehaviour
{
    public List<InventoryItem> items = new List<InventoryItem>();

    private void Start()
    {
        // Load saved unlock states
        foreach (var item in items)
        {
            if (item.unlockedByDefault)
            {
                item.isUnlocked = true;
            }
            else
            {
                item.isUnlocked = PlayerPrefs.GetInt("ItemUnlocked_" + item.itemName, 0) == 1;
            }

            UpdateUI(item);
        }
    }

    // Call this when player gets an item (via pickup or rewarded video)
    public void UnlockItem(string itemName)
    {
        var item = items.Find(i => i.itemName == itemName);
        if (item != null && !item.isUnlocked)
        {
            item.isUnlocked = true;

            // Save permanently
            PlayerPrefs.SetInt("ItemUnlocked_" + item.itemName, 1);
            PlayerPrefs.Save();

            UpdateUI(item);
        }
    }

    private void UpdateUI(InventoryItem item)
    {
        if (item.adIcon != null)
        {
            item.adIcon.SetActive(!item.isUnlocked);
        }
    }

    // Example: Hook this to button clicks in the store
    public void OnItemButtonClicked(string itemName)
    {
        var item = items.Find(i => i.itemName == itemName);
        if (item == null) return;

        if (!item.isUnlocked)
        {
            // Show rewarded ad first
            Debug.Log("Show rewarded ad for: " + itemName);

            // After success from AdManager, call:
            UnlockItem(itemName);
        }
        else
        {
            Debug.Log(itemName + " already unlocked, equip/use it.");
        }
    }
}