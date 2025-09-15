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
            item.itemButton.onClick.AddListener(() => GetItemViaRewardedVideo(item.itemName, item.isUnlocked));
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

            weapon = GameObject.Find("vAxe");
            foreach (Transform t in weapon.transform)
            {
                t.gameObject.SetActive(false);
            }
            foreach (Transform t in weapon.transform)
            {
                if (t.gameObject.name == itemName)
                    t.gameObject.SetActive(true);
            }
        }
    }

    private void UpdateUI(InventoryItem item)
    {
        if (item.adIcon != null)
        {
            item.adIcon.SetActive(!item.isUnlocked);
        }
    }
    string itemString;
    GameObject weapon;
    // Example: Hook this to button clicks in the store
    public void OnItemButtonClicked()
    {
        var item = items.Find(i => i.itemName == itemString);
        if (item == null) return;

        if (!item.isUnlocked)
        {
            // After success from AdManager, call:
            UnlockItem(itemString);
        }
        weapon = GameObject.Find("vAxe");
        foreach (Transform t in weapon.transform)
        {
            t.gameObject.SetActive(false);
        }
        foreach (Transform t in weapon.transform)
        {
            if (t.gameObject.name == itemString)
                t.gameObject.SetActive(true);
        }
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
    public void GetItemViaRewardedVideo(string itemName, bool isUnlocked)
    {
        itemString = itemName;
        if (isUnlocked)
        {
            OnItemButtonClicked();
            return;
        }
        if (AdsManagerWrapper.Instance)
        {
            AdsManagerWrapper.Instance.ShowRewardedVideo(OnItemButtonClicked);
        }
    }
}