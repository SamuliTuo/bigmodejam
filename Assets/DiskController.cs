using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskController : MonoBehaviourID
{
    [SerializeField] private float rotateSpeed = 1.0f;
    [SerializeField] private float bobSpeed = 1.0f;
    [SerializeField] private float bobRange = 1.0f;

    private Transform model;

    private void Start()
    {
        model = transform.GetChild(0);
        if (SaveGameManager.instance.GetUnlockedModes().Contains(PlayerModes.URBAN))
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        float distance = Mathf.Sin(Time.timeSinceLevelLoad * bobSpeed);
        model.localPosition = Vector3.up * distance * bobRange;
        model.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    bool looted = false;
    public void Loot()
    {
        if (looted)
            return;

        Destroy(gameObject);
    }
}
