using System.Collections.Generic;
using UnityEngine;

public class BrainrotSpawner : MonoBehaviour
{
    public GameObject[] brainrotsPrefab;
    List<int> generatedNumbers = new List<int>();
    int index;
    public int count;
    private void Start()
    {
        for(int i=0;i< count; i++)
        {
            SpawnBrainrots();
        }
    }
    public void SpawnBrainrots()
    {
        index = GetRandomNumber();
        if (generatedNumbers.Contains(index) ==false)
        {
            generatedNumbers.Add(index);
            GameObject go = Instantiate(brainrotsPrefab[index], transform.GetChild(index));
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;

        }
        else
        {
            SpawnBrainrots();
        }
    }

    int GetRandomNumber()
    {
        int num = Random.Range(0, brainrotsPrefab.Length);
        return num;
    }

}
