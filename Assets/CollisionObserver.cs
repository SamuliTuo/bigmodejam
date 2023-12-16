using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class CollisionObserver : MonoBehaviour
{
    ThirdPersonController _controller;
    PlayerAttacks _attacks;
    ObjectCollector _objectCollector;

    void Start()
    {
        _controller = GetComponent<ThirdPersonController>();
        _attacks = GetComponent<PlayerAttacks>();
        _objectCollector = GetComponent<ObjectCollector>();
    }

    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("EnemyStomper"))
        {
            hit.gameObject.GetComponent<EnemyStompCollider>()?.GetStompedOn();
            _controller.StompJump();
        }

        else if (hit.collider.CompareTag("Enemy"))
        {
            var script = hit.transform.root.GetComponent<EnemyController_basic>();
            if (script != null) 
            { 
                if (!script.pushed)
                {
                    _controller.GotHit();
                }
            }
        }

        else if (hit.collider.CompareTag("KillBox")) 
        {
            _controller.GotHit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Melon"))
        {
            MelonController m = other.gameObject.GetComponentInParent<MelonController>();
            if (m != null && m.wasCollected == false)
            {
                _objectCollector.CollectMelon(m);
                m.ThisWasCollected();
            }
        }

        else if (other.CompareTag("UrbanMode"))
        {
            SaveGameManager.instance.AddPlayerMode(PlayerModes.URBAN);
            _controller.CheckPlayerModes();
            _controller.ChangeMode(PlayerModes.URBAN);
            other.transform.root.GetComponent<DiskController>().Loot();
        }
    }
}
