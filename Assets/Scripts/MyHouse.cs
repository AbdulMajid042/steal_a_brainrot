using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyHouse : MonoBehaviour
{
    public static MyHouse instance;
    [System.Serializable]
    public class Place
    {
        public Transform spot;
        public bool isOccupied =false;
    }
    public Place[] places;
    public int index;

    public GameObject myHouseLock;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public bool TryReserveFirstFreePlace(out int reservedIndex, out Transform reservedSpot)
    {
        for (int i = 0; i < places.Length; i++)
        {
            if (!places[i].isOccupied && places[i].spot != null)
            {
                places[i].isOccupied = true;
                reservedIndex = i;
                reservedSpot = places[i].spot;
                return true;
            }
        }
        reservedIndex = -1;
        reservedSpot = null;
        return false;
    }

    // Release a previously reserved place
    public void ReleasePlace(int index)
    {
        if (index >= 0 && index < places.Length)
            places[index].isOccupied = false;
    }
}
