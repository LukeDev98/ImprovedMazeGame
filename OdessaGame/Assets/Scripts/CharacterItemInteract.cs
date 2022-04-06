using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterItemInteract : MonoBehaviour
{
    public Transform Player;

    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            LayerMask mask = LayerMask.GetMask("Collectable");
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 5.0f, mask))
            {
               Destroy(hit.transform.gameObject);

            }
        }
    }
}
