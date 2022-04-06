using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BakeNavMesh : MonoBehaviour
{
    [SerializeField]
    List<NavMeshSurface> navMeshSurfaces;


    void Awake()
    {
        Debug.Log("Here");
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach(GameObject G in allObjects)
        {
            if (G.GetComponent<NavMeshSurface>() != null)
            {
                Debug.Log("AAAAAAAAAAAA");
                navMeshSurfaces.Add(G.GetComponent<NavMeshSurface>());
            }
            
        }


        for(int i = 0; i < navMeshSurfaces.Count; i++)
        {
            navMeshSurfaces[i].BuildNavMesh();
        }
    }
}
