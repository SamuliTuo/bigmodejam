using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var e = other.transform.root.GetComponent<EnemyController_basic>();
            e?.GotAttacked();
        }

        else if (other.CompareTag("Breakable"))
        {
            var obj = other.transform.root.GetComponent<BreakableObject>();
            obj?.DestroyMe();
        }

        else if (other.CompareTag("Trashcan"))
        {
            var obj = other.transform.root.GetComponent<TrashcanController>();
            obj?.DestroyMe();
        }
    }
}
