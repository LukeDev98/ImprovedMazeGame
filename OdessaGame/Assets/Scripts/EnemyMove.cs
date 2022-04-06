using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    public NavMeshAgent theAgent;
    public GameObject Player;
    bool first = true;
    public Camera cam;
    float elapsedtime = 0.0f;
    void Update()
    {
        elapsedtime += Time.deltaTime;
        if (elapsedtime >= 10)
        {
            theAgent.destination = Player.transform.position;
            elapsedtime = 0;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                theAgent.SetDestination(hit.point);
            }
        }

    }
}
