using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonController : MonoBehaviourID
{
    public bool zoneMode = false;
    public int value = 1;

    [HideInInspector] public bool wasCollected = false;

    [SerializeField] private float rotateSpeed = 1.0f;
    [SerializeField] private float bobSpeed = 1.0f;
    [SerializeField] private float bobRange = 1.0f;

    private Transform model;
    //Vector3 startPos;


    private void Start()
    {
        model = transform.GetChild(0);
        WasICollected();
    }

    private void Update()
    {
        float distance = Mathf.Sin(Time.timeSinceLevelLoad * bobSpeed);
        model.localPosition = Vector3.up * distance * bobRange;
        model.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }

    public void ThisWasCollected()
    {
        SaveGameManager.instance.IWasLooted_melon(this.ID);
        wasCollected = true;
        Destroy(gameObject);
    }

    void WasICollected()
    {
        print(ID+" asking if im collected");
        if (SaveGameManager.instance.WasILootedAlready_melon(this.ID))
        {
            print(ID + " i was collected");
            Destroy(gameObject);
        }
    }
}
