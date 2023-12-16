using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraTargetBController : MonoBehaviour
{
    public Transform cameraTargetA;
    public Transform portal;
    public Transform otherPortal;


    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 playerOffsetFromPortal = cameraTargetA.position - otherPortal.position;
        transform.position = portal.position + playerOffsetFromPortal;

        float angularDifferenceBetweenPortalRotations = Quaternion.Angle(portal.rotation, otherPortal.rotation);

        Quaternion portalRotationDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
        Vector3 newCameraDirection = portalRotationDifference * cameraTargetA.forward;
        transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
    }
}
