using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Vector3 spawnPos;
    private Quaternion spawnRot;

    private void Start()
    {
        spawnPos = transform.GetChild(0).position;
        spawnRot = transform.GetChild(0).rotation;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveGameManager.instance.SetCurrentCheckpoint(spawnPos, spawnRot);
        }
    }
}
