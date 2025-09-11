using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class EnableNavmeshSurface : MonoBehaviour
{
    private void Awake()
    {
        if(GetComponent<NavMeshSurface>())
        {
            GetComponent<NavMeshSurface>().enabled = true;
        }
    }

}
