using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] private ThirdPersonController playerController = null;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            playerController.TeleportToZoneMode();
        }
    }
}
