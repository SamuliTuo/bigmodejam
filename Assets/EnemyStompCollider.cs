using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStompCollider : MonoBehaviour
{
    EnemyController_basic controller;

    void Start()
    {
        controller = GetComponentInParent<EnemyController_basic>();
    }

    public void GetStompedOn()
    {
        controller.GotStomped();
    }
}
