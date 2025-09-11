using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBrainrots : MonoBehaviour
{
    public int howManyBrainrots;
    GameObject[] brainrots;

    private void Start()
    {
        // Populate children into array
        brainrots = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            brainrots[i] = transform.GetChild(i).gameObject;
            brainrots[i].SetActive(false); // disable all at start
        }

        EnableRandomBrainrots();
    }

    void EnableRandomBrainrots()
    {
        // Safety clamp (in case howManyBrainrots > available)
        int count = Mathf.Clamp(howManyBrainrots, 0, brainrots.Length);

        // Create a list of indexes to shuffle
        List<int> indexes = new List<int>();
        for (int i = 0; i < brainrots.Length; i++)
            indexes.Add(i);

        // Shuffle list
        for (int i = 0; i < indexes.Count; i++)
        {
            int rand = Random.Range(i, indexes.Count);
            int temp = indexes[i];
            indexes[i] = indexes[rand];
            indexes[rand] = temp;
        }

        // Enable first N shuffled brainrots
        for (int i = 0; i < count; i++)
        {
            brainrots[indexes[i]].SetActive(true);
        }
    }
}
